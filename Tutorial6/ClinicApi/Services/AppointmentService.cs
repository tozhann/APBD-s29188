using System.Data;
using ClinicApi.DTOs;
using Microsoft.Data.SqlClient;

namespace ClinicApi.Services;

public class AppointmentService
{
    private readonly string _connectionString;

    private static readonly HashSet<string> ValidStatuses =
        new(StringComparer.OrdinalIgnoreCase) { "Scheduled", "Completed", "Cancelled" };

    public AppointmentService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection is not configured.");
    }

    // GET /api/appointments
    public async Task<List<AppointmentListDto>> GetAppointmentsAsync(string? status, string? patientLastName)
    {
        var results = new List<AppointmentListDto>();

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new SqlCommand("""
            SELECT
                a.IdAppointment,
                a.AppointmentDate,
                a.Status,
                a.Reason,
                p.FirstName + N' ' + p.LastName AS PatientFullName,
                p.Email AS PatientEmail
            FROM dbo.Appointments a
            JOIN dbo.Patients p ON p.IdPatient = a.IdPatient
            WHERE (@Status IS NULL OR a.Status = @Status)
              AND (@PatientLastName IS NULL OR p.LastName = @PatientLastName)
            ORDER BY a.AppointmentDate;
            """, connection);

        command.Parameters.Add("@Status", SqlDbType.NVarChar, 50).Value =
            (object?)status ?? DBNull.Value;
        command.Parameters.Add("@PatientLastName", SqlDbType.NVarChar, 100).Value =
            (object?)patientLastName ?? DBNull.Value;

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            results.Add(new AppointmentListDto
            {
                IdAppointment   = reader.GetInt32(reader.GetOrdinal("IdAppointment")),
                AppointmentDate = reader.GetDateTime(reader.GetOrdinal("AppointmentDate")),
                Status          = reader.GetString(reader.GetOrdinal("Status")),
                Reason          = reader.GetString(reader.GetOrdinal("Reason")),
                PatientFullName = reader.GetString(reader.GetOrdinal("PatientFullName")),
                PatientEmail    = reader.GetString(reader.GetOrdinal("PatientEmail")),
            });
        }

        return results;
    }

    // GET /api/appointments/{id}
    public async Task<AppointmentDetailsDto?> GetAppointmentByIdAsync(int idAppointment)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new SqlCommand("""
            SELECT
                a.IdAppointment,
                a.AppointmentDate,
                a.Status,
                a.Reason,
                a.InternalNotes,
                a.CreatedAt,
                p.FirstName + N' ' + p.LastName AS PatientFullName,
                p.Email      AS PatientEmail,
                p.PhoneNumber AS PatientPhone,
                d.FirstName + N' ' + d.LastName AS DoctorFullName,
                d.LicenseNumber,
                s.Name       AS Specialization
            FROM dbo.Appointments a
            JOIN dbo.Patients p       ON p.IdPatient       = a.IdPatient
            JOIN dbo.Doctors  d       ON d.IdDoctor        = a.IdDoctor
            JOIN dbo.Specializations s ON s.IdSpecialization = d.IdSpecialization
            WHERE a.IdAppointment = @IdAppointment;
            """, connection);

        command.Parameters.Add("@IdAppointment", SqlDbType.Int).Value = idAppointment;

        await using var reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
            return null;

        return new AppointmentDetailsDto
        {
            IdAppointment   = reader.GetInt32(reader.GetOrdinal("IdAppointment")),
            AppointmentDate = reader.GetDateTime(reader.GetOrdinal("AppointmentDate")),
            Status          = reader.GetString(reader.GetOrdinal("Status")),
            Reason          = reader.GetString(reader.GetOrdinal("Reason")),
            InternalNotes   = reader.IsDBNull(reader.GetOrdinal("InternalNotes"))
                                  ? null
                                  : reader.GetString(reader.GetOrdinal("InternalNotes")),
            CreatedAt       = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
            PatientFullName = reader.GetString(reader.GetOrdinal("PatientFullName")),
            PatientEmail    = reader.GetString(reader.GetOrdinal("PatientEmail")),
            PatientPhone    = reader.IsDBNull(reader.GetOrdinal("PatientPhone"))
                                  ? null
                                  : reader.GetString(reader.GetOrdinal("PatientPhone")),
            DoctorFullName  = reader.GetString(reader.GetOrdinal("DoctorFullName")),
            DoctorLicenseNumber = reader.GetString(reader.GetOrdinal("LicenseNumber")),
            Specialization  = reader.GetString(reader.GetOrdinal("Specialization")),
        };
    }

    // POST /api/appointments
    public async Task<(int? newId, string? error, int statusCode)> CreateAppointmentAsync(
        CreateAppointmentRequestDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Reason))
            return (null, "Reason cannot be empty.", 400);

        if (dto.Reason.Length > 250)
            return (null, "Reason must be at most 250 characters.", 400);

        if (dto.AppointmentDate <= DateTime.UtcNow)
            return (null, "Appointment date cannot be in the past.", 400);

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        // Validate patient exists and is active
        if (!await EntityExistsAndActiveAsync(connection, "Patients", "IdPatient", dto.IdPatient))
            return (null, "Patient does not exist or is not active.", 400);

        // Validate doctor exists and is active
        if (!await EntityExistsAndActiveAsync(connection, "Doctors", "IdDoctor", dto.IdDoctor))
            return (null, "Doctor does not exist or is not active.", 400);

        // Check doctor schedule conflict
        if (await DoctorHasConflictAsync(connection, dto.IdDoctor, dto.AppointmentDate, excludeId: null))
            return (null, "The doctor already has a Scheduled appointment at that exact time.", 409);

        // Insert
        await using var insertCmd = new SqlCommand("""
            INSERT INTO dbo.Appointments
                (IdPatient, IdDoctor, AppointmentDate, Status, Reason, CreatedAt)
            OUTPUT INSERTED.IdAppointment
            VALUES
                (@IdPatient, @IdDoctor, @AppointmentDate, N'Scheduled', @Reason, GETDATE());
            """, connection);

        insertCmd.Parameters.Add("@IdPatient",       SqlDbType.Int).Value         = dto.IdPatient;
        insertCmd.Parameters.Add("@IdDoctor",        SqlDbType.Int).Value         = dto.IdDoctor;
        insertCmd.Parameters.Add("@AppointmentDate", SqlDbType.DateTime2).Value   = dto.AppointmentDate;
        insertCmd.Parameters.Add("@Reason",          SqlDbType.NVarChar, 250).Value = dto.Reason;

        var newId = (int)(await insertCmd.ExecuteScalarAsync())!;
        return (newId, null, 201);
    }

    // PUT /api/appointments/{id}
    public async Task<(bool found, string? error, int statusCode)> UpdateAppointmentAsync(
        int idAppointment, UpdateAppointmentRequestDto dto)
    {
        if (!ValidStatuses.Contains(dto.Status))
            return (true, "Status must be one of: Scheduled, Completed, Cancelled.", 400);

        if (string.IsNullOrWhiteSpace(dto.Reason))
            return (true, "Reason cannot be empty.", 400);

        if (dto.Reason.Length > 250)
            return (true, "Reason must be at most 250 characters.", 400);

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        // Load current appointment
        await using var selectCmd = new SqlCommand("""
            SELECT Status, AppointmentDate FROM dbo.Appointments
            WHERE IdAppointment = @IdAppointment;
            """, connection);
        selectCmd.Parameters.Add("@IdAppointment", SqlDbType.Int).Value = idAppointment;

        await using var reader = await selectCmd.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
            return (false, null, 404);

        var currentStatus  = reader.GetString(reader.GetOrdinal("Status"));
        var currentDate    = reader.GetDateTime(reader.GetOrdinal("AppointmentDate"));
        await reader.CloseAsync();

        // If already Completed, do not allow date change
        if (currentStatus == "Completed" && dto.AppointmentDate != currentDate)
            return (true, "Cannot change the date of a Completed appointment.", 409);

        // Validate patient and doctor
        if (!await EntityExistsAndActiveAsync(connection, "Patients", "IdPatient", dto.IdPatient))
            return (true, "Patient does not exist or is not active.", 400);

        if (!await EntityExistsAndActiveAsync(connection, "Doctors", "IdDoctor", dto.IdDoctor))
            return (true, "Doctor does not exist or is not active.", 400);

        // Check conflict only when date changes
        if (dto.AppointmentDate != currentDate)
        {
            if (await DoctorHasConflictAsync(connection, dto.IdDoctor, dto.AppointmentDate, excludeId: idAppointment))
                return (true, "The doctor already has a Scheduled appointment at that exact time.", 409);
        }

        await using var updateCmd = new SqlCommand("""
            UPDATE dbo.Appointments
            SET IdPatient       = @IdPatient,
                IdDoctor        = @IdDoctor,
                AppointmentDate = @AppointmentDate,
                Status          = @Status,
                Reason          = @Reason,
                InternalNotes   = @InternalNotes
            WHERE IdAppointment = @IdAppointment;
            """, connection);

        updateCmd.Parameters.Add("@IdPatient",       SqlDbType.Int).Value           = dto.IdPatient;
        updateCmd.Parameters.Add("@IdDoctor",        SqlDbType.Int).Value           = dto.IdDoctor;
        updateCmd.Parameters.Add("@AppointmentDate", SqlDbType.DateTime2).Value     = dto.AppointmentDate;
        updateCmd.Parameters.Add("@Status",          SqlDbType.NVarChar, 50).Value  = dto.Status;
        updateCmd.Parameters.Add("@Reason",          SqlDbType.NVarChar, 250).Value = dto.Reason;
        updateCmd.Parameters.Add("@InternalNotes",   SqlDbType.NVarChar, 1000).Value =
            (object?)dto.InternalNotes ?? DBNull.Value;
        updateCmd.Parameters.Add("@IdAppointment",   SqlDbType.Int).Value           = idAppointment;

        await updateCmd.ExecuteNonQueryAsync();
        return (true, null, 200);
    }

    // DELETE /api/appointments/{id}
    public async Task<(bool found, string? error, int statusCode)> DeleteAppointmentAsync(int idAppointment)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var selectCmd = new SqlCommand("""
            SELECT Status FROM dbo.Appointments WHERE IdAppointment = @IdAppointment;
            """, connection);
        selectCmd.Parameters.Add("@IdAppointment", SqlDbType.Int).Value = idAppointment;

        var statusObj = await selectCmd.ExecuteScalarAsync();
        if (statusObj is null or DBNull)
            return (false, null, 404);

        var currentStatus = (string)statusObj;
        if (currentStatus == "Completed")
            return (true, "Cannot delete a Completed appointment.", 409);

        await using var deleteCmd = new SqlCommand("""
            DELETE FROM dbo.Appointments WHERE IdAppointment = @IdAppointment;
            """, connection);
        deleteCmd.Parameters.Add("@IdAppointment", SqlDbType.Int).Value = idAppointment;

        await deleteCmd.ExecuteNonQueryAsync();
        return (true, null, 204);
    }

    // Helpers
    private static async Task<bool> EntityExistsAndActiveAsync(
        SqlConnection connection, string table, string idColumn, int id)
    {
        await using var cmd = new SqlCommand(
            $"SELECT COUNT(1) FROM dbo.{table} WHERE {idColumn} = @Id AND IsActive = 1;",
            connection);
        cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
        var count = (int)(await cmd.ExecuteScalarAsync())!;
        return count > 0;
    }

    private static async Task<bool> DoctorHasConflictAsync(
        SqlConnection connection, int idDoctor, DateTime appointmentDate, int? excludeId)
    {
        var sql = """
            SELECT COUNT(1) FROM dbo.Appointments
            WHERE IdDoctor        = @IdDoctor
              AND AppointmentDate = @AppointmentDate
              AND Status          = N'Scheduled'
            """;

        if (excludeId.HasValue)
            sql += " AND IdAppointment <> @ExcludeId";

        sql += ";";

        await using var cmd = new SqlCommand(sql, connection);
        cmd.Parameters.Add("@IdDoctor",        SqlDbType.Int).Value       = idDoctor;
        cmd.Parameters.Add("@AppointmentDate", SqlDbType.DateTime2).Value = appointmentDate;
        if (excludeId.HasValue)
            cmd.Parameters.Add("@ExcludeId", SqlDbType.Int).Value = excludeId.Value;

        var count = (int)(await cmd.ExecuteScalarAsync())!;
        return count > 0;
    }
}

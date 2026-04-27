using ClinicApi.DTOs;
using ClinicApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClinicApi.Controllers;

[ApiController]
[Route("api/appointments")]
public class AppointmentsController : ControllerBase
{
    private readonly AppointmentService _service;

    public AppointmentsController(AppointmentService service)
    {
        _service = service;
    }

    // GET /api/appointments?status=Scheduled&patientLastName=Kowalska
    [HttpGet]
    public async Task<IActionResult> GetAppointments(
        [FromQuery] string? status,
        [FromQuery] string? patientLastName)
    {
        var appointments = await _service.GetAppointmentsAsync(status, patientLastName);
        return Ok(appointments);
    }

    // GET /api/appointments/{idAppointment}
    [HttpGet("{idAppointment:int}")]
    public async Task<IActionResult> GetAppointment(int idAppointment)
    {
        var dto = await _service.GetAppointmentByIdAsync(idAppointment);
        if (dto is null)
            return NotFound(new ErrorResponseDto { Message = $"Appointment {idAppointment} not found." });

        return Ok(dto);
    }

    // POST /api/appointments
    [HttpPost]
    public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentRequestDto request)
    {
        var (newId, error, statusCode) = await _service.CreateAppointmentAsync(request);

        if (error is not null)
        {
            var errDto = new ErrorResponseDto { Message = error };
            return statusCode switch
            {
                409 => Conflict(errDto),
                _   => BadRequest(errDto),
            };
        }

        return CreatedAtAction(
            nameof(GetAppointment),
            new { idAppointment = newId },
            new { idAppointment = newId });
    }

    // PUT /api/appointments/{idAppointment}
    [HttpPut("{idAppointment:int}")]
    public async Task<IActionResult> UpdateAppointment(
        int idAppointment,
        [FromBody] UpdateAppointmentRequestDto request)
    {
        var (found, error, statusCode) = await _service.UpdateAppointmentAsync(idAppointment, request);

        if (!found)
            return NotFound(new ErrorResponseDto { Message = $"Appointment {idAppointment} not found." });

        if (error is not null)
        {
            var errDto = new ErrorResponseDto { Message = error };
            return statusCode switch
            {
                409 => Conflict(errDto),
                _   => BadRequest(errDto),
            };
        }

        return Ok();
    }

    // DELETE /api/appointments/{idAppointment}
    [HttpDelete("{idAppointment:int}")]
    public async Task<IActionResult> DeleteAppointment(int idAppointment)
    {
        var (found, error, statusCode) = await _service.DeleteAppointmentAsync(idAppointment);

        if (!found)
            return NotFound(new ErrorResponseDto { Message = $"Appointment {idAppointment} not found." });

        if (error is not null)
            return Conflict(new ErrorResponseDto { Message = error });

        return NoContent();
    }
}

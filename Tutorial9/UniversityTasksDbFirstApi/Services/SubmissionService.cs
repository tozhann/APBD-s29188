using Microsoft.EntityFrameworkCore;
using UniversityTasksDbFirstApi.Data;
using UniversityTasksDbFirstApi.DTOs;
using UniversityTasksDbFirstApi.Models;

namespace UniversityTasksDbFirstApi.Services;

public class SubmissionService
{
    private readonly UniversityTasksDbContext _db;

    public SubmissionService(UniversityTasksDbContext db)
    {
        _db = db;
    }

    // POST /api/submissions
    public async Task<(SubmissionDto? result, string? error, int statusCode)> CreateAsync(CreateSubmissionDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.RepositoryUrl) || !dto.RepositoryUrl.StartsWith("https://"))
            return (null, "RepositoryUrl must start with 'https://'.", 400);

        var student = await _db.Students.AsNoTracking()
            .FirstOrDefaultAsync(s => s.StudentId == dto.StudentId);
        if (student is null)
            return (null, "Student not found.", 404);
        if (!student.IsActive)
            return (null, "Student is not active.", 400);

        var assignment = await _db.Assignments.AsNoTracking()
            .Include(a => a.Course)
            .FirstOrDefaultAsync(a => a.AssignmentId == dto.AssignmentId);
        if (assignment is null)
            return (null, "Assignment not found.", 404);
        if (!assignment.IsPublished)
            return (null, "Assignment is not published.", 400);

        var enrolled = await _db.Enrollments.AsNoTracking()
            .AnyAsync(e => e.StudentId == dto.StudentId
                        && e.CourseId == assignment.CourseId
                        && (e.Status == "Active" || e.Status == "Completed"));
        if (!enrolled)
            return (null, "Student is not enrolled in the course that owns this assignment.", 400);

        var duplicate = await _db.Submissions.AsNoTracking()
            .AnyAsync(s => s.AssignmentId == dto.AssignmentId && s.StudentId == dto.StudentId);
        if (duplicate)
            return (null, "Student has already submitted this assignment.", 409);

        var now = DateTime.UtcNow;
        var status = assignment.IsOverdue(now) ? "Late" : "Submitted";

        var submission = new Submission
        {
            AssignmentId  = dto.AssignmentId,
            StudentId     = dto.StudentId,
            RepositoryUrl = dto.RepositoryUrl,
            SubmittedAt   = now,
            Status        = status
        };

        _db.Submissions.Add(submission);
        await _db.SaveChangesAsync();

        return (new SubmissionDto
        {
            SubmissionId       = submission.SubmissionId,
            StudentFullName    = student.FullName,
            StudentIndexNumber = student.IndexNumber,
            AssignmentTitle    = assignment.Title,
            RepositoryUrl      = submission.RepositoryUrl,
            SubmittedAt        = submission.SubmittedAt,
            Status             = submission.Status,
            Score              = submission.Score,
            Feedback           = submission.Feedback
        }, null, 201);
    }

    // PUT /api/submissions/{id}/grade
    public async Task<(bool found, string? error)> GradeAsync(int submissionId, GradeSubmissionDto dto)
    {
        var submission = await _db.Submissions
            .Include(s => s.Assignment)
            .FirstOrDefaultAsync(s => s.SubmissionId == submissionId);

        if (submission is null) return (false, null);

        if (dto.Score < 0)
            return (true, "Score cannot be lower than 0.");
        if (dto.Score > submission.Assignment.MaxPoints)
            return (true, $"Score cannot exceed MaxPoints ({submission.Assignment.MaxPoints}).");

        // Change Tracker in action — load, modify, save
        submission.Score    = dto.Score;
        submission.Feedback = dto.Feedback;
        submission.Status   = "Graded";

        await _db.SaveChangesAsync();
        return (true, null);
    }

    // DELETE /api/submissions/{id}
    public async Task<(bool found, string? error)> DeleteAsync(int submissionId)
    {
        var submission = await _db.Submissions.FindAsync(submissionId);
        if (submission is null) return (false, null);

        if (submission.Status == "Graded")
            return (true, "Cannot delete a graded submission.");

        _db.Submissions.Remove(submission);
        await _db.SaveChangesAsync();
        return (true, null);
    }
}

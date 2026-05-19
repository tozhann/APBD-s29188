using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityTasksDbFirstApi.Data;
using UniversityTasksDbFirstApi.DTOs;

namespace UniversityTasksDbFirstApi.Controllers;

[ApiController]
[Route("api/students")]
public class StudentsController : ControllerBase
{
    private readonly UniversityTasksDbContext _db;

    public StudentsController(UniversityTasksDbContext db)
    {
        _db = db;
    }

    // GET /api/students/{idStudent}/dashboard
    [HttpGet("{idStudent:int}/dashboard")]
    public async Task<IActionResult> GetDashboard(int idStudent)
    {
        // Single query with Include + ThenInclude — avoids N+1
        var student = await _db.Students
            .AsNoTracking()
            .Include(s => s.Enrollments)
                .ThenInclude(e => e.Course)
            .Include(s => s.Submissions)
                .ThenInclude(sub => sub.Assignment)
            .FirstOrDefaultAsync(s => s.StudentId == idStudent);

        if (student is null)
            return NotFound(new { message = $"Student {idStudent} not found." });

        var dto = new StudentDashboardDto
        {
            StudentId   = student.StudentId,
            IndexNumber = student.IndexNumber,
            FullName    = student.FullName,
            IsActive    = student.IsActive,
            Enrollments = student.Enrollments.Select(e => new EnrollmentInfoDto
            {
                EnrollmentId = e.EnrollmentId,
                CourseName   = e.Course.Name,
                CourseCode   = e.Course.Code,
                Status       = e.Status,
                EnrolledAt   = e.EnrolledAt
            }).ToList(),
            Submissions = student.Submissions.Select(s => new SubmissionInfoDto
            {
                SubmissionId    = s.SubmissionId,
                AssignmentTitle = s.Assignment.Title,
                RepositoryUrl   = s.RepositoryUrl,
                Status          = s.Status,
                Score           = s.Score
            }).ToList()
        };

        return Ok(dto);
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityTasksDbFirstApi.Data;
using UniversityTasksDbFirstApi.DTOs;

namespace UniversityTasksDbFirstApi.Controllers;

[ApiController]
[Route("api/courses")]
public class CoursesController : ControllerBase
{
    private readonly UniversityTasksDbContext _db;

    public CoursesController(UniversityTasksDbContext db)
    {
        _db = db;
    }

    // GET /api/courses?activeOnly=true
    [HttpGet]
    public async Task<IActionResult> GetCourses([FromQuery] bool activeOnly = false)
    {
        var query = _db.Courses.AsNoTracking();

        if (activeOnly)
            query = query.Where(c => c.IsActive);

        var courses = await query
            .Select(c => new CourseDto
            {
                CourseId        = c.CourseId,
                Code            = c.Code,
                Name            = c.Name,
                Credits         = c.Credits,
                IsActive        = c.IsActive,
                AssignmentCount = c.Assignments.Count
            })
            .ToListAsync();

        return Ok(courses);
    }

    // GET /api/courses/{idCourse}/assignments?publishedOnly=true
    [HttpGet("{idCourse:int}/assignments")]
    public async Task<IActionResult> GetAssignments(int idCourse, [FromQuery] bool publishedOnly = false)
    {
        var courseExists = await _db.Courses.AsNoTracking()
            .AnyAsync(c => c.CourseId == idCourse);

        if (!courseExists)
            return NotFound(new { message = $"Course {idCourse} not found." });

        var query = _db.Assignments.AsNoTracking()
            .Where(a => a.CourseId == idCourse);

        if (publishedOnly)
            query = query.Where(a => a.IsPublished);

        var assignments = await query
            .Select(a => new AssignmentDto
            {
                AssignmentId    = a.AssignmentId,
                Title           = a.Title,
                DueDate         = a.DueDate,
                MaxPoints       = a.MaxPoints,
                IsPublished     = a.IsPublished,
                SubmissionCount = a.Submissions.Count
            })
            .ToListAsync();

        return Ok(assignments);
    }
}

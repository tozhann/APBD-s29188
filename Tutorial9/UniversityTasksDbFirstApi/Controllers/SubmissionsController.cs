using Microsoft.AspNetCore.Mvc;
using UniversityTasksDbFirstApi.DTOs;
using UniversityTasksDbFirstApi.Services;

namespace UniversityTasksDbFirstApi.Controllers;

[ApiController]
[Route("api/submissions")]
public class SubmissionsController : ControllerBase
{
    private readonly SubmissionService _service;

    public SubmissionsController(SubmissionService service)
    {
        _service = service;
    }

    // POST /api/submissions
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSubmissionDto dto)
    {
        var (result, error, statusCode) = await _service.CreateAsync(dto);

        if (error is not null)
        {
            return statusCode switch
            {
                404 => NotFound(new { message = error }),
                409 => Conflict(new { message = error }),
                _   => BadRequest(new { message = error })
            };
        }

        return CreatedAtAction(nameof(Create), new { id = result!.SubmissionId }, result);
    }

    // PUT /api/submissions/{idSubmission}/grade
    [HttpPut("{idSubmission:int}/grade")]
    public async Task<IActionResult> Grade(int idSubmission, [FromBody] GradeSubmissionDto dto)
    {
        var (found, error) = await _service.GradeAsync(idSubmission, dto);

        if (!found)
            return NotFound(new { message = $"Submission {idSubmission} not found." });

        if (error is not null)
            return BadRequest(new { message = error });

        return Ok();
    }

    // DELETE /api/submissions/{idSubmission}
    [HttpDelete("{idSubmission:int}")]
    public async Task<IActionResult> Delete(int idSubmission)
    {
        var (found, error) = await _service.DeleteAsync(idSubmission);

        if (!found)
            return NotFound(new { message = $"Submission {idSubmission} not found." });

        if (error is not null)
            return BadRequest(new { message = error });

        return NoContent();
    }
}

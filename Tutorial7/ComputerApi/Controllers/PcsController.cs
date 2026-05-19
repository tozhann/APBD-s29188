using ComputerApi.DTOs;
using ComputerApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace ComputerApi.Controllers;

[ApiController]
[Route("api/pcs")]
public class PcsController : ControllerBase
{
    private readonly IPcService _service;

    public PcsController(IPcService service)
    {
        _service = service;
    }

    // GET /api/pcs
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var pcs = await _service.GetAllAsync();
        return Ok(pcs);
    }

    // GET /api/pcs/{id}/components
    [HttpGet("{id:int}/components")]
    public async Task<IActionResult> GetComponents(int id)
    {
        var result = await _service.GetComponentsAsync(id);
        if (result is null)
            return NotFound(new { message = $"PC with id {id} not found." });

        return Ok(result);
    }

    // POST /api/pcs
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePcDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetAll), new { id = created.Id }, created);
    }

    // PUT /api/pcs/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePcDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var updated = await _service.UpdateAsync(id, dto);
        if (updated is null)
            return NotFound(new { message = $"PC with id {id} not found." });

        return Ok(updated);
    }

    // DELETE /api/pcs/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted)
            return NotFound(new { message = $"PC with id {id} not found." });

        return NoContent();
    }
}

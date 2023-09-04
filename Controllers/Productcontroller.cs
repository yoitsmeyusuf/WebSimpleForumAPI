using Takasbu.Models;
using Takasbu.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Takasbu.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class SubjectsController : ControllerBase
{
    private readonly SubjectService _SubjectsService;

    public SubjectsController(SubjectService SubjectsService) => _SubjectsService = SubjectsService;

    [HttpGet]
    public async Task<List<Subject>> Get() => await _SubjectsService.GetAsync();

    [HttpGet]
    public async Task<ActionResult<Subject>> Get(string id)
    {
        var Subject = await _SubjectsService.GetAsync(id);

        if (Subject is null)
        {
            return NotFound();
        }

        return Subject;
    }

    [HttpPost]
    public async Task<IActionResult> Post(Subject newSubject, IFormFile file)
    {
        await _SubjectsService.CreateAsync(newSubject);

        return CreatedAtAction(nameof(Get), new { id = newSubject.Id }, newSubject);
    }

    [HttpPut]
    public async Task<IActionResult> Update(string id, Subject updatedSubject)
    {
        var Subject = await _SubjectsService.GetAsync(id);

        if (Subject is null)
        {
            return NotFound();
        }

        updatedSubject.Id = Subject.Id;

        await _SubjectsService.UpdateAsync(id, updatedSubject);

        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(string id)
    {
        var Subject = await _SubjectsService.GetAsync(id);

        if (Subject is null)
        {
            return NotFound();
        }

        await _SubjectsService.RemoveAsync(id);

        return NoContent();
    }
}

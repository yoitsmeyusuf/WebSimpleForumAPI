using ForumApi.Models;
using ForumApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ForumApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class CommentsController : ControllerBase
{
    private readonly CommentService _CommentsService;

    public CommentsController(CommentService CommentsService) => _CommentsService = CommentsService;

    [HttpGet]
    public async Task<List<Comment>> Get() => await _CommentsService.GetAsync();

    [HttpGet]
    public async Task<ActionResult<Comment>> Get(string id)
    {
        var Comment = await _CommentsService.GetAsync(id);

        if (Comment is null)
        {
            return NotFound();
        }

        return Comment;
    }

    [HttpPost]
    public async Task<IActionResult> Post(Comment newComment)
    {
        await _CommentsService.CreateAsync(newComment);

        return CreatedAtAction(nameof(Get), new { id = newComment.Id }, newComment);
    }

    [HttpPut]
    public async Task<IActionResult> Update(string id, Comment updatedComment)
    {
        var Comment = await _CommentsService.GetAsync(id);

        if (Comment is null)
        {
            return NotFound();
        }

        updatedComment.Id = Comment.Id;

        await _CommentsService.UpdateAsync(id, updatedComment);

        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(string id)
    {
        var Comment = await _CommentsService.GetAsync(id);

        if (Comment is null)
        {
            return NotFound();
        }

        await _CommentsService.RemoveAsync(id);

        return NoContent();
    }
}

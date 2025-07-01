using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebAPI.Data;
using WebAPI.Data.Entities;

namespace WebAPI.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class CommentController : ControllerBase
	{
		private readonly HeritageDbContext _context;
		public CommentController(HeritageDbContext context) => _context = context;

		[HttpGet("heritage/{heritageId}")]
		public async Task<IActionResult> GetForHeritage(int heritageId)
		{
			var comments = await _context.Comment
				.Include(c => c.ApplicationUser)
				.Where(c => c.CulturalHeritageId == heritageId && c.Approved)
				.ToListAsync();

			return Ok(comments);
		}

		[HttpPost]
		[Authorize]
		public async Task<IActionResult> Post([FromBody] Comment model)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);

			model.Timestamp = DateTime.UtcNow;
			model.Approved = false;

			_context.Comment.Add(model);
			await _context.SaveChangesAsync();

			return Ok(model);
		}

		[HttpPut("{id}/approve")]
		[Authorize]
		public async Task<IActionResult> Approve(int id)
		{
			var comment = await _context.Comment.FindAsync(id);
			if (comment == null) return NotFound();

			comment.Approved = true;
			await _context.SaveChangesAsync();

			return NoContent();
		}

		[HttpDelete("{id}")]
		[Authorize]
		public async Task<IActionResult> Delete(int id)
		{
			var comment = await _context.Comment.FindAsync(id);
			if (comment == null) return NotFound();

			_context.Comment.Remove(comment);
			await _context.SaveChangesAsync();

			return NoContent();
		}
	}
}

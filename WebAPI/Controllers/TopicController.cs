using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebAPI.Data;

namespace WebAPI.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class TopicController : ControllerBase
	{
		private readonly HeritageDbContext _context;
		public TopicController(HeritageDbContext context) => _context = context;

		[HttpGet]
		public async Task<IActionResult> GetAll() =>
			Ok(await _context.Topic.ToListAsync());

		[HttpPost]
		[Authorize]
		public async Task<IActionResult> Create([FromBody] Data.Entities.Topic model)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);

			_context.Topic.Add(model);
			await _context.SaveChangesAsync();
			return CreatedAtAction(nameof(GetAll), new { id = model.Id }, model);
		}

		[HttpPut("{id}")]
		[Authorize]
		public async Task<IActionResult> Update(int id, [FromBody] Data.Entities.Topic model)
		{
			if (id != model.Id || !ModelState.IsValid) return BadRequest();

			var exists = await _context.Topic.AnyAsync(t => t.Id == id);
			if (!exists) return NotFound();

			_context.Entry(model).State = EntityState.Modified;
			await _context.SaveChangesAsync();
			return NoContent();
		}

		[HttpDelete("{id}")]
		[Authorize]
		public async Task<IActionResult> Delete(int id)
		{
			var entity = await _context.Topic.FindAsync(id);
			if (entity == null) return NotFound();

			_context.Topic.Remove(entity);
			await _context.SaveChangesAsync();
			return NoContent();
		}
	}
}

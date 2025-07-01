using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebAPI.Data;

namespace WebAPI.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class NationalMinorityController : ControllerBase
	{
		private readonly HeritageDbContext _context;
		public NationalMinorityController(HeritageDbContext context) => _context = context;

		[HttpGet]
		public async Task<IActionResult> GetAll() =>
			Ok(await _context.NationalMinority.ToListAsync());

		[HttpGet("{id}")]
		public async Task<IActionResult> Get(int id)
		{
			var item = await _context.NationalMinority.FindAsync(id);
			return item == null ? NotFound() : Ok(item);
		}

		[HttpPost]
		[Authorize]
		public async Task<IActionResult> Create([FromBody] Data.Entities.NationalMinority model)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);

			_context.NationalMinority.Add(model);
			await _context.SaveChangesAsync();
			return CreatedAtAction(nameof(Get), new { id = model.Id }, model);
		}

		[HttpPut("{id}")]
		[Authorize]
		public async Task<IActionResult> Update(int id, [FromBody] Data.Entities.NationalMinority model)
		{
			if (id != model.Id || !ModelState.IsValid) return BadRequest();

			var exists = await _context.NationalMinority.AnyAsync(n => n.Id == id);
			if (!exists) return NotFound();

			_context.Entry(model).State = EntityState.Modified;
			await _context.SaveChangesAsync();
			return NoContent();
		}

		[HttpDelete("{id}")]
		[Authorize]
		public async Task<IActionResult> Delete(int id)
		{
			var entity = await _context.NationalMinority.FindAsync(id);
			if (entity == null) return NotFound();

			_context.NationalMinority.Remove(entity);
			await _context.SaveChangesAsync();
			return NoContent();
		}
	}
}

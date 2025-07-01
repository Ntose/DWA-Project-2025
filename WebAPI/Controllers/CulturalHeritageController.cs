using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Data;
using WebAPI.Data.Entities;

namespace WebAPI.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class CulturalHeritageController : ControllerBase
	{
		private readonly HeritageDbContext _context;

		public CulturalHeritageController(HeritageDbContext context)
		{
			_context = context;
		}

		// GET api/culturalheritage
		[HttpGet]
		public async Task<IActionResult> GetAll([FromQuery] string name = null, [FromQuery] int? minorityId = null, [FromQuery] int page = 1, [FromQuery] int count = 10)
		{
			var query = _context.CulturalHeritage
				.Include(ch => ch.NationalMinority)
				.Include(ch => ch.CulturalHeritageTopics).ThenInclude(ct => ct.Topic)
				.AsQueryable();

			if (!string.IsNullOrEmpty(name))
				query = query.Where(ch => ch.Name.Contains(name));

			if (minorityId.HasValue)
				query = query.Where(ch => ch.NationalMinorityId == minorityId.Value);

			var results = await query
				.OrderByDescending(ch => ch.DateAdded)
				.Skip((page - 1) * count)
				.Take(count)
				.ToListAsync();

			return Ok(results);
		}

		// GET api/culturalheritage/{id}
		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(int id)
		{
			var heritage = await _context.CulturalHeritage
				.Include(ch => ch.NationalMinority)
				.Include(ch => ch.CulturalHeritageTopics).ThenInclude(ct => ct.Topic)
				.Include(ch => ch.Comments.Where(c => c.Approved)).ThenInclude(c => c.ApplicationUser)
				.FirstOrDefaultAsync(ch => ch.Id == id);

			return heritage is null ? NotFound() : Ok(heritage);
		}

		// POST api/culturalheritage
		[HttpPost]
		public async Task<IActionResult> Create([FromBody] CulturalHeritage model)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			_context.CulturalHeritage.Add(model);
			await _context.SaveChangesAsync();

			if (model.CulturalHeritageTopics != null)
			{
				foreach (var rel in model.CulturalHeritageTopics)
					rel.CulturalHeritageId = model.Id;

				_context.CulturalHeritageTopic.AddRange(model.CulturalHeritageTopics);
				await _context.SaveChangesAsync();
			}

			return CreatedAtAction(nameof(GetById), new { id = model.Id }, model);
		}

		// PUT api/culturalheritage/{id}
		[HttpPut("{id}")]
		public async Task<IActionResult> Update(int id, [FromBody] CulturalHeritage updated)
		{
			if (id != updated.Id)
				return BadRequest("ID mismatch");

			var heritage = await _context.CulturalHeritage
				.Include(ch => ch.CulturalHeritageTopics)
				.FirstOrDefaultAsync(ch => ch.Id == id);

			if (heritage == null)
				return NotFound();

			heritage.Name = updated.Name;
			heritage.Description = updated.Description;
			heritage.ImageUrl = updated.ImageUrl;
			heritage.NationalMinorityId = updated.NationalMinorityId;

			_context.CulturalHeritageTopic.RemoveRange(heritage.CulturalHeritageTopics);

			if (updated.CulturalHeritageTopics != null)
			{
				foreach (var rel in updated.CulturalHeritageTopics)
					rel.CulturalHeritageId = id;

				_context.CulturalHeritageTopic.AddRange(updated.CulturalHeritageTopics);
			}

			await _context.SaveChangesAsync();
			return NoContent();
		}

		// DELETE api/culturalheritage/{id}
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			var heritage = await _context.CulturalHeritage.FindAsync(id);
			if (heritage == null)
				return NotFound();

			_context.CulturalHeritage.Remove(heritage);
			await _context.SaveChangesAsync();

			return NoContent();
		}
	}
}

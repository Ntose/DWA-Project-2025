using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Data;
using WebAPI.Data.Entities;
using WebAPI.Dtos.CulturalHeritage;
using WebAPI.Infrastructure;  // for PagedResult<T>
using WebAPI.Data.Entities;

namespace WebAPI.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]  // all actions require a valid JWT unless overridden
	public class CulturalHeritageController : ControllerBase
	{
		private readonly HeritageDbContext _context;
		private readonly IMapper _mapper;

		public CulturalHeritageController(
			HeritageDbContext context,
			IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		// GET: api/CulturalHeritage
		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> GetAll()
		{
			var list = await _context.CulturalHeritage
				.Include(ch => ch.NationalMinority)
				.Include(ch => ch.CulturalHeritageTopics)
					.ThenInclude(ct => ct.Topic)
				.ToListAsync();

			var dtos = _mapper.Map<CulturalHeritageReadDto[]>(list);
			return Ok(dtos);
		}

		// GET: api/CulturalHeritage/{id}
		[HttpGet("{id}")]
		[AllowAnonymous]
		public async Task<IActionResult> Get(int id)
		{
			var ent = await _context.CulturalHeritage
				.Include(ch => ch.NationalMinority)
				.Include(ch => ch.CulturalHeritageTopics)
					.ThenInclude(ct => ct.Topic)
				.FirstOrDefaultAsync(ch => ch.Id == id);

			if (ent == null)
				return NotFound();

			var dto = _mapper.Map<CulturalHeritageReadDto>(ent);
			return Ok(dto);
		}

		// GET: api/CulturalHeritage/search?term=foo&page=1&count=10
		[HttpGet("search")]
		[AllowAnonymous]
		public async Task<IActionResult> Search(
			string? term,
			int page = 1,
			int count = 10)
		{
			if (page < 1 || count < 1)
				return BadRequest("Page and count must be positive integers.");

			var query = _context.CulturalHeritage
				.Include(ch => ch.NationalMinority)
				.Include(ch => ch.CulturalHeritageTopics)
					.ThenInclude(ct => ct.Topic)
				.AsQueryable();

			if (!string.IsNullOrWhiteSpace(term))
			{
				query = query.Where(ch =>
					ch.Name.Contains(term) ||
					(ch.Description != null && ch.Description.Contains(term)));
			}

			var total = await query.CountAsync();
			var items = await query
				.Skip((page - 1) * count)
				.Take(count)
				.ToListAsync();

			var dtos = _mapper.Map<CulturalHeritageReadDto[]>(items);

			var result = new PagedResult<CulturalHeritageReadDto>
			{
				Page = page,
				Count = count,
				TotalItems = total,
				Items = dtos
			};

			return Ok(result);
		}

		// POST: api/CulturalHeritage
		[HttpPost]
		public async Task<IActionResult> Create([FromBody] CulturalHeritageCreateDto createDto)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			// Map to entity and populate topics bridge
			var ent = _mapper.Map<CulturalHeritage>(createDto);
			foreach (var topicId in createDto.TopicIds.Distinct())
			{
				ent.CulturalHeritageTopics
				   .Add(new CulturalHeritageTopic { TopicId = topicId });
			}

			try
			{
				_context.CulturalHeritage.Add(ent);
				await _context.SaveChangesAsync();

				// Log success
				_context.Log.Add(new Log
				{
					Level = "Info",
					Message = $"CulturalHeritage with id={ent.Id} created."
				});
				await _context.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				// Log error then rethrow to be handled by middleware
				_context.Log.Add(new Log
				{
					Level = "Error",
					Message = $"Error creating CulturalHeritage: {ex.Message}"
				});
				await _context.SaveChangesAsync();
				throw;
			}

			// Reload with includes to return full DTO
			var created = await _context.CulturalHeritage
				.Include(ch => ch.NationalMinority)
				.Include(ch => ch.CulturalHeritageTopics)
					.ThenInclude(ct => ct.Topic)
				.FirstOrDefaultAsync(ch => ch.Id == ent.Id);

			var readDto = _mapper.Map<CulturalHeritageReadDto>(created!);
			return CreatedAtAction(nameof(Get), new { id = readDto.Id }, readDto);
		}

		// PUT: api/CulturalHeritage/{id}
		[HttpPut("{id}")]
		public async Task<IActionResult> Update(
			int id,
			[FromBody] CulturalHeritageUpdateDto updateDto)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var ent = await _context.CulturalHeritage
				.Include(ch => ch.CulturalHeritageTopics)
				.FirstOrDefaultAsync(ch => ch.Id == id);

			if (ent == null)
				return NotFound();

			// Update scalar properties
			ent.Name = updateDto.Name;
			ent.Description = updateDto.Description;
			ent.ImageUrl = updateDto.ImageUrl;
			ent.NationalMinorityId = updateDto.NationalMinorityId;

			// Reset topics
			ent.CulturalHeritageTopics.Clear();
			foreach (var topicId in updateDto.TopicIds.Distinct())
			{
				ent.CulturalHeritageTopics
				   .Add(new CulturalHeritageTopic { CulturalHeritageId = id, TopicId = topicId });
			}

			try
			{
				await _context.SaveChangesAsync();

				_context.Log.Add(new Log
				{
					Level = "Info",
					Message = $"CulturalHeritage with id={id} updated."
				});
				await _context.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				_context.Log.Add(new Log
				{
					Level = "Error",
					Message = $"Error updating CulturalHeritage id={id}: {ex.Message}"
				});
				await _context.SaveChangesAsync();
				throw;
			}

			return NoContent();
		}

		// DELETE: api/CulturalHeritage/{id}
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			var ent = await _context.CulturalHeritage.FindAsync(id);
			if (ent == null)
				return NotFound();

			try
			{
				_context.CulturalHeritage.Remove(ent);
				await _context.SaveChangesAsync();

				_context.Log.Add(new Log
				{
					Level = "Info",
					Message = $"CulturalHeritage with id={id} deleted."
				});
				await _context.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				_context.Log.Add(new Log
				{
					Level = "Error",
					Message = $"Error deleting CulturalHeritage id={id}: {ex.Message}"
				});
				await _context.SaveChangesAsync();
				throw;
			}

			return NoContent();
		}
	}
}

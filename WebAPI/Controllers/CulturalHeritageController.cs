using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebAPI.Data;
using WebAPI.Data.Entities;
using WebAPI.Dtos.CulturalHeritage;

namespace WebAPI.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class CulturalHeritageController : ControllerBase
	{
		private readonly HeritageDbContext _context;
		private readonly IMapper _mapper;

		public CulturalHeritageController(HeritageDbContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		// GET: api/CulturalHeritage
		[HttpGet]
		public async Task<ActionResult<IEnumerable<CulturalHeritageReadDto>>> GetAll()
		{
			var entities = await _context.CulturalHeritage
				.Include(ch => ch.NationalMinority)
				.ToListAsync();

			return Ok(_mapper.Map<IEnumerable<CulturalHeritageReadDto>>(entities));
		}

		// GET: api/CulturalHeritage/5
		[HttpGet("{id}")]
		public async Task<ActionResult<CulturalHeritageReadDto>> Get(int id)
		{
			var entity = await _context.CulturalHeritage
				.Include(ch => ch.NationalMinority)
				.FirstOrDefaultAsync(ch => ch.Id == id);

			if (entity == null) return NotFound();
			return Ok(_mapper.Map<CulturalHeritageReadDto>(entity));
		}

		// POST: api/CulturalHeritage
		[HttpPost]
		[Authorize]
		public async Task<ActionResult<CulturalHeritageReadDto>> Create(
			[FromBody] CulturalHeritageCreateDto createDto)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var entity = _mapper.Map<CulturalHeritage>(createDto);
			_context.CulturalHeritage.Add(entity);
			await _context.SaveChangesAsync();

			var readDto = _mapper.Map<CulturalHeritageReadDto>(entity);
			return CreatedAtAction(nameof(Get), new { id = readDto.Id }, readDto);
		}

		// PUT: api/CulturalHeritage/5
		[HttpPut("{id}")]
		[Authorize]
		public async Task<IActionResult> Update(
			int id,
			[FromBody] CulturalHeritageUpdateDto updateDto)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var entity = await _context.CulturalHeritage.FindAsync(id);
			if (entity == null) return NotFound();

			_mapper.Map(updateDto, entity);
			_context.Entry(entity).State = EntityState.Modified;
			await _context.SaveChangesAsync();

			return NoContent();
		}

		// DELETE: api/CulturalHeritage/5
		[HttpDelete("{id}")]
		[Authorize]
		public async Task<IActionResult> Delete(int id)
		{
			var entity = await _context.CulturalHeritage.FindAsync(id);
			if (entity == null) return NotFound();

			_context.CulturalHeritage.Remove(entity);
			await _context.SaveChangesAsync();
			return NoContent();
		}
	}
}

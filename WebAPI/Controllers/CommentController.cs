using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebAPI.Data;
using WebAPI.Data.Entities;
using WebAPI.Dtos.Comment;

namespace WebAPI.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class CommentController : ControllerBase
	{
		private readonly HeritageDbContext _context;
		private readonly IMapper _mapper;

		public CommentController(HeritageDbContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		// GET: api/Comment/heritage/5
		[HttpGet("heritage/{heritageId}")]
		public async Task<ActionResult<IEnumerable<CommentReadDto>>> GetForHeritage(int heritageId)
		{
			var entities = await _context.Comment
				.Include(c => c.ApplicationUser)
				.Where(c => c.CulturalHeritageId == heritageId && c.Approved)
				.ToListAsync();

			return Ok(_mapper.Map<IEnumerable<CommentReadDto>>(entities));
		}

		// POST: api/Comment
		[HttpPost]
		[Authorize]
		public async Task<ActionResult<CommentReadDto>> Create(
			[FromBody] CommentCreateDto createDto)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var entity = _mapper.Map<Comment>(createDto);
			entity.Timestamp = DateTime.UtcNow;
			entity.Approved = false;

			_context.Comment.Add(entity);
			await _context.SaveChangesAsync();

			var readDto = _mapper.Map<CommentReadDto>(entity);
			return Ok(readDto);
		}

		// PUT: api/Comment/5/approve
		[HttpPut("{id}/approve")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Approve(int id)
		{
			var entity = await _context.Comment.FindAsync(id);
			if (entity == null) return NotFound();

			entity.Approved = true;
			await _context.SaveChangesAsync();
			return NoContent();
		}

		// DELETE: api/Comment/5
		[HttpDelete("{id}")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Delete(int id)
		{
			var entity = await _context.Comment.FindAsync(id);
			if (entity == null) return NotFound();

			_context.Comment.Remove(entity);
			await _context.SaveChangesAsync();
			return NoContent();
		}
	}
}

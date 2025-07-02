using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebAPI.Data;
using WebAPI.Dtos.Log;

namespace WebAPI.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]  // require JWT
	public class LogsController : ControllerBase
	{
		private readonly HeritageDbContext _context;
		private readonly IMapper _mapper;

		public LogsController(HeritageDbContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		// GET api/Logs/get/10
		[HttpGet("get/{n}")]
		public async Task<ActionResult<IEnumerable<LogReadDto>>> GetLast(int n)
		{
			var logs = await _context.Log
				.OrderByDescending(l => l.Timestamp)
				.Take(n)
				.ToListAsync();

			return Ok(_mapper.Map<IEnumerable<LogReadDto>>(logs));
		}

		// GET api/Logs/count
		[HttpGet("count")]
		public async Task<ActionResult<int>> Count()
		{
			var total = await _context.Log.CountAsync();
			return Ok(total);
		}
	}
}

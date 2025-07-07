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
    [Authorize] // Require JWT authentication
    public class LogsController : ControllerBase
    {
        private readonly HeritageDbContext _context;
        private readonly IMapper _mapper;

        public LogsController(HeritageDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns the most recent N log entries.
        /// </summary>
        /// <param name="n">Number of logs to retrieve.</param>
        [HttpGet("get/{n}")]
        public async Task<ActionResult<IEnumerable<LogReadDto>>> GetLast(int n)
        {
            var logs = await _context.Log
                .OrderByDescending(l => l.Timestamp)
                .Take(n)
                .ToListAsync();

            var dtos = _mapper.Map<IEnumerable<LogReadDto>>(logs);
            return Ok(dtos);
        }

        /// <summary>
        /// Returns the total number of log entries.
        /// </summary>
        [HttpGet("count")]
        public async Task<ActionResult<int>> Count()
        {
            var total = await _context.Log.CountAsync();
            return Ok(total);
        }
    }
}

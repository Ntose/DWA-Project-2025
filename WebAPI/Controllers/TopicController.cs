using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebAPI.Data;
using WebAPI.Data.Entities;
using WebAPI.Dtos.Topic;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TopicController : ControllerBase
    {
        private readonly HeritageDbContext _context;
        private readonly IMapper _mapper;

        public TopicController(HeritageDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns all topics.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TopicReadDto>>> GetAll()
        {
            var entities = await _context.Topic.ToListAsync();
            var dtos = _mapper.Map<IEnumerable<TopicReadDto>>(entities);
            return Ok(dtos);
        }

        /// <summary>
        /// Returns a specific topic by ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<TopicReadDto>> Get(int id)
        {
            var entity = await _context.Topic.FindAsync(id);
            if (entity == null)
                return NotFound();

            var dto = _mapper.Map<TopicReadDto>(entity);
            return Ok(dto);
        }

        /// <summary>
        /// Creates a new topic.
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<TopicReadDto>> Create([FromBody] TopicCreateDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var entity = _mapper.Map<Topic>(createDto);
            _context.Topic.Add(entity);
            await _context.SaveChangesAsync();

            var readDto = _mapper.Map<TopicReadDto>(entity);
            return CreatedAtAction(nameof(Get), new { id = readDto.Id }, readDto);
        }

        /// <summary>
        /// Updates an existing topic.
        /// </summary>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] TopicCreateDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var entity = await _context.Topic.FindAsync(id);
            if (entity == null)
                return NotFound();

            _mapper.Map(updateDto, entity);
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Deletes a topic by ID.
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.Topic.FindAsync(id);
            if (entity == null)
                return NotFound();

            _context.Topic.Remove(entity);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

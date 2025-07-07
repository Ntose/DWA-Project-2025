using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebAPI.Data;
using WebAPI.Data.Entities;
using WebAPI.Dtos.NationalMinority;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NationalMinorityController : ControllerBase
    {
        private readonly HeritageDbContext _context;
        private readonly IMapper _mapper;

        public NationalMinorityController(HeritageDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns all national minorities.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NationalMinorityReadDto>>> GetAll()
        {
            var entities = await _context.NationalMinority.ToListAsync();
            var dtos = _mapper.Map<IEnumerable<NationalMinorityReadDto>>(entities);
            return Ok(dtos);
        }

        /// <summary>
        /// Returns a specific national minority by ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<NationalMinorityReadDto>> Get(int id)
        {
            var entity = await _context.NationalMinority.FindAsync(id);
            if (entity == null)
                return NotFound();

            var dto = _mapper.Map<NationalMinorityReadDto>(entity);
            return Ok(dto);
        }

        /// <summary>
        /// Creates a new national minority.
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<NationalMinorityReadDto>> Create(
            [FromBody] NationalMinorityCreateDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var entity = _mapper.Map<NationalMinority>(createDto);
            _context.NationalMinority.Add(entity);
            await _context.SaveChangesAsync();

            var readDto = _mapper.Map<NationalMinorityReadDto>(entity);
            return CreatedAtAction(nameof(Get), new { id = readDto.Id }, readDto);
        }

        /// <summary>
        /// Updates an existing national minority.
        /// </summary>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] NationalMinorityCreateDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var entity = await _context.NationalMinority.FindAsync(id);
            if (entity == null)
                return NotFound();

            _mapper.Map(updateDto, entity);
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Deletes a national minority by ID.
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.NationalMinority.FindAsync(id);
            if (entity == null)
                return NotFound();

            _context.NationalMinority.Remove(entity);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

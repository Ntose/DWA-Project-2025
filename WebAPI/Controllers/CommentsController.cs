using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Data;
using WebAPI.Data.Entities;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/CulturalHeritage/{heritageId:int}/Comments")]
    public class CommentsController : ControllerBase
    {
        private readonly HeritageDbContext _db;

        public CommentsController(HeritageDbContext db) => _db = db;

        /// <summary>
        /// Returns approved comments for a specific cultural heritage item.
        /// </summary>
        /// <param name="heritageId">The ID of the cultural heritage item.</param>
        [HttpGet, AllowAnonymous]
        public async Task<ActionResult<List<CommentDto>>> Get(int heritageId)
        {
            if (!await _db.CulturalHeritage.AnyAsync(h => h.Id == heritageId))
                return NotFound();

            var list = await _db.Comments
                .AsNoTracking()
                .Where(c => c.CulturalHeritageId == heritageId && c.Approved)
                .Include(c => c.ApplicationUser)
                .OrderBy(c => c.Timestamp)
                .Select(c => new CommentDto
                {
                    Id = c.Id,
                    Text = c.Text,
                    Timestamp = c.Timestamp,
                    Author = c.ApplicationUser.Username
                })
                .ToListAsync();

            return Ok(list);
        }

        /// <summary>
        /// Submits a new comment for a specific cultural heritage item.
        /// Requires authentication.
        /// </summary>
        /// <param name="heritageId">The ID of the cultural heritage item.</param>
        /// <param name="input">The comment content.</param>
        [HttpPost, Authorize]
        public async Task<ActionResult> Post(int heritageId, [FromBody] CommentCreateDto input)
        {
            if (string.IsNullOrWhiteSpace(input.Text))
                return BadRequest("Comment cannot be empty.");

            if (!await _db.CulturalHeritage.AnyAsync(h => h.Id == heritageId))
                return NotFound();

            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Forbid();

            var comment = new Comment
            {
                CulturalHeritageId = heritageId,
                Text = input.Text,
                Timestamp = DateTime.UtcNow,
                Approved = true, // Auto-approve for now
                UserId = int.Parse(userIdClaim.Value)
            };

            _db.Comments.Add(comment);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { heritageId }, new { comment.Id });
        }
    }

    // DTO for returning comment data
    public class CommentDto
    {
        public int Id { get; set; }
        public string Text { get; set; } = "";
        public DateTime Timestamp { get; set; }
        public string Author { get; set; } = "";
    }

    // DTO for creating a new comment
    public class CommentCreateDto
    {
        public string Text { get; set; } = "";
    }
}

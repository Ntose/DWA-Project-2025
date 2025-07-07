// File: WebAPI/Controllers/UserController.cs

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
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly HeritageDbContext _context;
        public UserController(HeritageDbContext context)
            => _context = context;

        // GET: api/User
        // Returns all users (Admin‐only)
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<UserDto>>> GetAll()
        {
            var users = await _context.ApplicationUser
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    Role = u.Role,
                    DateRegistered = u.DateRegistered
                })
                .ToListAsync();

            return Ok(users);
        }

        // GET: api/User/profile
        // Returns the logged‐in user’s profile plus their comments
        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return Forbid();

            var dto = await _context.ApplicationUser
                .Where(u => u.Username == username)
                .Select(u => new UserProfileDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Phone = u.Phone,
                    DateRegistered = u.DateRegistered,
                    Role = u.Role,
                    Comments = u.Comments
                        .OrderByDescending(c => c.Timestamp)
                        .Select(c => new CommentDto
                        {
                            Id = c.Id,
                            CulturalHeritageId = c.CulturalHeritageId,
                            Text = c.Text,
                            Timestamp = c.Timestamp,
                            Approved = c.Approved
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (dto == null)
                return NotFound();

            return Ok(dto);
        }

        // PUT: api/User/profile
        // Updates the logged‐in user’s profile info
        [HttpPut("profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileInput input)
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return Forbid();

            var user = await _context.ApplicationUser
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
                return NotFound("User not found.");

            user.FirstName = input.FirstName;
            user.LastName = input.LastName;
            user.Email = input.Email;
            user.Phone = input.Phone;

            await _context.SaveChangesAsync();
            return Ok();
        }

        #region DTO Definitions
        public class UpdateProfileInput
        {
            public string FirstName { get; set; } = "";
            public string LastName { get; set; } = "";
            public string Email { get; set; } = "";
            public string Phone { get; set; } = "";
        }

        public class UserDto
        {
            public int Id { get; set; }
            public string Username { get; set; } = "";
            public string Email { get; set; } = "";
            public string Role { get; set; } = "";
            public DateTime DateRegistered { get; set; }
        }

        public class UserProfileDto
        {
            public int Id { get; set; }
            public string Username { get; set; } = "";
            public string Email { get; set; } = "";
            public string FirstName { get; set; } = "";
            public string LastName { get; set; } = "";
            public string Phone { get; set; } = "";
            public DateTime DateRegistered { get; set; }
            public string Role { get; set; } = "";
            public List<CommentDto> Comments { get; set; } = new();
        }

        public class CommentDto
        {
            public int Id { get; set; }
            public int CulturalHeritageId { get; set; }
            public string Text { get; set; } = "";
            public DateTime Timestamp { get; set; }
            public bool Approved { get; set; }
        }

        #endregion
    }
}
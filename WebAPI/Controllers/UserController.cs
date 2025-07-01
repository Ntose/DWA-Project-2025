using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Data;

namespace WebAPI.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class UserController : ControllerBase
	{
		private readonly HeritageDbContext _context;
		public UserController(HeritageDbContext context) => _context = context;

		[HttpGet]
		[Authorize]
		public async Task<IActionResult> GetAll() =>
			Ok(await _context.ApplicationUser
				.Include(u => u.Comments)
				.ToListAsync());

		[HttpGet("profile")]
		[Authorize]
		public async Task<IActionResult> GetProfile()
		{
			var username = User.Identity?.Name;
			var user = await _context.ApplicationUser
				.FirstOrDefaultAsync(u => u.Username == username);

			return user == null ? NotFound() : Ok(user);
		}
	}
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize] // Optionally add: [Authorize(Roles = "Admin")]
	public class LogsController : ControllerBase
	{
		[HttpGet("get/{count}")]
		public IActionResult GetRecentLogs(int count)
		{
			return Ok($"(stub) Returning last {count} logs.");
		}

		[HttpGet("count")]
		public IActionResult GetCount()
		{
			return Ok(new { count = 0 });
		}
	}
}

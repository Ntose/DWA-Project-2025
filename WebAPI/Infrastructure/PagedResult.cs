namespace WebAPI.Infrastructure
{
	public class PagedResult<T>
	{
		public int Page { get; set; }
		public int Count { get; set; }
		public int TotalItems { get; set; }
		public IEnumerable<T> Items { get; set; }
	}

}

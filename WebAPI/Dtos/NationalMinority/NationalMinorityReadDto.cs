namespace WebAPI.Dtos.NationalMinority
{
    /// <summary>
    /// DTO used to send NationalMinority data to clients.
    /// </summary>
    public class NationalMinorityReadDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
    }
}

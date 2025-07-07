namespace WebAPI.Dtos.Topic
{
    /// <summary>
    /// DTO used to send Topic data to clients.
    /// </summary>
    public class TopicReadDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
    }
}

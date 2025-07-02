namespace WebApp.ViewModels
{
    public class CommentVm
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime PostedOn { get; set; }
        public bool IsApproved { get; set; }
        public int HeritageId { get; set; }
        public string HeritageTitle { get; set; }
    }
}

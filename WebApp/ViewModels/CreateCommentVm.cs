using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels
{
    public class CreateCommentVm
    {
        [Required]
        public int HeritageId { get; set; }

        [Required]
        [StringLength(500)]
        public string Content { get; set; }
    }
}

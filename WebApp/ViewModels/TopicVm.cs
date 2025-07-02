using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels
{
    public class TopicVm
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Topic Name")]
        public string Name { get; set; }

        public string Description { get; set; }
    }
}

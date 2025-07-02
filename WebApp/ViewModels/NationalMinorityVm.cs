using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels
{
    public class NationalMinorityVm
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Minority Name")]
        public string Name { get; set; }

        public string Description { get; set; }
    }
}

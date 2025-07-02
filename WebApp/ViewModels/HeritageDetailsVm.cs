using System;
using System.Collections.Generic;
using WebAPI.Data.Entities;

namespace WebApp.ViewModels
{
    public class HeritageDetailsVm
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }

        // Related data
        public string NationalMinorityName { get; set; }
        public List<string> Topics { get; set; } = new();
        public List<CommentVm> Comments { get; set; } = new();
    }
}

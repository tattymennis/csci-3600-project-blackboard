using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using csci_3600_project_the_struggle.Data;
using System.ComponentModel.DataAnnotations;

namespace csci_3600_project_the_struggle.Models
{
    // Custom model for _PostPartial. Carries its background image
    // to be rendered in View and a list of BoardPosts (update from DB).
    
    public class Blackboard
    {
        [Key]
        public int Id { get; set; } // should not need this, EF wanted it
        
        public string Image = "~/Content/img/blackboard.jpg";
        public List<BoardPost> BoardPosts { get; set; }
    }
}
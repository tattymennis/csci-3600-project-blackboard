using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace csci_3600_project_the_struggle.Models
{
    public class BlackboardViewModel
    {
        [Key]
        public string Id { get; set; } // logged in users should have extra options in the View
        public Blackboard Blackboard { get; set; } // Model for Board containing BoardPosts, also in Partial
        public DateTime PostTime { get; set; }

        [Required]
        [DisplayName("Handle")]
        [StringLength(24, ErrorMessage = "User handle must be between 1-24 characters long"), MinLength(1)]
        public string UserHandle { get; set; }

        [Required]
        [StringLength(256)]
        public string Message { get; set; }

        //[Required]
        [DisplayName("Encryption Algorithm")]
        public IEnumerable<SelectListItem> Encryption{ get; set; } // will become a type connected to a drop down box
        public string EncryptionChoice { get; set; }

        [Required]
        [MinLength(8)]
        [DisplayName("Key")]
        public string EncryptionKey { get; set; }
    }

}
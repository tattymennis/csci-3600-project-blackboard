using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace csci_3600_project_the_struggle.Data
{
    public class BoardPost
    {
        [Key]
        public int Id { get; set; }
        //[Required]
        //public AspNetUser Poster { get; set; }
        // Probably don't need type AspNetUser for Poster field of a post.

        [Required]
        public string Poster { get; set; }

        [Required]
        public DateTime PostTime { get; set; }

        [Required]
        public string Content { get; set; }

        public int TimeToLive { get; set; }

        public string EncryptionAlgorithm { get; set; }

        public byte[] Salt { get; set; }
        public byte[] IV { get; set; }
    }
}
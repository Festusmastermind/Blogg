using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Blogg.Models
{
    public class Post
    {
        //creating the post table columns ..
        //id, title, body, created_at, image, userid.
        public int ID { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }
        [Required]
        [StringLength(1000)]

        [Display(Name = "Content")]
        public string Body { get; set; }

        [DataType(DataType.Date)]
        [Display(Name ="Created At")]
        public DateTime Created { get; set; }
        public string Imagepath { get; set; }

        public string ApplicationUserId { get; set; }
        public ApplicationUser Creator{ get; set; } //reference navigation property



    }

}

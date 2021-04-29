using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blogg.Models
{
    //this inherits all the properties of IdentityUser class
    public class ApplicationUser: IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public ICollection<Post> Posts { get; set; }  //ICollection Navigation Properpty
    }
}

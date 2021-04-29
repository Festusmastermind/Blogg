using Blogg.Data;
using Blogg.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blogg.Controllers
{
    public class ProfileController : Controller
    {

        private readonly BlogDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<Post> _logger;

        //dependecy injection i.e injecting the access to my tables in database via constructor.
        public ProfileController(BlogDbContext context, UserManager<ApplicationUser> userManager,
            ILogger<Post> logger)
        {
            _userManager = userManager;
            _logger = logger;
            _context = context; 
        }

        [HttpGet]
        public IActionResult Edit()
        {
            //we are going to current user details
            return View();
        }
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]

        public IActionResult UpdateCurrentUser()
        {
            return View();

        }






    }
}

using Blogg.Data;
using Blogg.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Blogg.Controllers
{
    public class PostController : Controller
    {
        private readonly BlogDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
      //  private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<Post> _logger;
     
        
        public PostController(BlogDbContext context, UserManager<ApplicationUser> userManager,
            ILogger<Post> logger)
        {
            _userManager = userManager;
            _logger = logger;
            _context = context; //dependecy injection i.e injecting the access to my tables in database via constructor.
        }
        [AllowAnonymous]
        public async Task<IActionResult> Index(string searchString)
        {
            var posts = from p in _context.Posts
                        select p;
            //posts.Include(p => p.Creator);
            if (!String.IsNullOrEmpty(searchString))
            {
                posts = posts.Where(p => p.Title.Contains(searchString) || p.Body.Contains(searchString));
            }
            return View(await posts.Include(p => p.Creator).OrderByDescending(p => p.Created).ToListAsync());
        }

        public IActionResult Dashboard()
        {
            //This resource returns the ICollection instance of posts of the logged in user..
            var userid = _userManager.GetUserId(User);
            var myPosts = _context.Posts.Where(p=> p.ApplicationUserId == userid);
            return View(myPosts);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title, Body, Created")] Post postmodel)
        {
            //This resource will save the data passed into the post table storing with the userid..
            var currentUserId = _userManager.GetUserId(User);
          //var userid = _context.Posts.FirstOrDefaultAsync(p => p.ApplicationUserId == currentUserId);
            try
            {
                if (ModelState.IsValid)
                {
                    Post post = new Post
                    {
                        Title = postmodel.Title,
                        Body = postmodel.Body,
                        Created = postmodel.Created,
                        ApplicationUserId = currentUserId
                    };
                    _context.Add(post); //this will add the data passed into addchanges state and its save at the next line.
                    await _context.SaveChangesAsync();
                    TempData["success"] = "Post Added Successfully ";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException)
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. ");
            }
            return View(postmodel);
        }
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
         
            var  singlePost = await _context.Posts.Include(c => c.Creator)
                .SingleOrDefaultAsync(s => s.ID == id);
            if (singlePost != null)
            {
                return View(singlePost);
            }
            else
            {
                return NotFound();
            }
           
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var singlePost = await _context.Posts.SingleOrDefaultAsync(p => p.ID == id);

            var sessionuser = _userManager.GetUserId(User); //get the logged-in userid
            if ((singlePost == null) || (sessionuser != singlePost.ApplicationUserId))
            {
                _logger.LogError("This post can't be edit nigga");
                TempData["ErrorMessage"] = "You don't have access to Edit this post";
                return RedirectToAction(nameof(Index));
            }
            return View(singlePost);
        }
        //This Post Method is more secore ..
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            //get the data from the database and try to update the specifed fields provided via lambda
            var postToUpdate = await _context.Posts.FirstOrDefaultAsync(p => p.ID == id);
            if (await TryUpdateModelAsync<Post>(postToUpdate, "", p => p.Title, p=>p.Body, p=>p.Created))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    TempData["success"] = "Post Updated Successfully";
                    //i can send a count message to this page to know if its an error message or not..
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)  //catches any dBexception and logs it..
                {
                    _logger.LogError("There is an error in updating this data" + ex);
                    ModelState.AddModelError(string.Empty, "There is a problem with this data, Check your data");
                }
            }
            return View(postToUpdate);
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var sessionuser = _userManager.GetUserId(User);
            var post = await _context.Posts.FirstOrDefaultAsync(p => p.ID == id);
            //check for the owner of the post
            var postitself = post.ApplicationUserId;

            _logger.LogInformation("Postowner" + sessionuser + " " + postitself);
            if((post == null) || (sessionuser != post.ApplicationUserId))
            {
                _logger.LogError("This post can't be hacked nigga");
                TempData["ErrorMessage"] = "You don't have access to delete this post";
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(p => p.ID == id);  //retrieve the details of the id specified
            var postname = post.Title;
            _context.Remove(post);
            await _context.SaveChangesAsync();
            TempData["success"] = postname + "Post is Deleted Succesfully";
            return RedirectToAction(nameof(Index));
        }

        //It can come in add to check for posts presence..it return either true or false.
        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.ID == id);
        }




    }


}

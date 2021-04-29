using Blogg.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Blogg.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        [TempData]
        public string StatusMessage { get; set; }
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        public IActionResult About()
        {
            //ViewData and ViewBag is both used to pass data from the controller to the view 
            //ViewBag is of dynamic type and ViewDatab is of String type..
            ViewData["aboutus"] = "There is nothing really much about us";
            ViewBag.Learn = "Learn Asp.net core by buidling solutions.";
            return View();
        }
        [AllowAnonymous]
        public IActionResult Services()
        {
            //passing data to the view using the dictionary types...ViewData and ViewBag
            //ViewBag is best suitable for this tasks
            ViewBag.Web = "Web Application Development";
            ViewBag.Mobile = "Mobile Application Development";
            ViewBag.Desktop = "Desktop Application Development";
            ViewBag.Project = "Project Deliveries";
            ViewBag.Sofware = "Software as a Services Development ";
            ViewBag.Blockchain = "BlockChain Integration ";
            ViewBag.Payment = "Payment Intergration of any Kind";
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

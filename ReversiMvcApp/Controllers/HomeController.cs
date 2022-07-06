using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ReversiMvcApp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ReversiMvcApp.Repositories;
using ReversiMvcApp.Data;

namespace ReversiMvcApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISpelData _spelData;
        private readonly ReversiDbContext _context;

        public HomeController(ILogger<HomeController> logger, ISpelData spelData, ReversiDbContext context)
        {
            _logger = logger;
            _spelData = spelData;
            _context = context;
        }

        public IActionResult Index()
        {
            Speler speler = null;
            ClaimsPrincipal currUser = this.User;
            if (currUser?.FindFirst(ClaimTypes.NameIdentifier)?.Value != null)
            {
                var currentPlayerToken = currUser.FindFirst(ClaimTypes.NameIdentifier).Value;
                ViewData["spelerId"] = currentPlayerToken;
                ViewData["spelId"] = _spelData.GetSpelBySpelerId(currentPlayerToken)?.Result?.Token;
                speler = _context.Spelers.FirstOrDefault(s => s.Guid == currentPlayerToken);
            }

            if (speler != null) return View(speler);
                    return View();
        }
        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

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
using Microsoft.EntityFrameworkCore;

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

        public async Task<IActionResult> IndexAsync()
        {
            Speler speler = null;
            ClaimsPrincipal currUser = this.User;
            if (currUser?.FindFirst(ClaimTypes.NameIdentifier)?.Value != null)
            {
                var currentPlayerToken = currUser.FindFirst(ClaimTypes.NameIdentifier).Value;
                Spel spel = _spelData.GetSpelBySpelerId(currentPlayerToken)?.Result;
                if (spel != null && spel.Speler1Token != null && spel.Speler2Token != null)
                {
                    string speler2 = spel.Speler1Token != currentPlayerToken ? spel.Speler1Token : spel.Speler2Token;

                    await CalculatePoints(currentPlayerToken, speler2);
                    _spelData.GeefOp(spel.Token);
                }


                ViewData["spelerId"] = currentPlayerToken;
                ViewData["spelId"] = spel?.Token;
                speler = _context.Spelers.FirstOrDefault(s => s.Guid == currentPlayerToken);
            }

            if (speler != null) return View(speler);
                    return View();
        }

        public async Task CalculatePoints(string spelerToken, string speler2)
        {
            //check if speler has played more games
            if (await _context?.Spelers?.FirstOrDefaultAsync(s => s.Guid == spelerToken) == null)
            {
                Speler speler = new Speler
                {
                    Guid = spelerToken,
                    Naam = this.User.Identity.Name,
                    AantalVerloren = 1
                };

                _context.Spelers.Add(speler);
            }
            else
            {
                Speler speler = await _context?.Spelers?.FirstOrDefaultAsync(s => s.Guid == spelerToken);
                speler.AantalVerloren++;
                _context.Spelers.Update(speler);
            }
            if (await _context?.Spelers?.FirstOrDefaultAsync(s => s.Guid == speler2) == null)
            {
                Speler speler = new Speler
                {
                    Guid = spelerToken,
                    Naam = "TestUser",
                    AantalGewonnen = 1
                };

                _context.Spelers.Add(speler);
            }
            else
            {
                Speler speler = await _context?.Spelers?.FirstOrDefaultAsync(s => s.Guid == speler2);
                speler.AantalGewonnen++;
                _context.Spelers.Update(speler);
            }
            _context.SaveChanges();
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

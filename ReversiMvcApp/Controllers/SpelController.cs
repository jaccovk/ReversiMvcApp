using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReversiMvcApp.Data;
using ReversiMvcApp.Models;
using ReversiMvcApp.Repositories;

namespace ReversiMvcApp.Controllers
{
    public class SpelController : Controller
    {
        private readonly ReversiDbContext _context;
        private ISpelData _spelData;

        public SpelController(ReversiDbContext context, ISpelData data)
        {
            _context = context;
            _spelData = data;
        }


        //give up
        public async Task<IActionResult> GeefOp(string spelerToken)
        { 
            
             //remove spel will happen in Home/Index
            return RedirectToAction("Index", "Home");
        }

        // GET: Spel
        [Authorize]
        public async Task<IActionResult> Index()
        {
            if (await _spelData.GetSpellen() != null)
                return View(await _spelData.GetSpellen());
            return View();
        }

        // GET: Spel/SpelBord/5
        [Authorize]
        public async Task<IActionResult> SpelBord()
        {
            ClaimsPrincipal currUser = this.User;
            var currentPlayerToken = currUser.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (await _spelData.GetSpelBySpelerId(currentPlayerToken) != null)
            {
                var spel = await _spelData.GetSpelBySpelerId(currentPlayerToken);

                ViewData["spelerId"] = currentPlayerToken;
                ViewData["spelId"] = _spelData.GetSpelBySpelerId(currentPlayerToken).Result.Token;

                if (spel == null)
                {
                    ViewBag.Message = "Er is iets fout gegaan. Probeer het opnieuw. \n (Je kunt niet je eigen spel joinen.)";
                    return RedirectToAction("Index");
                }


                return View(spel);
            }
            return RedirectToAction("Index", "Spel");
        }

        //PUT:spel/
        public async Task<IActionResult> NeemDeelAanSpel(string token)
        {
            ClaimsPrincipal currPlayer = this.User;
            var currPlayerToken = currPlayer.FindFirst(ClaimTypes.NameIdentifier).Value;

            await _spelData.NeemDeelAanSpel(token, currPlayerToken);

            return RedirectToAction("SpelBord", "Spel");
        }
        public IActionResult JoinEigenSpel(string token)
        {
            return RedirectToAction("SpelBord", "Spel");
        }



        // GET: Spel/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Spel/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Omschrijving")] Spel spel)
        {
            if (ModelState.IsValid)
            {
                //ga pas verder als de gebruiker is ingelogd
                if (SecurityCheck.LoginCheck(this.User))
                {
                    //encrypt de spelertoken
                    spel.Speler1Token = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                    //voeg het nieuwe spel toe en wacht op reactie
                    string check = await _spelData.AddSpel(spel);


                    if (check == "ok")
                    {
                        return RedirectToAction("SpelBord");
                    }
                    if (check == "noOmschrijving")
                        ViewBag.Message = "Vul een onderwerp in";
                    if (check == "onlyOneGame")
                        ViewBag.Message = "Je kunt maar één game tegelijk starten!";
                    if (check == "somethingWrong")
                        ViewBag.Message = "Er is iets fout gegaan. Probeer opnieuw";
                    return View();
                }
                ViewBag.Message = "Log eerst in!";
                return View();

            }
            return View();
        }
    }
}

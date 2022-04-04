using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Internal.Account;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using Newtonsoft.Json.Serialization;
using ReversiMvcApp.Data;
using ReversiMvcApp.Models;
using ReversiMvcApp.Repositories;

namespace ReversiMvcApp.Controllers
{
    public class SpelController : Controller
    {
        private readonly ReversiDbContext _context;
        private ISpelData Data;

        public SpelController(ReversiDbContext context, ISpelData data)
        {
            _context = context;
            Data = data;
        }

        // GET: Spel
        public async Task<IActionResult> Index()
        {
            if (await Data.GetSpellen() != null)
                return View(await Data.GetSpellen()); 
            return BadRequest();
        }

        // GET: Spel/Details/5
        [Authorize]
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var spel = await Data.GetSpelDetails(id, User);
            if (spel == null)
            {
                ViewBag.Message = "Er is iets fout gegaan. Probeer het opnieuw. \n (Je kunt niet je eigen spel joinen.)";
                return RedirectToAction("Index");
            }
            return View(spel);
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
                    string check = await Data.AddSpel(spel);
                    if (check == "ok")
                        return RedirectToAction("Details");
                    if(check == "noOmschrijving")
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

        // GET: Spel/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var spel = await _context.Spel.FindAsync(id);
            if (spel == null)
            {
                return NotFound();
            }
            return View(spel);
        }

        // POST: Spel/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Token,Speler1Token,Speler2Token,Omschrijving,Winnaar,Afgelopen,AandeBeurt")] Spel spel)
        {
            if (id != spel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(spel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SpelExists(spel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(spel);
        }

        // GET: Spel/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var spel = await _context.Spel
                .FirstOrDefaultAsync(m => m.Id == id);
            if (spel == null)
            {
                return NotFound();
            }

            return View(spel);
        }

        // POST: Spel/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var spel = await _context.Spel.FindAsync(id);
            _context.Spel.Remove(spel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SpelExists(int id)
        {
            return _context.Spel.Any(e => e.Id == id);
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReversiMvcApp.Data;
using ReversiMvcApp.Models;
using ReversiMvcApp.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReversiMvcApp.Controllers
{
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ReversiDbContext _context;
        private readonly ISpelData _spelData;

        public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ISpelData spelData, ReversiDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _spelData = spelData;
        }

        [Authorize(Roles = "Beheerder")]
        public async Task<IActionResult> RolesAsync()
        {
            var userRoles = new List<RoleManager>();

            // get all users and roles
            var users = _userManager.Users;
            var roles = _roleManager.Roles;

            // loop through users and add them to the list
            foreach (var user in users)
            {
                foreach (var role in roles)
                {
                    if (await _userManager.IsInRoleAsync(user, role.Name))
                    {
                        var userRole = new RoleManager
                        {
                            UserName = user.UserName,
                            Role = role.Name
                        };

                        userRoles.Add(userRole);
                    }
                }
            }
            return View(userRoles);
        }

        [Authorize(Roles = "Beheerder")]
        [HttpPost]
        public IActionResult ChangeRole(string userName, string role)
        {
            var user = _userManager.FindByNameAsync(userName).Result;
            var userRole = _userManager.GetRolesAsync(user).Result;

            if (userRole.Count > 0)
            {
                _userManager.RemoveFromRoleAsync(user, userRole[0]).Wait();
            }

            _userManager.AddToRoleAsync(user, role).Wait();

            return RedirectToAction("Roles");
        }

        [Authorize(Roles = "Beheerder,Mediator")]
        public IActionResult Delete()
        {
            var users = _userManager.Users;
            var spelers = new List<IdentityUser>();

            // find all users that has the role of speler
            foreach (var user in users)
            {
                if (_userManager.IsInRoleAsync(user, "Speler").Result)
                {
                    spelers.Add(user);
                }
            }

            if (spelers.Count == 0)
            {
                return View();
            }

            return View(spelers);
        }

        [Authorize(Roles = "Beheerder,Mediator")]
        [HttpPost]
        public async Task<IActionResult> DeleteSpelerAsync(string userName)
        {
            // get token from id
            var user = _userManager.FindByNameAsync(userName).Result;
            var currPlayerToken = user.Id;

            // add a point to the opponent
            Spel spel = _spelData.GetSpelBySpelerId(currPlayerToken)?.Result;
            string speler2token = spel.Speler1Token != currPlayerToken ? spel.Speler1Token : spel.Speler2Token;

            Speler speler2 = await _context?.Spelers?.FirstOrDefaultAsync(s => s.Guid == speler2token);
            speler2.AantalGewonnen++;
            _context.Spelers.Update(speler2);
            _context.SaveChanges();

            spel.Afgelopen = true;
            _spelData.UpdateSpelAfgelopen(spel.Token, spel).Wait();

            /*            if (_spelData.RemoveSpelAsync(currPlayerToken) != null)
                        {
                            _context.SaveChanges();
                        }*/

            Speler speler = await _context?.Spelers?.FirstOrDefaultAsync(s => s.Guid == currPlayerToken);
            _context.Spelers.Remove(speler);

            //delete Identity user
            _userManager.DeleteAsync(user).Wait();

            return RedirectToAction("Delete");
        }
    }
}

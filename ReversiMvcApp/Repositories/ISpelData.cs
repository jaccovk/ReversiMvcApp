using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ReversiMvcApp.Models;

namespace ReversiMvcApp.Repositories
{
    public interface ISpelData
    {
        public Task<List<Spel>> GetSpellen();
        public Task<List<Spel>> GetSpellenBySpelerToken(string spelerToken);
        public Task<string> AddSpel(Spel spel);
        public Task<bool> UpdateSpelAfgelopen(string spelToken, Spel spel);
        public Task<Spel> GetSpelDetails(string? id, ClaimsPrincipal user);
        public Task<Spel> GetSpelBySpelerId(string spelerId);
        public void GeefOp(string spelerToken);
        public Task<Spel> NeemDeelAanSpel(string token, string currPlayerToken);
        public Task<bool> RemoveSpelAsync(string userName);

    }
}

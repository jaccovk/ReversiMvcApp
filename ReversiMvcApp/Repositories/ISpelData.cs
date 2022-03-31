using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ReversiMvcApp.Models;

namespace ReversiMvcApp.Repositories
{
    public interface ISpelData
    {
        public Task<List<Spel>> GetSpellen();
        public Task AddSpel(Spel spel);
        public Task<Spel> GetSpelDetails(int? id);

    }
}

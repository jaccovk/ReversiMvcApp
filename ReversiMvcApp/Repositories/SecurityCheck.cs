using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Policy;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ReversiMvcApp.Models;

namespace ReversiMvcApp.Repositories
{
    public static class SecurityCheck
    {
        public static bool LoginCheck(ClaimsPrincipal user)
        {
            if (user?.FindFirst(ClaimTypes.NameIdentifier)?.Value != null)
                return true;
            return false;
        }

        public static async Task<bool> OnlyOneGameOpen(string spelerToken, HttpClient client, string url)
        {
            //lijst met spellen opvragen
            HttpResponseMessage response = await client.GetAsync($"{url}/getSpellen");
            List<Spel> spellen = JsonConvert.DeserializeObject<List<Spel>>(await response.Content.ReadAsStringAsync())?.ToList();
            if (response.IsSuccessStatusCode)
            {
                foreach (Spel spel in spellen)
                {
                    //als er een spel is dat het opgegeven spelertoken bevat, return dan false.
                    if (spel.Speler1Token != spelerToken || spel.Speler2Token != spelerToken)
                        return false;
                }
            }
            return true;
        }
    }
}

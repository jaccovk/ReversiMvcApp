using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using ReversiMvcApp.Data;
using ReversiMvcApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ReversiMvcApp.Repositories
{
    public class SpelData : ISpelData
    {
        readonly ReversiDbContext _context;
        private readonly HttpClient _client = new();
        private string Url => "https://localhost:44326/api/Spel";

        public async Task<List<Spel>> GetSpellen()
        {
            try
            {
                HttpResponseMessage response = await _client.GetAsync($"{Url}/getSpellen");//Url}");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(responseBody);
                return JsonConvert.DeserializeObject<List<Spel>>(responseBody);
                

            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                return null;
            }
        }

        public async Task AddSpel(Spel spel)
        {
            //Voeg de encrypte spelertoken toe aan de FromHeader van de api d.m.v. HttpClient
            _client.DefaultRequestHeaders.Add("x-spelertoken", spel.Speler1Token);

            //encodeer de omschrijving en zet deze om in een StringContent. wordt gebruikt door HttpClient ipv string 
            var content = new StringContent($"\"{spel.Omschrijving}\"", Encoding.UTF8, "application/json");

            //maak een nieuw response message aan en wacht tot de httpClient de post request heeft uitgevoerd en een OK message heeft teruggekregen.
            HttpResponseMessage response = await _client.PostAsync($"{Url}/createGame",content);

            //controleer of de client een success heeft gereturnd. 
            response.EnsureSuccessStatusCode();
        }

        public async Task<Spel> GetSpelDetails(int? id)
        {
            HttpResponseMessage response = await _client.GetAsync($"{Url}/{id}");
        }
    }
}

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
        private readonly HttpClient _client = new();
        private string Url => "https://localhost:44326/api/Spel";
        public async Task<List<Spel>> GetSpellen()
        {
            try
            {
                HttpResponseMessage response = await _client.GetAsync($"{Url}/getWachtendeSpellen");
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
        public async Task<List<Spel>> GetSpellenBySpelerToken(string spelerToken)
        {
            try
            {
                _client.DefaultRequestHeaders.Add("x-spelertoken", spelerToken);
                HttpResponseMessage response = await _client.GetAsync($"{Url}/getSpellenBySpelerToken");
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
        

        public async Task<Spel> NeemDeelAanSpel(string token, string currPlayerToken)
        {
            var neemDeel = new
            {
                SpelToken = token,
                SpelerToken = currPlayerToken
            };
            StringContent stringContent = new StringContent(JsonConvert.SerializeObject(neemDeel), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync($"{Url}/neemDeelAanSpel", stringContent);
            string responseBody = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Spel>(responseBody);
        }
        public async Task<Spel> GetSpelBySpelerId(string spelerId)
        {
            HttpResponseMessage response = await _client.GetAsync($"{Url}/getSpelBySpelerId/{spelerId}");
            string responseBody = await response.Content.ReadAsStringAsync();
            Debug.WriteLine($"response: {responseBody}");
            return JsonConvert.DeserializeObject<Spel>(responseBody);
        }

        public async Task<string> AddSpel(Spel spel)
        {

            //check of de speler een spel open heeft staan.
            if (await SecurityCheck.OnlyOneGameOpen(spel.Speler1Token, _client, Url) == false)
            {
                return "onlyOneGame";
            }

            //Voeg de encrypte spelertoken toe aan de FromHeader van de api d.m.v. HttpClient
            _client.DefaultRequestHeaders.Add("x-spelertoken", spel.Speler1Token);

            //controle of er een omschrijving ingevuld is(in principe vangt het html script dit op, maar soms gaat het fout. dit is een extra controle
            if (spel.Omschrijving == null) return "noOmschrijving";

            //encodeer de omschrijving en zet deze om in een StringContent. wordt gebruikt door HttpClient ipv string 
            var content = new StringContent($"\"{spel.Omschrijving}\"", Encoding.UTF8, "application/json");

            //maak een nieuw response message aan en wacht tot de httpClient de post request heeft uitgevoerd en een OK message heeft teruggekregen.
            HttpResponseMessage response = await _client.PostAsync($"{Url}/createGame", content);
            
            //controleer of de client een success heeft gereturnd. 
            if (response.IsSuccessStatusCode) return "ok";
            return "somethingWrong";
        }

        public async Task<bool> UpdateSpelAfgelopen(string spelToken, Spel spel)
        {
            _client.DefaultRequestHeaders.Add("x-speltoken", spelToken);

            StringContent stringContent = new StringContent(JsonConvert.SerializeObject(spel), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _client.PutAsync($"{Url}/updateSpelAfgelopen", stringContent);

            return response.IsSuccessStatusCode;
        }

        public async Task<Spel> GetSpelDetails(string? id, ClaimsPrincipal user)
        {
            //verstuur de encrypte spelertoken naar de header 
            _client.DefaultRequestHeaders.Add("x-speltoken", id);

            //vraag het spel op d.m.v. het opgegeven speltoken en controleer of deze niet null is
            HttpResponseMessage response = await _client.GetAsync($"{Url}/getSpel");
            if (!response.IsSuccessStatusCode) return null;

            //zet de gereturnde json object om in Spel en wacht op response. wanneer spel null is, return null.
            Spel spel = JsonConvert.DeserializeObject<Spel>(
                await response.Content.ReadAsStringAsync());

            if (spel == null) return null;

            //vraag de user id op en encrypt deze. return vervolgens spel
            string currentUserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (spel.Speler1Token == currentUserId) return null;
            spel.Speler2Token = currentUserId;
            return spel;
        }
        
        public async Task<bool> RemoveSpelAsync(string spelerToken)
        {
            _client.DefaultRequestHeaders.Add("x-spelertoken", spelerToken);
            var response = await _client.DeleteAsync($"{Url}/removeSpel");
            return response.IsSuccessStatusCode;
        }

        public void GeefOp(string spelerToken)
        {
            //Voeg de encrypte spelertoken toe aan de FromHeader van de api d.m.v. HttpClient
            _client.DefaultRequestHeaders.Add("x-spelertoken", spelerToken);

            //verzend de request naar de api
            _client.PutAsync($"{Url}/geefOp", null);
        }
    }
}

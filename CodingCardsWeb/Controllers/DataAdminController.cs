using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CodingCards.Data;
using CodingCards.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CodingCards.Controllers
{
    public class DataAdminController : Controller
    {
        private readonly ICardRepository _cardRepository;
        private UserManager<ApplicationUser> _userManager;

        public DataAdminController(ICardRepository context, UserManager<ApplicationUser> userManager)
        {
            _cardRepository = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> IndexIntoElastic()
        {

            for (int i = 2; i < 1740; i++)
            {
                var item = await _cardRepository.GetCardAsync(i);
                if (item != null)
                {
                    var json = JsonConvert.SerializeObject(item, Formatting.None,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });
                    var data = new StringContent(json, Encoding.UTF8, "application/json");

                    var client = new HttpClient();

                    var response = await client.PutAsync("http://a-main-elastic.experimentsinthedeep.com" + "/cards/_doc/" + item.id, data);
                }

            }


            return RedirectToAction("Index", "Home");
        }
    }
}
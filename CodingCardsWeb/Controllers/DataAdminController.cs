using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CodingCards.Data;
using CodingCards.Models;
using CodingCards.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CodingCards.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DataAdminController : Controller
    {
        private readonly ICardRepository _cardRepository;
        private UserManager<ApplicationUser> _userManager;
        private readonly ElasticService _es;

        public DataAdminController(ICardRepository context, 
            UserManager<ApplicationUser> userManager,
            ElasticService es)
        {
            _cardRepository = context;
            _userManager = userManager;
            _es = es;
        }

        public async Task<IActionResult> IndexIntoElastic()
        {
            var items = await _cardRepository.GetCardsAllAsync();

            foreach (var card in items)
            {
                await _es.AddCardToES(card);
            }
           

            return RedirectToAction("Index", "Home");
        }
    }
}
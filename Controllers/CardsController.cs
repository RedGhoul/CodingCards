using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CodingCards.Data;
using CodingCards.Models;
using CodingCards.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Microsoft.Extensions.Caching.Distributed;
using System.Data.SQLite;

namespace CodingCards.Controllers
{
    [Authorize]
    public class CardsController : Controller
    {
        private readonly ICardRepository _cardRepository;
        private readonly IDistributedCache _cache;
        private readonly ApplicationDbContext _ctx;
        public CardsController(ApplicationDbContext context,ICardRepository cardRepo, IDistributedCache cache)
        {
            _ctx = context;
            _cardRepository = cardRepo;
            _cache = cache;
        }

        [HttpPost]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index(CardIndexViewModel cardIndexViewModel)
        {
            CardIndexViewModel cardIndexvm = cardIndexViewModel;

            if (cardIndexViewModel.Page == 0)
            {
                cardIndexvm = CardHelpers.SetDefaultFindModel(cardIndexvm);
            }

            CardHelpers.SetupViewBag(cardIndexvm, ViewBag);

            var result = await _cardRepository.ConfigureSearchAsync(cardIndexvm);
            var count = await _cardRepository.GetTotalCards();

            ViewBag.MaxPage = count / cardIndexvm.Page;

            ViewBag.Page = cardIndexvm.Page;
            cardIndexvm.Cards = result;
            return View(cardIndexvm);
        }

        public async Task<IActionResult> ViewCard(int id)
        {
            var result = await _cardRepository.GetCardAsync(id);
            return View(result);
        }

        public async Task<IActionResult> GetRandomCard()
        {
            var result = await _cardRepository.GetRandomCardAsync();
            return View("ViewCard",result);
        }

        // GET: Cards/Create
        public IActionResult CreateTextCard()
        {
            CardCreateViewModel model = new CardCreateViewModel();
            model.Card_Type = Enum.GetValues(typeof(CardType)).Cast<CardType>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = ((int)v).ToString()
            }).ToList();
            return View(model);
        }


        // GET: Cards/Create
        public IActionResult CreateCodeCard()
        {
            CardCreateViewModel model = new CardCreateViewModel();
            model.Card_Type = Enum.GetValues(typeof(CardType)).Cast<CardType>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = ((int)v).ToString()
            }).ToList();
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTextCard(Card card)
        {
            if (ModelState.IsValid)
            {
                await _cardRepository.SaveCard(card, HttpContext.User);
                return RedirectToAction(nameof(Index));
            }
            return View(card);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCodeCard(Card card)
        {
            if (ModelState.IsValid)
            {
                card.Type = CardType.Code;
                await _cardRepository.SaveCard(card, HttpContext.User);
                return RedirectToAction(nameof(Index));
            }
            return View(card);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            var card = await _cardRepository.GetCardAsync(id);

            if (card == null)
            {
                return NotFound();
            }
            return View(card);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("id,Name,Question,Answer")] Card card)
        {
            if (id != card.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _cardRepository.UpdateCard(card);
                }
                catch (DbUpdateConcurrencyException)
                {
                    
                }
                return RedirectToAction(nameof(Index));
            }
            return View(card);
        }

        // GET: Cards/Delete/5
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var card = await _cardRepository.GetCardAsync(id);

            if (card == null)
            {
                return NotFound();
            }

            return View(card);
        }

        // POST: Cards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _cardRepository.DeleteConfirmedAsync(id);
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> UsersCards()
        {
            UserCardViewModel vm = new UserCardViewModel
            {
                UserCards = await _cardRepository.GetUserCards(HttpContext.User)
            };
            return View(vm);
        }
        
       
    }
}

using System;
using System.Collections.Generic;
using System.Data;
//using System.Data.SQLite;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CodingCards.Data;
using CodingCards.Helpers;
using CodingCards.Models;
using CodingCards.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace CodingCards.Controllers
{

    public class CardsController : Controller
    {
        private readonly ICardRepository _cardRepository;
        private readonly string SearchVMCacheKey = "SearchVMCacheKey";

        public CardsController(ICardRepository context)
        {
            _cardRepository = context;
        }


        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var value = HttpContext.Session.GetString(SearchVMCacheKey);
            CardIndexViewModel cardIndexViewModel = string.IsNullOrEmpty(value) ?
                CardHelpers.SetDefaultFindModel(new CardIndexViewModel()) :
                JsonConvert.DeserializeObject<CardIndexViewModel>(value);

            CardHelpers.SetupViewBag(cardIndexViewModel, ViewBag);

            var result = await _cardRepository.ConfigureSearchAsync(cardIndexViewModel);
            var count = await _cardRepository.GetTotalCards();

            ViewBag.MaxPage = count / cardIndexViewModel.Page;

            ViewBag.Page = cardIndexViewModel.Page;
            cardIndexViewModel.Cards = result;
            return View(cardIndexViewModel);
        }



        [AllowAnonymous]
        [HttpPost]
        public IActionResult IndexPost(CardIndexViewModel homeIndexVm)
        {
            homeIndexVm = CardHelpers.SetDefaultFindModel(homeIndexVm);

            CardHelpers.SetupViewBag(homeIndexVm, ViewBag);

            var vmData = JsonConvert.SerializeObject(homeIndexVm);
            HttpContext.Session.SetString(SearchVMCacheKey, vmData);

            return RedirectToAction("Index");
        }

        [AllowAnonymous]
        public async Task<IActionResult> ViewCard(int id)
        {
            var result = await _cardRepository.GetCardAsync(id);
            return View(result);
        }

        [AllowAnonymous]
        public async Task<IActionResult> GetRandomCard()
        {
            var result = await _cardRepository.GetRandomCardAsync();
            return View("ViewCard",result);
        }
        [AllowAnonymous]
        public async Task<IActionResult> GetAnswer(int? id)
        {
            var card = await _cardRepository.GetCardAsync(id);

            if(card == null)
            {
                return NotFound();
            }
            return View(card);
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
        [Authorize]
        public async Task<IActionResult> UsersCards()
        {
            var userCards = await _cardRepository.GetUserCards(HttpContext.User);
            return View(userCards);
        }


    }
}

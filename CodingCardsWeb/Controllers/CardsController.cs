using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CodingCards.Data;
using CodingCards.Helpers;
using CodingCards.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Syncfusion.EJ2.Grids;
using Microsoft.AspNetCore.Identity;

namespace CodingCards.Controllers
{
    [Authorize]
    public class CardsController : Controller
    {
        private readonly ICardRepository _cardRepository;
        private UserManager<ApplicationUser> _userManager;

        public CardsController(ICardRepository context, UserManager<ApplicationUser> userManager)
        {
            _cardRepository = context;
            _userManager = userManager;
        }

        // GET: Cards
        public async Task<IActionResult> HomeCards()
        {
            //await Helper.GetDataFromSQLite(_context);
            var value = await _cardRepository.GetRandomSetOfCardsAsync(100);
            ViewBag.datasource = value.ToArray();
            ViewBag.NumberOfEntrys = 100;
            ViewBag.TotalCards = await _cardRepository.GetTotalCards();
            return View();
        }
        public async Task<IActionResult> ViewAllCards()
        {
            //await Helper.GetDataFromSQLite(_context);
            ViewBag.datasource = await _cardRepository.GetCardsAllAsync();
            return View();
        }

        public async Task<IActionResult> GetCard()
        {
            var result = await _cardRepository.GetRandomCardAsync();
            return View(result);
        }

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
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Card card)
        {
            if (ModelState.IsValid)
            {
                //await _cardRepository.SaveCard(card);
                //return RedirectToAction(nameof(HomeCards));
            }
            return View(card);
        }

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
                return RedirectToAction(nameof(HomeCards));
            }
            return View(card);
        }

        // GET: Cards/Delete/5
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
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _cardRepository.DeleteConfirmedAsync(id);
            return RedirectToAction(nameof(HomeCards));
        }


    }
}

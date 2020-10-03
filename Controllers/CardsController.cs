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
using Microsoft.Extensions.Caching.Distributed;
using System.Data.SQLite;

namespace CodingCards.Controllers
{
    [Authorize]
    public class CardsController : Controller
    {
        private readonly ICardRepository _cardRepository;
        private readonly string SearchVMCacheKey = "SearchVMCacheKey";
        private readonly IDistributedCache _cache;
        private readonly ApplicationDbContext _ctx;
        public CardsController(ApplicationDbContext context,ICardRepository cardRepo, IDistributedCache cache)
        {
            _ctx = context;
            _cardRepository = cardRepo;
            _cache = cache;
        }


        [HttpGet]
        [AllowAnonymous]
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
        
        [HttpGet, ActionName("CSVToDB")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DbToIndex()
        {
            await GetDataFromSQLite(_ctx);
            return RedirectToAction(nameof(Index));
        }

        private async Task GetDataFromSQLite(ApplicationDbContext _context)
        {
            string cs = @"Data Source=C:\Users\Avane\Documents\Repos\PersonalProjects\CodingCards\DBS\collectionSysDesign.anki2";

            using (SQLiteConnection con = new SQLiteConnection(cs))
            {
                con.Open();

                //string stm = "SELECT * FROM cards";
                string stm = "SELECT * FROM notes";
                using (SQLiteCommand cmd = new SQLiteCommand(stm, con))
                {
                    using (SQLiteDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            //string Answer = rdr.GetString(3);
                            //string Question = rdr.GetString(2);
                            //var thing = _context.Cards.Where(x => x.Answer.Equals(Answer));

                            //if (thing.Count() == 0)
                            //{
                            //    await _context.Cards.AddAsync(new Card()
                            //    {
                            //        Answer = rdr.GetString(3),
                            //        Question = rdr.GetString(2)
                            //    });
                            //    _context.SaveChanges();
                            //}

                            string Answer = rdr.GetString(6);
                            string Question = rdr.GetString(7);
                            await _context.Cards.AddAsync(new Card()
                            {
                                Answer = Answer,
                                Question = Question,
                            });
                            _context.SaveChanges();


                        }
                    }
                }

                con.Close();
            }
        }

    }
}

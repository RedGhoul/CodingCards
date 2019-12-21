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
        private readonly ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;

        public CardsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Cards
        public async Task<IActionResult> ListAllCards()
        {
            //await Helper.GetDataFromSQLite(_context);
            ViewBag.datasource = await _context.Cards.ToListAsync();
            return View();
        }

        public async Task<IActionResult> GetCard()
        {
            int total = _context.Cards.Count();
            Random r = new Random();
            int offset = r.Next(0, total);

            var result = _context.Cards.Skip(offset).FirstOrDefault();

            var user = await _userManager.GetUserAsync(User);
            result.NumberOfViews++;
            _context.ApplicationUserCards.Add(new ApplicationUserCard()
            {
                ApplicationUser = user,
                Card = result
            });
            await _context.SaveChangesAsync();

            return View(result);
        }

        public async Task<IActionResult> GetAnswer(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var card = await _context.Cards
                .FirstOrDefaultAsync(m => m.id == id);

            if (card == null)
            {
                return NotFound();
            }
            card.NumberOfViewAnswers++;
            await _context.SaveChangesAsync();
            return View(card);
        }

        // GET: Cards/Create
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,Question,Answer")] Card card)
        {
            if (ModelState.IsValid)
            {
                _context.Add(card);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ListAllCards));
            }
            return View(card);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var card = await _context.Cards.FindAsync(id);
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
                    _context.Update(card);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CardExists(card.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(ListAllCards));
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

            var card = await _context.Cards
                .FirstOrDefaultAsync(m => m.id == id);
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
            var card = await _context.Cards.FindAsync(id);
            _context.Cards.Remove(card);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ListAllCards));
        }

        private bool CardExists(int id)
        {
            return _context.Cards.Any(e => e.id == id);
        }
    }
}

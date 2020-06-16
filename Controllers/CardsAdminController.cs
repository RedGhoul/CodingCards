using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CodingCards.Data;
using CodingCards.Models;

namespace CodingCards.Controllers
{
    public class CardsAdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CardsAdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CardsAdmin
        public async Task<IActionResult> Index()
        {
            var listOfCards = await _context.Cards.ToListAsync();
            List<AdminCardDTO> Cards = new List<AdminCardDTO>();
            foreach (var item in listOfCards)
            {
                Cards.Add(new AdminCardDTO
                {
                    id = item.id,
                    Name = item.Name,
                    Question = item.Question.Length <= 50 ? item.Question : item.Question.Substring(0, 50),
                    Type = item.Type,
                    NumberOfViewAnswers = item.NumberOfViewAnswers,
                    LangName = item.LangName
                });
            }
            return View(Cards);
        }

        // GET: CardsAdmin/Details/5
        public async Task<IActionResult> Details(int? id)
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

        // GET: CardsAdmin/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CardsAdmin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,Name,Question,Answer,Type,NumberOfViews,NumberOfViewAnswers,LangName")] Card card)
        {
            if (ModelState.IsValid)
            {
                _context.Add(card);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(card);
        }

        // GET: CardsAdmin/Edit/5
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

        // POST: CardsAdmin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,Name,Question,Answer,Type,NumberOfViews,NumberOfViewAnswers,LangName")] Card card)
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
                return RedirectToAction(nameof(Index));
            }
            return View(card);
        }

        // GET: CardsAdmin/Delete/5
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

        // POST: CardsAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var card = await _context.Cards.FindAsync(id);
            _context.Cards.Remove(card);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CardExists(int id)
        {
            return _context.Cards.Any(e => e.id == id);
        }
    }
}

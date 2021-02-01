using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
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
        private readonly IMapper _mapper;
        private ApplicationDbContext _ctx;
        public DataAdminController(
            ApplicationDbContext context,
            IMapper mapper,
            ICardRepository cardRepo, 
            UserManager<ApplicationUser> userManager,
            ElasticService es)
        {
            _ctx = context;
            _cardRepository = cardRepo;
            _userManager = userManager;
            _es = es;
            _mapper = mapper;
        }

        public async Task<IActionResult> IndexIntoElastic()
        {
            var items = await _cardRepository.GetCardsAllAsync();

            foreach (var card in items)
            {
                var CardDTO = _mapper.Map<CardDTO>(card);
                await _es.AddCardToES(CardDTO);
            }
           

            return RedirectToAction("Index", "Home");
        }

        [HttpGet, ActionName("CSVToDB")]
        public async Task<IActionResult> DbToIndex()
        {
            await GetDataFromSQLite(_ctx);
            return RedirectToAction(nameof(Index));
        }

        private async Task GetDataFromSQLite(ApplicationDbContext _context)
        {
            string cs = @"Data Source=C:\Users\Avane\Documents\Repos\PersonalProjects\CodingCards\DBS\cards-jwasham-extreme.db";

            using (SQLiteConnection con = new SQLiteConnection(cs))
            {
                con.Open();

                string stm = "SELECT * FROM cards";
                //string stm = "SELECT * FROM notes";
                using (SQLiteCommand cmd = new SQLiteCommand(stm, con))
                {
                    using (SQLiteDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            string Answer = rdr.GetString(3);
                            string Question = rdr.GetString(2);
                            var thing = _context.Cards.Where(x => x.Answer.Equals(Answer));

                            if (thing.Count() == 0)
                            {
                                await _context.Cards.AddAsync(new Card()
                                {
                                    Answer = rdr.GetString(3),
                                    Question = rdr.GetString(2)
                                });
                                _context.SaveChanges();
                            }

                            //string Answer = rdr.GetString(6);
                            //string Question = rdr.GetString(7);
                            //await _context.Cards.AddAsync(new Card()
                            //{
                            //    Answer = Answer,
                            //    Question = Question,
                            //});
                            _context.SaveChanges();


                        }
                    }
                }

                con.Close();
            }
        }

    }
}
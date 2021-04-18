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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CodingCards.Controllers
{
    [Authorize(Roles="Admin")]
    public class DataAdminController : Controller
    {
        private UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private ApplicationDbContext _ctx;
        public DataAdminController(
            ApplicationDbContext context,
            IMapper mapper,
            UserManager<ApplicationUser> userManager)
        {
            _ctx = context;
            _userManager = userManager;
            _mapper = mapper;
        }

        [HttpGet, ActionName("SQLiteToDB")]
        public async Task<IActionResult> DbToIndex()
        {
            await GetDataFromSQLite(_ctx);
            return RedirectToAction("Index", "Home");
        }

        private async Task GetDataFromSQLite(ApplicationDbContext _context)
        {
            string dbLocationExtremeCardsJwashamExtreme = @"Data Source=C:\Users\Avane\Documents\Repos\PersonalProjects\CodingCards\DBS\cards-jwasham-extreme.db";

            using (SQLiteConnection con = new(dbLocationExtremeCardsJwashamExtreme))
            {
                con.Open();

                string stm = "SELECT * FROM cards";
                using (SQLiteCommand cmd = new(stm, con))
                {
                    using (SQLiteDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            string Answer = rdr.GetString(3);
                            string Question = rdr.GetString(2);
                            var thing = _context.Cards.Where(x => x.Answer.Equals(Answer));

                            if (!thing.Any())
                            {
                                await _context.Cards.AddAsync(new Card()
                                {
                                    Answer = rdr.GetString(3),
                                    Question = rdr.GetString(2)
                                });
                                _context.SaveChanges();
                            }
                        }
                    }
                }

                con.Close();
            }

            string collectionOODesign = @"Data Source=C:\Users\Avane\Documents\Repos\PersonalProjects\CodingCards\DBS\collectionOODesign.anki2";

            using (SQLiteConnection con = new SQLiteConnection(collectionOODesign))
            {
                con.Open();
                string stm = "SELECT * FROM notes";
                using (SQLiteCommand cmd = new(stm, con))
                {
                    using (SQLiteDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            string Answer = rdr.GetString(6);
                            var thing = _context.Cards.Where(x => x.Answer.Equals(Answer));
                            if (!thing.Any())
                            {
                                
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
                }

                con.Close();
            }

            string collectionSysDesign = @"Data Source=C:\Users\Avane\Documents\Repos\PersonalProjects\CodingCards\DBS\collectionSysDesign.anki2";

            using (SQLiteConnection con = new(collectionSysDesign))
            {
                con.Open();
                string stm = "SELECT * FROM notes";
                using (SQLiteCommand cmd = new(stm, con))
                {
                    using (SQLiteDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            string Answer = rdr.GetString(6);
                            var thing = _context.Cards.Where(x => x.Answer.Equals(Answer));
                            if (!thing.Any())
                            {
                                
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
                }

                con.Close();
            }
        }

    }
}
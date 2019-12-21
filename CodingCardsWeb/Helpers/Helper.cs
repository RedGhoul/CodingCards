using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;
using CodingCards.Data;
using CodingCards.Models;

namespace CodingCards.Helpers
{
    public static class Helper
    {
        public static async Task GetDataFromSQLite(ApplicationDbContext _context)
        {
            string cs = @"Data Source=C:\Users\Avane\source\repos\CodingCards\CodingCards\collectionSysDesign.anki2";

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
                            // string Answer = rdr.GetString(3);
                            // string Question = rdr.GetString(2);
                            // var thing = _context.Cards.Where(x => x.Answer.Equals(Answer));

                            // if (thing.Count() == 0)
                            // {
                            //     await _context.Cards.AddAsync(new Card()
                            //     {
                            //         Answer = rdr.GetString(3),
                            //         Question = rdr.GetString(2)
                            //     });
                            //     _context.SaveChanges();
                            // }

                            string Answer = rdr.GetString(6);
                            string Question = rdr.GetString(7);
                            await _context.Cards.AddAsync(new Card()
                            {
                                Answer = Answer,
                                Question = Question
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

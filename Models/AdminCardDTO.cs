using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodingCards.Models
{
    public class AdminCardDTO
    {
        public int id;

        public string Name { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public CardType Type { get; set; }
        public int NumberOfViews { get; set; }
        public int NumberOfViewAnswers { get; set; }
        public string LangName { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodingCards.Models
{
    public class Card
    {
        public int id { get; set; }
        public string Name { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public CardType Type { get; set; }
        public int NumberOfViews { get; set; }
        public int NumberOfViewAnswers { get; set; }
        public ApplicationUser CardCreator { get; set; }
    }
}

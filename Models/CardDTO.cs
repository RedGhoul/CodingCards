using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodingCards.Models
{
    public class CardDTO
    {
        public int id { get; set; }
        public string Name { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public CardType Type { get; set; }
        public string LangName { get; set; }
    }
}

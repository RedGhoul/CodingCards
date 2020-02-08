using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodingCards.Models
{
    public class CardIndexViewModel
    {
        public List<Card> Cards { get; set; }
        public string KeyWords { get; set; }
        public CardType? CardType { get; set; }
        public int Page { get; set; }
        public int MaxResults { get; set; }
    }
}

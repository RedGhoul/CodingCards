using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodingCards.Models
{
    public class ApplicationUserCard
    {
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public int CardId { get; set; }
        public Card Card { get; set; }
    }
}

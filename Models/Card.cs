using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodingCards.Models
{
    public class Card
    {
        public Card()
        {
            this.DateModified = DateTime.UtcNow;
            this.DateCreated = DateTime.UtcNow;
        }

        public int id { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public CardType Type { get; set; }
        public int NumberOfViews { get; set; }
        public int NumberOfViewAnswers { get; set; }
        public string LangName { get; set; }
        public ApplicationUser CardCreator { get; set; }
        public string? CardCreatorId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateDeleted { get; set; }
        public DateTime DateModified { get; set; }
    }
}

using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodingCards.Models
{
    public class CardCreateViewModel
    {
        public string Name { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public IEnumerable<SelectListItem> Card_Type { get; set; }
        public CardType Type { get; set; }
        public int NumberOfViews { get; set; }
        public int NumberOfViewAnswers { get; set; }
        public string LangName { get; set; }
    }
}

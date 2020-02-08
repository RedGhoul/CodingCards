using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodingCards.Models;

namespace CodingCards.Util
{
    public static class CardHelpers
    {
        public static void SetupViewBag(CardIndexViewModel homeIndexVM, dynamic ViewBag)
        {
            ViewBag.KeyWords = homeIndexVM.KeyWords;
            ViewBag.MaxResults = homeIndexVM.MaxResults;
            ViewBag.TotalJobs = homeIndexVM.MaxResults != 0 ? homeIndexVM.MaxResults : 100;
            ViewBag.Page = homeIndexVM.Page;
        }

        public static CardIndexViewModel SetDefaultFindModel(CardIndexViewModel CardIndexVM)
        {
            if (CardIndexVM == null)
            {
                CardIndexVM = new CardIndexViewModel();

            }

            CardIndexVM.KeyWords ??= "";

            if (CardIndexVM.MaxResults == 0)
            {
                CardIndexVM.MaxResults = 100;
            }

            if (CardIndexVM.Page == 0)
            {
                CardIndexVM.Page = 1;
            }

            if (CardIndexVM.CardType == null)
            {
                CardIndexVM.CardType = CardType.Generic;
            }
            return CardIndexVM;

        }

    }
}

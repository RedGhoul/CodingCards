using System.Diagnostics;
using System.Threading.Tasks;
using CodingCards.Data;
using Microsoft.AspNetCore.Mvc;
using CodingCards.Models;

namespace CodingCards.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICardRepository _cardRepository;

        public HomeController(ICardRepository cardRepository)
        {
            _cardRepository = cardRepository;
        }

        public async Task<IActionResult> Index()
        {
            var cards = await _cardRepository.GetRandomSetOfCardsAsyncDb(9);
            CardIndexViewModel vm = new CardIndexViewModel();
            vm.Cards = cards;
            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


    }

}

using CodingCards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CodingCards.Data
{
    public interface ICardRepository
    {
        Task<int> GetTotalCards();
        Task<Card> GetRandomCardAsync();
        Task<Card> GetCardAsync(int? id);
        Task<Card> SaveCard(Card card, ClaimsPrincipal user);
        Task<List<Card>> GetUserCards(ClaimsPrincipal user);
        Task<Card> UpdateCard(Card card);
        Task DeleteConfirmed(int? id);
        Task<List<Card>> GetRandomSetOfCardsAsyncDb(int totalAmount);
        bool CardExists(int id);
        List<Card> ConfigureSearch(CardIndexViewModel cardIndexVM);
    }
}

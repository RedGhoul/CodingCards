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
        Task<IEnumerable<Card>> GetCardsAsync(int amount);
        Task<IEnumerable<Card>> GetCardsAllAsync();
        Task<Card> GetRandomCardAsync();
        Task<Card> GetCardAsync(int? id);
        Task<Card> SaveCard(Card card);
        Task<Card> SaveCard(Card card, ClaimsPrincipal user);
        Task<List<Card>> GetUserCards(ClaimsPrincipal user);
        Task<Card> UpdateCard(Card card);
        Task DeleteConfirmedAsync(int? id);
        Task<List<Card>> GetRandomSetOfCardsAsyncEs(int totalAmount);
        Task<List<Card>> GetRandomSetOfCardsAsyncDb(int totalAmount);
        bool CardExists(int id);
        Task<List<Card>> ConfigureSearchAsync(CardIndexViewModel cardIndexVM);
    }
}

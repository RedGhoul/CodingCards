using CodingCards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodingCards.Data
{
    public interface ICardRepository
    {
        Task<string> GetTotalCards();
        Task<IEnumerable<Card>> GetCardsAsync(int amount);
        Task<IEnumerable<Card>> GetCardsAllAsync();
        Task<Card> GetRandomCardAsync();
        Task<Card> GetCardAsync(int? id);
        Task<Card> SaveCard(Card card);
        Task<Card> UpdateCard(Card card);
        Task DeleteConfirmedAsync(int? id);
        Task<List<Card>> GetRandomSetOfCardsAsyncES(int totalAmount);
        Task<List<Card>> GetRandomSetOfCardsAsyncDB(int totalAmount);
        bool CardExists(int id);
    }
}

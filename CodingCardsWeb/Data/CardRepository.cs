using CodingCards.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodingCards.Data
{
    public class CardRepository : ICardRepository
    {
        private readonly ApplicationDbContext _ctx;
        private readonly IDistributedCache _cache;

        public CardRepository(ApplicationDbContext ctx, IDistributedCache cache)
        {
            _ctx = ctx;
            _cache = cache;
        }
        public async Task<string> GetTotalCards()
        {
            string cacheKey = "TotalCards";
            string TotalCards = await _cache.GetStringAsync(cacheKey);

            if (string.IsNullOrEmpty(TotalCards))
            {
                int TotalCardsI = await _ctx.Cards.CountAsync();
                TotalCards = TotalCardsI.ToString();
                var options = new DistributedCacheEntryOptions();
                options.SetSlidingExpiration(TimeSpan.FromMinutes(30));
                await _cache.SetStringAsync(cacheKey, TotalCards, options);
            }

            return TotalCards;
        }

        public async Task<IEnumerable<Card>> GetCardsAsync(int amount)
        {
            string cacheKey = "Cards" + amount;
            string JobsString = await _cache.GetStringAsync(cacheKey);
            IEnumerable<Card> cards = null;
            if (string.IsNullOrEmpty(JobsString))
            {
                cards = await _ctx.Cards.Take(amount).ToListAsync();
                var options = new DistributedCacheEntryOptions();
                options.SetSlidingExpiration(TimeSpan.FromMinutes(30));
                await _cache.SetStringAsync(cacheKey, JsonConvert.SerializeObject(cards), options);
            }
            else
            {
                cards = JsonConvert.DeserializeObject<IEnumerable<Card>>(JobsString);
            }

            return cards;
        }

        public async Task<IEnumerable<Card>> GetCardsAllAsync()
        {
            string cacheKey = "CardsAll";
            string JobsString = await _cache.GetStringAsync(cacheKey);
            IEnumerable<Card> cards = null;
            if (string.IsNullOrEmpty(JobsString))
            {
                cards = await _ctx.Cards.ToListAsync();
                var options = new DistributedCacheEntryOptions();
                options.SetSlidingExpiration(TimeSpan.FromMinutes(30));
                await _cache.SetStringAsync(cacheKey, JsonConvert.SerializeObject(cards), options);
            }
            else
            {
                cards = JsonConvert.DeserializeObject<IEnumerable<Card>>(JobsString);
            }

            return cards;
        }

        public async Task<Card> GetRandomCardAsync()
        {
            string total = await GetTotalCards();
            Random r = new Random();
            int offset = r.Next(0, Int32.Parse(total));

            var result = await GetCardAsync(offset);

            return result;
        }

        public async Task<IEnumerable<Card>> GetRandomSetOfCardsAsync(int totalAmount)
        {
            string cacheKey = "RandomSetOfCards";
            string cardsString = await _cache.GetStringAsync(cacheKey);
            IEnumerable<Card> cards = null;
            if (string.IsNullOrEmpty(cardsString))
            {
                Random r = new Random();
                int offset = r.Next(0, totalAmount);
                cards = await _ctx.Cards.Take(totalAmount).Skip(offset).ToListAsync();
                var options = new DistributedCacheEntryOptions();
                options.SetSlidingExpiration(TimeSpan.FromMinutes(30));
                await _cache.SetStringAsync(cacheKey, JsonConvert.SerializeObject(cards), options);
            }
            else
            {
                cards = JsonConvert.DeserializeObject<IEnumerable<Card>>(cardsString);
            }
            

            return cards;
        }

        public async Task<Card> GetCardAsync(int? id)
        {
            if (id == null)
            {
                return null;
            }

            var card = await _ctx.Cards
                .FirstOrDefaultAsync(m => m.id == id);

            if (card == null)
            {
                return null;
            }
            card.NumberOfViewAnswers++;
            await _ctx.SaveChangesAsync();

            return card;
        }

        public async Task<Card> SaveCard(Card card)
        {
            _ctx.Add(card);
            await _ctx.SaveChangesAsync();

            return card;
        }

        public async Task<Card> UpdateCard(Card card)
        {
            try
            {
                _ctx.Update(card);
                await _ctx.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CardExists(card.id))
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }

            return card;
        }

        public bool CardExists(int id)
        {
            return _ctx.Cards.Any(e => e.id == id);
        }

        public async Task DeleteConfirmedAsync(int? id)
        {
            var card = await GetCardAsync(id);
            if(card != null)
            {
                _ctx.Cards.Remove(card);
                await _ctx.SaveChangesAsync();
            }

        }

    }
}

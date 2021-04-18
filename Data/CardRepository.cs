using CodingCards.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using AutoMapper;

namespace CodingCards.Data
{
    public class CardRepository : ICardRepository
    {
        private readonly ApplicationDbContext _ctx;
        private readonly IDistributedCache _cache;
        private UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public CardRepository(
            IMapper mapper,
            ApplicationDbContext ctx, 
            IDistributedCache cache, 
            UserManager<ApplicationUser> userManager)
        {
            _ctx = ctx;
            _cache = cache;
            _userManager = userManager;
            _mapper = mapper;
        }
        public async Task<int> GetTotalCards()
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

            return Int32.Parse(TotalCards);
        }

        public async Task<Card> GetRandomCardAsync()
        {
            int total = await GetTotalCards();
            Random r = new Random();
            int offset = r.Next(0, total);

            var result = await GetCardAsync(offset);
          

            return result;
        }

        public async Task<List<Card>> GetRandomSetOfCardsAsyncDb(int totalAmount)
        {
            if(_ctx.Cards.Count() == 0)
            {
                var cardLists = new List<Card>();
                return cardLists;
            }
            var cards = await _ctx.Cards.Skip(new Random().Next(1, _ctx.Cards.Count())).Take(totalAmount).ToListAsync();
            foreach (Card card in cards)
            {
                card.NumberOfViews++;
            }
            await _ctx.SaveChangesAsync();
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
            card.NumberOfViews++;
            await _ctx.SaveChangesAsync();

            return card;
        }

        public async Task<List<Card>> GetUserCards(ClaimsPrincipal user)
        {
            var currentUser = await _userManager.GetUserAsync(user);
            var userCards = await _ctx.Cards
                .Where(x => x.CardCreator.Id.Equals(currentUser.Id))
                .ToListAsync();

            return userCards;
        }

        public async Task<Card> UpdateCard(Card card)
        {
            try
            {
                _ctx.Update(card);
                await _ctx.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!CardExists(card.id))
                {
                    return null;
                }
                else
                {
                    Console.WriteLine(ex.InnerException);
                }
            }

            return card;
        }

        public bool CardExists(int id)
        {
            return _ctx.Cards.Any(e => e.id == id);
        }

        public List<Card> ConfigureSearch(CardIndexViewModel cardIndexVM)
        {
            var fromNumber = 0;
            if (cardIndexVM.Page > 1)
            {
                fromNumber = cardIndexVM.Page * 12;
            }
            return _ctx.Cards.Skip(12 * fromNumber).Take(12).ToList();
        }


        public async Task DeleteConfirmed(int? id)
        {
            var card = await GetCardAsync(id);
            if(card != null)
            {
                _ctx.Cards.Remove(card);
                await _ctx.SaveChangesAsync();
            }

        }

        public async Task<Card> SaveCard(Card card, ClaimsPrincipal user)
        {
            var currentUser = await _userManager.GetUserAsync(user);
            card.CardCreator = currentUser;
            _ctx.Add(card);
            await _ctx.SaveChangesAsync();
            var CardDTO = _mapper.Map<CardDTO>(card);
            return card;
        }
    }
}

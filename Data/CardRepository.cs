using CodingCards.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CodingCards.Services;
using Microsoft.AspNetCore.Identity;
using AutoMapper;

namespace CodingCards.Data
{
    public class CardRepository : ICardRepository
    {
        private readonly ApplicationDbContext _ctx;
        private readonly IDistributedCache _cache;
        private readonly ElasticService _es;
        private UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public CardRepository(
            IMapper mapper,
            ApplicationDbContext ctx, 
            IDistributedCache cache, 
            ElasticService es,
            UserManager<ApplicationUser> userManager)
        {
            _ctx = ctx;
            _cache = cache;
            _es = es;
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
            int total = await GetTotalCards();
            Random r = new Random();
            int offset = r.Next(0, total);

            var result = await GetCardAsync(offset);
          

            return result;
        }

        public async Task<List<Card>> GetRandomSetOfCardsAsyncEs(int totalAmount)
        {
            var CardDTOs = await _es.QueryJobPosting(new Random().Next(1, 50), "", totalAmount,0);
            var Cards = _mapper.Map<List<Card>>(CardDTOs);
            return Cards;
        }

        public async Task<List<Card>> GetRandomSetOfCardsAsyncDb(int totalAmount)
        {
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

        public async Task<Card> SaveCard(Card card)
        {
            _ctx.Add(card);
            await _ctx.SaveChangesAsync();
            var CardDTO = _mapper.Map<CardDTO>(card);
            await _es.AddCardToES(CardDTO);
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

        public async Task<List<Card>> ConfigureSearchAsync(CardIndexViewModel cardIndexVM)
        {
            var fromNumber = 0;
            if (cardIndexVM.Page > 1)
            {
                fromNumber = cardIndexVM.Page * 12;
            }
            var cardDTOs = await _es.QueryJobPosting(fromNumber, cardIndexVM.KeyWords,12,cardIndexVM.CardType);
            var Cards = _mapper.Map<List<Card>>(cardDTOs);
            return Cards;
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

        public async Task<Card> SaveCard(Card card, ClaimsPrincipal user)
        {
            var currentUser = await _userManager.GetUserAsync(user);
            card.CardCreator = currentUser;
            _ctx.Add(card);
            await _ctx.SaveChangesAsync();
            var CardDTO = _mapper.Map<CardDTO>(card);
            await _es.AddCardToES(CardDTO);
            return card;
        }
    }
}

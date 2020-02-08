using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CodingCards.Models;
using Newtonsoft.Json;

namespace CodingCards.Services
{
    public class ElasticService
    {
        public string baseUrlsearch = "http://a-main-elastic.experimentsinthedeep.com/codinginterviewcards/_search?";
        public async Task<bool> AddCardToES(Card card)
        {
            var json = JsonConvert.SerializeObject(card, Formatting.None,
                new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var client = new HttpClient();

            var response = await client.PutAsync("http://a-main-elastic.experimentsinthedeep.com" + "/codinginterviewcards/_doc/" + card.id, data);

            return response.IsSuccessStatusCode;

        }
        public async Task<List<Card>> QueryJobPosting(int fromNumber, string keywords, int size)
        {
            var client = new HttpClient();

            HttpResponseMessage response = null;
            string finalQueryString = "";
            if (string.IsNullOrEmpty(keywords))
            {
                finalQueryString = baseUrlsearch + "q=from=" + fromNumber + "&size=" + size;
            }
            else
            {
                finalQueryString = baseUrlsearch + "q=Answer:" + keywords + "&from=" + fromNumber + "&size=" + 12;
            }
            response = await client.GetAsync(finalQueryString);

            var result = response.Content.ReadAsStringAsync().Result;

            try
            {
                RootObject list = JsonConvert
                    .DeserializeObject<RootObject>(result);
                List<Card> listsCards = new List<Card>();
                foreach (var item in list.hits.hits)
                {

                    listsCards.Add(item._source);
                }
                return listsCards;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
                return new List<Card>();
            }


        }

    }
}

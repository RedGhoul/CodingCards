using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CodingCards.Models;
using Newtonsoft.Json;

namespace CodingCards.Services
{
    public class ElasticService
    {
        public string baseUrlsearch = "http://a-main-elastic.experimentsinthedeep.com/cards/_search?";

        public async Task<List<Card>> QueryJobPosting(int fromNumber, string keywords, int size)
        {
            var client = new HttpClient();
            //http://a-main-elastic.experimentsinthedeep.com/jobpostings/_search?q=Description:aws&from=12&size=12

            HttpResponseMessage response = null;
            string finalQueryString = "";
            if (string.IsNullOrEmpty(keywords))
            {
                finalQueryString = baseUrlsearch + "q=from=" + fromNumber + "&size=" + size;
            }
            else
            {
                finalQueryString = baseUrlsearch + "q=Question:" + keywords + "&from=" + fromNumber + "&size=" + 12;
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
                return null;
            }


        }
    }
}

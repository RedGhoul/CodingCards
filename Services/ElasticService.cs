using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CodingCards.Helpers;
using CodingCards.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace CodingCards.Services
{
    public class ElasticService
    {
        public string baseUrl = "";
        public ElasticService(IConfiguration configuration)
        {
            baseUrl = Secrets.GetConnectionString(configuration,"CardsDigitalOceanPROD_ES") + "/codinginterviewcards/";
        }
        public async Task<bool> AddCardToES(Card card)
        {
            var json = JsonConvert.SerializeObject(card, Formatting.None,
                new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var client = new HttpClient();

            var response = await client.PutAsync(baseUrl + "_doc/" + card.id, data);

            return response.IsSuccessStatusCode;

        }
        public async Task<List<Card>> QueryJobPosting(int fromNumber, string keywords, int size, CardType? cardType)
        {
            var client = new HttpClient();
            string result = "";
            HttpResponseMessage response = null;
            if (string.IsNullOrEmpty(keywords.Replace(" ", "")))
            {
                string finalQueryString = baseUrl + "_search?" + "q=Type:"+ (int)cardType + "&from=" + fromNumber + "&size=" + size;
                response = await client.GetAsync(finalQueryString);

                result = response.Content.ReadAsStringAsync().Result;
            }
            else
            {
                RootDSLObject dsl = new RootDSLObject();
                try
                {

                    dsl.from = fromNumber;
                    dsl.size = size;
                    dsl.query = new Query();
                    dsl.query.@bool = new Bool();
                    dsl.query.@bool.must = new List<Must>();
                    dsl.query.@bool.filter = new List<Filter>();
                    dsl.query.@bool.must.Add(new Must
                    {
                        match = new
                        {
                            Answer = keywords
                        }
                    });

                    dsl.query.@bool.must.Add(new Must
                    {
                        match = new
                        {
                            Question = keywords
                        }
                    });
                    dsl.query.@bool.filter.Add(new Filter
                    {
                        term = new
                        {
                            Type = cardType
                        }
                    });
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                var json = JsonConvert.SerializeObject(dsl);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                response = await client.PostAsync(baseUrl + "_search", data);

                result = response.Content.ReadAsStringAsync().Result;
            }

 
           

            try
            {
                RootResponseObject list = JsonConvert
                    .DeserializeObject<RootResponseObject>(result);
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

        public async Task<List<Card>> QueryJobPosting(int fromNumber, string keywords, int size)
        {
            var client = new HttpClient();

            HttpResponseMessage response = null;
            RootDSLObject dsl = new RootDSLObject();
            try
            {

                dsl.from = fromNumber;
                dsl.size = size;
                dsl.query = new Query();
                dsl.query.@bool = new Bool();
                dsl.query.@bool.must = new List<Must>();
                dsl.query.@bool.filter = new List<Filter>();
                dsl.query.@bool.must.Add(new Must
                {
                    match = new
                    {
                        Answer = keywords
                    }
                });

                dsl.query.@bool.must.Add(new Must
                {
                    match = new
                    {
                        Question = keywords
                    }
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            var json = JsonConvert.SerializeObject(dsl);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            response = await client.PostAsync(baseUrl + "_search", data);

            var result = response.Content.ReadAsStringAsync().Result;

            try
            {
                RootResponseObject list = JsonConvert
                    .DeserializeObject<RootResponseObject>(result);
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

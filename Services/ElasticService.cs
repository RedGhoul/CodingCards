using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CodingCards.Helpers;
using CodingCards.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nest;
using Newtonsoft.Json;

namespace CodingCards.Services
{
    public class ElasticService
    {
        public string baseUrl;
        private static ElasticClient elasticClient;
        private ILogger<ElasticService> _logger;
        public ElasticService(IConfiguration configuration, ILogger<ElasticService> logger)
        {
            baseUrl = Secrets.GetConnectionString(configuration, "App_ElasticIndexBaseUrl");
            var settings = new ConnectionSettings(new Uri(baseUrl))
                .DefaultIndex("codinginterviewcards")
                .BasicAuthentication(
                    Secrets.GetAppSettingsValue(configuration, "ELASTIC_USERNAME_Search"),
                    Secrets.GetAppSettingsValue(configuration, "ELASTIC_PASSWORD_Search"))
                .RequestTimeout(TimeSpan.FromMinutes(2));
            elasticClient = new ElasticClient(settings);
            _logger = logger;
        }
        public async Task<bool> AddCardToES(CardDTO cardDTO)
        {
            var returnedResponse = await elasticClient.IndexDocumentAsync(cardDTO);

            return returnedResponse.IsValid;
        }

        public async Task<List<CardDTO>> QueryJobPosting(int fromNumber, string keywords, int size, CardType? cardType)
        {
            _logger.LogInformation($"fromNumber: {fromNumber}  keywords: {keywords}");
            var searchResponse = await elasticClient.SearchAsync<CardDTO>(s => s
                .From(fromNumber)
                .Size(size)
                .Query(q => q
                     .Match(m => m
                        .Field(f => f.Name)
                        .Query(keywords)
                     )
                ).Query(q => q
                     .Match(m => m
                        .Field(f => f.Question)
                        .Query(keywords)
                     )
                ));
            _logger.LogInformation(searchResponse.DebugInformation);
            var Cards = searchResponse.Documents;
            
            return (List<CardDTO>)Cards;
        }

    }
}

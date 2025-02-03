using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace GsFlix
{
    public class fnGetAllMovies
    {
        private readonly ILogger<fnGetAllMovies> _logger;
        private readonly CosmosClient _cosmosClient;
        public fnGetAllMovies(ILogger<fnGetAllMovies> logger, CosmosClient cosmosClient)
        {
            _logger = logger;
            _cosmosClient = cosmosClient;
        }
        

        [Function("fnGetAllMovies")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var container = _cosmosClient.GetContainer("GsFlixDB", "movies");

            var query = new QueryDefinition("SELECT * FROM c");

            var result = container.GetItemQueryIterator<MovieResult>(query);

            var results = new List<MovieResult>();

            while (result.HasMoreResults)
            {
              foreach (var item in await result.ReadNextAsync())
              {
                  results.Add(item);
              }
            }

            var responseMessage = req.CreateResponse(HttpStatusCode.OK);
            await responseMessage.WriteAsJsonAsync(results);

            return responseMessage;
        }
    }
}

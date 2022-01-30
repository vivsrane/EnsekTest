
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace EnsekApi
{
    public class CosmosDbServices : ICosmosDbService
    {
        private Container _container;

        public CosmosDbServices(
            CosmosClient dbClient,
            string databaseName,
            string containerName)
        {
            this._container = dbClient.GetContainer(databaseName, containerName);
        }
        
        public async Task AddItemAsync(MeterReading item)
        {
            await this._container.CreateItemAsync<MeterReading>(item, new PartitionKey(item.AccountId));
        }

        public async Task DeleteItemAsync(string id)
        {
            await this._container.DeleteItemAsync<MeterReading>(id, new PartitionKey(id));
        }

        public async Task<MeterReading> GetItemAsync(string id)
        {
            try
            {
                ItemResponse<MeterReading> response = await this._container.ReadItemAsync<MeterReading>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch(CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            { 
                return null;
            }

        }

        public async Task<IEnumerable<MeterReading>> GetItemsAsync(string queryString)
        {
            var query = this._container.GetItemQueryIterator<MeterReading>(new QueryDefinition(queryString));
            List<MeterReading> results = new List<MeterReading>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task UpdateItemAsync(string id, MeterReading item)
        {
            await this._container.UpsertItemAsync<MeterReading>(item, new PartitionKey(id));
        }
    }
}
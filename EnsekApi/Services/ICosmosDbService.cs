using System.Collections.Generic;
using System.Threading.Tasks;

namespace EnsekApi
{
    public interface ICosmosDbService
    {
        Task<IEnumerable<MeterReading>> GetItemsAsync(string query);
        Task<MeterReading> GetItemAsync(string id);
        Task AddItemAsync(MeterReading item);
        Task UpdateItemAsync(string id, MeterReading item);
        Task DeleteItemAsync(string id);
    }
}
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Azure.Cosmos;

namespace EnsekApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EnsekMeterReadController : ControllerBase
    {  
        private readonly ILogger<EnsekMeterReadController> _logger;

        public EnsekMeterReadController(ILogger<EnsekMeterReadController> logger)
        {
            _logger = logger;
        }
    

        [HttpPost]
        [Route("/meter-reading-uploads")]
        public async Task<MeterReadingUploadResponse> Post(IFormFile csvFile)
        {
            var errorList=new List<string>();
            var meterReadingModel=new MeterReading();
            
            var meterReadingData=await meterReadingModel.LoadFromCsvFile(csvFile,errorList);

            var client = new CosmosClientBuilder("")
                    .WithSerializerOptions(new CosmosSerializationOptions
                    {
                        PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
                    })
                    .Build();
            
            var cosmosDbService=new CosmosDbServices(client,"testDb","meterReading");

            foreach(var meterReading in meterReadingData.Where(mr=>mr!=null))
            {
                await cosmosDbService.AddItemAsync(meterReading);
            }

            return new MeterReadingUploadResponse(){
                ErrorMessages=errorList,
                FailedUploadRecords=meterReadingData.Count(meterReading=>meterReading==null),
                SuccessfulUploadRecords=meterReadingData.Count(meterReading=>meterReading!=null)
            };

            
        }

        
    }
}

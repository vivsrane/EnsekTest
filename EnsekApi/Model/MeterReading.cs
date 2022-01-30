using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.TypeConversion;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace EnsekApi
{
public class MeterReading
{
    [Required(ErrorMessage ="Account Id is required")]
    [JsonProperty("accountId")]
    public string AccountId { get; set; }

    [Required(ErrorMessage ="Account Id is required")]
    [JsonProperty("id")]
    public string Id { get; set; }
    
    [Required(ErrorMessage ="Meter reading date time is required")]
    [JsonProperty("meterReadingTime")]
    public DateTime MeterReadingTime { get; set; }
    
    [Required(ErrorMessage ="Meter value is required")]
    [JsonProperty("meterValue")]
    public int MeterValue { get; set; }

    public async Task<List<MeterReading>> LoadFromCsvFile(IFormFile csvFile,List<string> errors)
    {
        var meterReadings=new List<MeterReading>();

        using (var fileStream= csvFile.OpenReadStream())
        {
            using (var csvFileStream = new CsvReader(new StreamReader(fileStream) , CultureInfo.InvariantCulture))
            {
                await csvFileStream.ReadAsync();
                if (!csvFileStream.ReadHeader())
                {
                    return null;
                }

                while (await csvFileStream.ReadAsync())
                {
                    var meterReadingData=ReadFileData(csvFileStream,errors);
                    meterReadings.Add(meterReadingData);                    
                }
            }
        }

        return meterReadings;
    }
    public MeterReading ReadFileData(CsvReader csvFileReader, List<string> errors)
    {
        try{
            var meterReadingData=new MeterReading(){Id= Convert.ToString(Guid.NewGuid())};
            
            var validationResults = new List<ValidationResult>();

            meterReadingData.AccountId = csvFileReader.GetField<string>("AccountId");
            var meterReadingDateTime=csvFileReader.GetField<string>("MeterReadingDateTime");
            DateTime meterReadingTime;
            if(!DateTime.TryParseExact(meterReadingDateTime, "dd/MM/yyyy hh:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out meterReadingTime))        
            {
                    errors.Add($"Invalid Date for account number {meterReadingData.AccountId}");  
            }   
            else{
                meterReadingData.MeterReadingTime=meterReadingTime;
            }         
            meterReadingData.MeterValue = csvFileReader.GetField<int>("MeterReadValue");
                            
            if(Validator.TryValidateObject(meterReadingData, new ValidationContext(meterReadingData, null, null),validationResults))
            {
                return  meterReadingData;    
            }               
            foreach(var validationResult in validationResults)
            {
                errors.Add(validationResult.ErrorMessage);
            }
        }
        catch(TypeConverterException)
        {
            errors.Add("Invalid data");
            return null;            
        }catch(Exception)
        {
            throw;
        }
        return null;
    }


}
}
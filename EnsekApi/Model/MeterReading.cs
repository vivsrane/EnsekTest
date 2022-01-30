using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.AspNetCore.Http;

namespace EnsekApi
{
public class MeterReading
{
    [Required(ErrorMessage ="Account Id is required")]
    public string AccountId { get; set; }
    
    [Required(ErrorMessage ="Meter reading date time is required")]
    public DateTime MeterReadingTime { get; set; }
    
    [Required(ErrorMessage ="Meter value is required")]
    public int MeterValue { get; set; }

    public async Task<List<MeterReading>> LoadFromCsvFile(IFormFile csvFile,List<string> errors)
    {
        var meterReadings=new List<MeterReading>();

        using (var fileStream= csvFile.OpenReadStream())
        {
            using (var csvFileStream = new CsvReader(new StreamReader(fileStream) , CultureInfo.InvariantCulture))
            {
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
        var meterReadingData=new MeterReading();
        
        var validationResults = new List<ValidationResult>();

        meterReadingData.AccountId = csvFileReader.GetField<string>("AccountId");
        var meterReadingDateTime=csvFileReader.GetField<string>("MeterReadingDateTime");
        DateTime meterReadingTime;
        if(!DateTime.TryParse(meterReadingDateTime,out meterReadingTime))   
        {
                errors.Add($"Invalid Date for account number {meterReadingData.AccountId}");  
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
            return null;
    }


}
}
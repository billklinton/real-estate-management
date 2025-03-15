using CsvHelper;
using CsvHelper.Configuration;
using RealEstateManagement.Shareable.CsvMappers;
using RealEstateManagement.Shareable.Dtos;
using System.Globalization;

namespace RealEstateManagement.Domain.Services
{
    public class CsvService : ICsvService
    {
        public List<RealEstateDto> ReadCSV(Stream file)
        {
            var reader = new StreamReader(file);
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HeaderValidated = null,
                MissingFieldFound = null,
                TrimOptions = TrimOptions.Trim,
                Delimiter = ";"
            };

            var records = new List<RealEstateDto>();

            using (var csv = new CsvReader(reader, config))
            {
                csv.Context.RegisterClassMap<RealEstateMap>();
                records = csv.GetRecords<RealEstateDto>().ToList();     
            }

            return records;
        }
    }
}

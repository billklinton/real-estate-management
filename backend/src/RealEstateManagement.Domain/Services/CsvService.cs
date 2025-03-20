using CsvHelper;
using CsvHelper.Configuration;
using RealEstateManagement.Shareable.CsvMappers;
using RealEstateManagement.Shareable.Dtos;
using System.Globalization;
using System.Text;

namespace RealEstateManagement.Domain.Services
{
    public class CsvService : ICsvService
    {
        public List<RealEstateDto>? ReadCSV(Stream file)
        {
            try
            {
                var reader = new StreamReader(file, Encoding.Latin1);
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
                    csv.Context.RegisterClassMap<RealEstateCSVMapper>();
                    records = csv.GetRecords<RealEstateDto>().ToList();
                }

                return records;
            }
            catch 
            {
                return null;
            }            
        }
    }
}

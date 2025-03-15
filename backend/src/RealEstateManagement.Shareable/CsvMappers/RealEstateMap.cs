using CsvHelper.Configuration;
using RealEstateManagement.Shareable.Dtos;

namespace RealEstateManagement.Shareable.CsvMappers
{
    public class RealEstateMap : ClassMap<RealEstateDto>
    {
        public RealEstateMap()
        {
            Map(m => m.PropertyNumber).Name("PropertyNumber");
            Map(m => m.State).Name("State");
            Map(m => m.City).Name("City");
            Map(m => m.Neighborhood).Name("Neighborhood");
            Map(m => m.Address).Name("Address");
            Map(m => m.Price).Name("Price");
            Map(m => m.AppraisalValue).Name("AppraisalValue");
            Map(m => m.Discount).Name("Discount");
            Map(m => m.Description).Name("Description");
            Map(m => m.SaleMode).Name("SaleMode");
            Map(m => m.AccessLink).Name("AccessLink");
        }
    }
}

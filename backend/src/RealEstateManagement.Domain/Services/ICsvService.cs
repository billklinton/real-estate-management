using RealEstateManagement.Shareable.Dtos;
using RealEstateManagement.Shareable.Models;

namespace RealEstateManagement.Domain.Services
{
    public interface ICsvService
    {
        public List<RealEstateDto>? ReadCSV(Stream file);
    }
}

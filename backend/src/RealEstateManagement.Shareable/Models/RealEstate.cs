using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RealEstateManagement.Shareable.Models
{
    public class RealEstate
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = default!;

        public string PropertyNumber { get; set; } = default!;

        public string State { get; set; } = default!;

        public string City { get; set; } = default!;

        public string Neighborhood { get; set; } = default!;

        public string Address { get; set; } = default!;

        public string Price { get; set; } = default!;

        public string AppraisalValue { get; set; } = default!;

        public string Discount { get; set; } = default!;

        public string Description { get; set; } = default!;

        public string SaleMode { get; set; } = default!;

        public string AccessLink { get; set; } = default!;
    }
}

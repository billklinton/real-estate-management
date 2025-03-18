using Confluent.Kafka;
using System.Text;
using System.Text.Json;

namespace RealEstateManagement.Kafka
{
    public class ListDeserializer<T> : IDeserializer<List<T>>
    {
        public List<T> Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            if (isNull || data.IsEmpty)
                return new List<T>();

            var json = Encoding.UTF8.GetString(data);
            return JsonSerializer.Deserialize<List<T>>(json);
        }
    }
}

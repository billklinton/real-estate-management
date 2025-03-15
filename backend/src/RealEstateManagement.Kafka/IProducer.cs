namespace RealEstateManagement.Kafka
{
    public interface IProducer
    {
        public Task SendMessageAsync<T>(T message);
    }
}

namespace RealEstateManagement.Shareable.Configs
{
    public class KafkaConfig
    {
        public string BootstrapServers { get; set; } = default!;
        public string TopicName { get; set; } = default!;
    }
}

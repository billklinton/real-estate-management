namespace RealEstateManagement.Shareable.Configs
{
    public class AppConfig
    {
        public KafkaConfig KafkaConfig { get; set; } = new();
        public TokenConfig TokenConfig { get; set; } = new();
    }
}

namespace RealEstateManagement.Shareable.Exceptions
{
    public class DataInvalidException : ApplicationException
    {
        public IEnumerable<string> Errors { get; }
        
        public DataInvalidException(IDictionary<string, IEnumerable<string>> errors) : base("Invalid request")
        {
            Errors = errors.Select(m => $"{m.Key}: {string.Join(", ", m.Value)}");
        }
    }
}

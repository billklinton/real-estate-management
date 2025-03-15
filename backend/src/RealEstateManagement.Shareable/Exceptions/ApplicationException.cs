namespace RealEstateManagement.Shareable.Exceptions
{
    public class ApplicationException : Exception
    {
        public ApplicationException(string mensagem)
            : base(mensagem)
        {
        }

        public ApplicationException(string mensagem, Exception innerException)
            : base(mensagem, innerException)
        {
        }
    }
}

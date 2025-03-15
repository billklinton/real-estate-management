namespace RealEstateManagement.Shareable.Exceptions
{
    public class UnauthorizedException : ApplicationException
    {
        public UnauthorizedException(string mensagem) : base(mensagem)
        {
        }
    }
}

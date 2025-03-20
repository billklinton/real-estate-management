namespace RealEstateManagement.Shareable.Exceptions
{
    public class UnauthorizedException : ApplicationException
    {
        public UnauthorizedException(string mensagem = "Invalid user credentials!") : base(mensagem)
        {
        }
    }
}

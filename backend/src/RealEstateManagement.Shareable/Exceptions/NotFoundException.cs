namespace RealEstateManagement.Shareable.Exceptions
{
    public class NotFoundException : ApplicationException
    {
        public NotFoundException(string mensagem) : base(mensagem)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateManagement.Shareable.Exceptions
{
    public class InvalidArgumentException : ApplicationException
    {
        public InvalidArgumentException(string mensagem = "Invalid Arguments") : base(mensagem)
        {
        }
    }
}

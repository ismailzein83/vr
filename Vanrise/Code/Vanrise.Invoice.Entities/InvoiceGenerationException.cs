using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public class InvoiceGenerationException : Exception
    {
        public InvoiceGenerationException(string errorMessage)
            : base(errorMessage)
        {
        }
    }
}

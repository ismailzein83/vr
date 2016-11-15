using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public abstract class InvoiceGridActionSettings
    {
        public abstract string ActionTypeName { get;}
        public abstract Guid ConfigId { get; }
        public virtual Guid ActionId { get; set; }
    }
}

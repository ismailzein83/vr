using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities
{
    public abstract class PaymentMethod
    {
        public Guid ConfigId { get; set; }
        public abstract string GetDescription();
    }
}

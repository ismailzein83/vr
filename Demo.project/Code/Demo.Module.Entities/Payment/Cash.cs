using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities
{
    public class Cash : PaymentMethod
    {
        public double Amount { get; set; }
        public override string GetDescription()
        {
            return ("Amount : " + Amount);
        }
    }
}

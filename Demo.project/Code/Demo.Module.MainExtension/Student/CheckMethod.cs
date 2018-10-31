using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.MainExtension.Student
{
    class CheckMethod: PaymentMethod
    {
        public int Amount { get; set; }
        public int CheckNumber { get; set; }
        public override Guid ConfigId
        {
            get { return new Guid("{5401C633-B1B4-4965-A60D-28E56EEB5340}"); }
        }

        public override string GetPaymentMethodDescription()
        {
            return string.Format("Payment Method : Check,Check Number : {0},Amount : {1} ", CheckNumber, Amount);
        }
    
    
    }
}

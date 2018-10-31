using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.MainExtension.Student
{
    class CashMethod: PaymentMethod
    {
        public int Amount { get; set; }
        public int CurrencyID { get; set; }

        public string CurrencyName { get; set; }
        public override Guid ConfigId
        {
            get { return new Guid("{339DCA85-0441-46C3-A20E-AF63864442F5}"); }
        }

        public override string GetPaymentMethodDescription()
        {
            return string.Format("Payment Method : Cash ,Amount : {0}{1} ", Amount, CurrencyName);
        }
    
    
    }
}

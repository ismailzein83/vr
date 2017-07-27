using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities
{
    public class Card : PaymentMethod
    {
        public string Type { get; set; }
        public string CardNumber { get; set; }
        public string CVV { get; set; }
        public DateTime ExpirationDate { get; set; }
        public override string GetDescription()
        {
            string date = ExpirationDate.ToString("yyyy-MM-dd");
            return ("Type :" + Type + "\n" + "CardNumber :" + CardNumber + "\n" + "CVV :" + CVV + "\n" + "ExpirationDate :" + date);
        }
    }
}

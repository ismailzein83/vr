using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.BusinessEntity.Entities
{
    public class Currency
    {
        public string CurrencyID { get; set; }
        public string Name { get; set; }
        public string IsMainCurrency { get; set; }
        public string IsVisible { get; set; }
        public double LastRate { get; set; }
        public DateTime LastUpdated { get; set; }
        public int UserID { get; set; }
        public string CurrencyFullName { get; set; }
    }
}

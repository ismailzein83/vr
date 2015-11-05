using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CDRProcessing.Entities
{
    public class BillingMainCDR 
    {
        static BillingMainCDR()
        {
            BillingCDRBase BillingCDR = new BillingCDRBase();
            MainCost MainCost = new MainCost();
            MainSale MainSale = new MainSale();
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(BillingMainCDR), "BillingCDR", "MainCost", "MainSale");
        }
        public BillingCDRBase BillingCDR { get; set; }
        public MainCost MainCost { get; set; }
        public MainSale MainSale { get; set; }
    }
    public class MainCost
    {
        static MainCost()
        {
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(MainCost), "RateValue", "TotalNet", "CurrencyId");
        }
        public decimal RateValue { get; set; }

        public decimal TotalNet { get; set; }

        public int CurrencyId { get; set; }
    }
    public class MainSale
    {
        static MainSale()
        {
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(MainSale), "RateValue", "TotalNet", "CurrencyId");
        }
        public decimal RateValue { get; set; }

        public decimal TotalNet { get; set; }

        public int CurrencyId { get; set; }
    }
}

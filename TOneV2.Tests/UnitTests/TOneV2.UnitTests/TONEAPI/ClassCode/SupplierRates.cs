using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TONEAPI.ClassCode
{

  
    public class SupplierRates
    {

        public class Entity
        {
            public int SupplierRateId { get; set; }
            public int ZoneId { get; set; }
            public int PriceListId { get; set; }
            public int CurrencyId { get; set; }
            public double NormalRate { get; set; }
            public object OtherRates { get; set; }
            public DateTime BED { get; set; }
            public object EED { get; set; }
            public object SourceId { get; set; }
        }

        public class Datum
        {
            public Entity Entity { get; set; }
            public string SupplierZoneName { get; set; }
            public string CurrencyName { get; set; }
        }

        public class supplierrate
        {
            public string ResultKey { get; set; }
            public IList<Datum> Data { get; set; }
            public int TotalCount { get; set; }
        }

        public string getsupplierrate(RestClient rc , Uri ur, string token , string parameter)
        {
         
            string result = rc.MakeRequested(parameter, token);

            var obj = Json
          
            
            
            return result;


        }
    }
}
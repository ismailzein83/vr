using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.LCR.Entities
{
    public class RouteSupplierOption
    {
        static RouteSupplierOption()
        {
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(OptionInfo),
                "SupplierId", "SupplierZoneId", "Rate", "ServicesFlag");
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(RouteSupplierOption),
                "Info", "Percentage", "IsBlocked", "Priority");
            //Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(RouteSupplierOption),
            //    "SupplierId", "SupplierZoneId", "Rate", "ServicesFlag", "Percentage", "IsBlocked", "Priority");
        }

        public RouteSupplierOption(string supplierId, int supplierZoneId, decimal rate, short serviceFlag)
        {
            Info = new OptionInfo
            {
                SupplierId = supplierId,
                SupplierZoneId = supplierZoneId,
                Rate = rate,
                ServicesFlag = serviceFlag
            };
        }



        public OptionInfo Info { get; set; }

        public string SupplierId { get { return Info.SupplierId; } }

        public int SupplierZoneId { get { return Info.SupplierZoneId; } }

        public decimal Rate { get { return Info.Rate; } }

        public short ServicesFlag { get { return Info.ServicesFlag; } }
       
        public Int16? Percentage { get; set; }

        public bool IsBlocked { get; set; }

        public int Priority { get; set; }

        public RouteSupplierOption Clone()
        {
            return this.MemberwiseClone() as RouteSupplierOption;
        }
    }    

    public class OptionInfo
    {
        public string SupplierId { get; set; }

        public int SupplierZoneId { get; set; }

        public decimal Rate { get; set; }

        public short ServicesFlag { get; set; }

    }
}

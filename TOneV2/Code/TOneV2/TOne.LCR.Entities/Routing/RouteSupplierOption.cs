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
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(OptionSetting),
                "Percentage", "IsBlocked", "Priority");
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(RouteSupplierOption),
                "Info", "Setting");
            //Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(RouteSupplierOption),
            //    "SupplierId", "SupplierZoneId", "Rate", "ServicesFlag", "Percentage", "IsBlocked", "Priority");
        }

        public RouteSupplierOption(string supplierId, int supplierZoneId, decimal rate, short serviceFlag)
            : this(new OptionInfo
            {
                SupplierId = supplierId,
                SupplierZoneId = supplierZoneId,
                Rate = rate,
                ServicesFlag = serviceFlag
            })
        {
        }

        public RouteSupplierOption(OptionInfo optionInfo)
        {
            _info = optionInfo;
        }


        private OptionInfo _info;

        public OptionInfo Info
        {
            get { return _info; }
            set { _info = value; }
        }

        public string SupplierId { get { return _info.SupplierId; } }

        public int SupplierZoneId { get { return _info.SupplierZoneId; } }

        public decimal Rate { get { return _info.Rate; } }

        public short ServicesFlag { get { return _info.ServicesFlag; } }

        public OptionSetting Setting { get; set; }

    }    

    public class OptionInfo
    {
        public string SupplierId { get; set; }

        public int SupplierZoneId { get; set; }

        public decimal Rate { get; set; }

        public short ServicesFlag { get; set; }

    }

    public class OptionSetting
    {
        public Int16? Percentage { get; set; }

        public bool IsBlocked { get; set; }

        public int Priority { get; set; }

        public bool IgnoreRateCheck { get; set; }
    }
}

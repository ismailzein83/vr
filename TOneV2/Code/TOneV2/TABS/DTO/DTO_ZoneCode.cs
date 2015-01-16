using System;
using System.Collections.Generic;

namespace TABS.DTO
{
    [Serializable]
    public class DTO_ZoneCode
    {
        public Zone Zone { get; set; }
        public Code Code { get; set; }

        public DTO_ZoneCode(Code code)
        {
            this.Zone = code.Zone;
            this.Code = code;
        }

        public static List<DTO_ZoneCode> Get(IList<Code> zonesCodes)
        {
            List<DTO_ZoneCode> list = new List<DTO_ZoneCode>(zonesCodes.Count);
            if (zonesCodes != null)
                foreach (Code code in zonesCodes)
                    list.Add(new DTO_ZoneCode(code));
            return list;
        }

        public CarrierAccount Supplier { get { return (Zone != null) ? Zone.Supplier : null; } }
        public string SupplierName { get { return (Zone != null) ? Zone.Supplier.Name : null; } }
        public int? ZoneID { get { return (Zone != null) ? (int?)Zone.ZoneID : null; } }
        public string Zone_Name { get { return (Zone != null) ? Zone.Name : null; } }
        public DateTime? Zone_BED { get { return (Zone != null) ? (DateTime?)Zone.BeginEffectiveDate : null; } }
        public DateTime? Zone_EED { get { return (Zone != null) ? Zone.EndEffectiveDate : null; } }
        public bool? Zone_IsEffective { get { return (Zone != null) ? (bool?)Zone.IsEffective : null; } }

        public long? Code_ID { get { return (Code != null) ? (long?)Code.ID : null; } }
        public string Code_Value { get { return (Code != null) ? Code.Value : null; } }
        public DateTime? Code_BED { get { return (Code != null) ? (DateTime?)Code.BeginEffectiveDate : null; } }
        public DateTime? Code_EED { get { return (Code != null) ? Code.EndEffectiveDate : null; } }
        public bool? Code_IsEffective { get { return (Zone != null) ? (bool?)Code.IsEffective : null; } }
        public bool IsMobile { get { return Zone.IsMobile; } }
        public bool IsProper { get { return Zone.IsProper; } }
        public short ServicesFlag { get { return Zone.ServicesFlag; } }
    }
}

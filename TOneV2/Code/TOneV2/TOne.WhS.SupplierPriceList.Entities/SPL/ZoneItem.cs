using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.SupplierPriceList.Entities.SPL
{
    public interface IZone
    {
        long ZoneId { get; }

        string Name { get; }

        DateTime BED { get; }

        DateTime? EED { get; }
    }

    public class ZonesByName : Dictionary<string, List<IZone>>
    {

    }
    //public enum ItemStatus { NotChanged = 0, New = 1, Updated = 2 }

    public class NewZone : IZone
    {
        public long ZoneId { get; set; }

        public int CountryId { get; set; }

        public string Name { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }

        //public List<NewCode> NewCodes { get; set; }

        //public List<NewRate> NewRates { get; set; }
    }

    public class NewZonesByName : Dictionary<string, List<NewZone>>
    {

    }

    public class ImportedCode
    {
        public string Code { get; set; }

        public string ZoneName { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }
    }

    public class ImportedRate
    {
        public string ZoneName { get; set; }

        public Decimal NormalRate { get; set; }

        public Dictionary<int, Decimal> OtherRates { get; set; }

        public int? CurrencyId { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }
    }

    public class NewCode
    {
        public string Code { get; set; }

        public IZone Zone { get; set; }

        public int CodeGroupId { get; set; }
        
        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }
    }

    public class NewRate
    {
        public IZone Zone { get; set; }

        public Decimal NormalRate { get; set; }

        public Dictionary<int, Decimal> OtherRates { get; set; }

        public int? CurrencyId { get; set; }
        
        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }
    }

    public class ChangedZone
    {
        public long ZoneId { get; set; }

        public DateTime EED { get; set; }
    }

    public class ChangedCode
    {
        public long CodeId { get; set; }

        public DateTime EED { get; set; }
    }

    public class ChangedRate
    {
        public long RateId { get; set; }

        public DateTime EED { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOne.LCR.Entities
{
    public class CodeMatch
    {
        public string Code { get; set; }
        public string SupplierId { get; set; }
        public string SupplierCode { get; set; }
        public long SupplierCodeId { get; set; }
        public int SupplierZoneId { get; set; }
        public RateInfo SupplierRate { get; set; }
    }

    public class CodeMatchesBySupplierId : Dictionary<string, CodeMatch>
    {

    }

    /// <summary>
    /// Key Code
    /// </summary>
    public class CodeMatchesByCode : Dictionary<string, List<CodeMatch>>
    {

    }

    /// <summary>
    /// Key Supplier Zone Id
    /// </summary>
    public class CodeMatchesByZoneId : Dictionary<int, CodeMatchesByCode>
    {

    }

    public class SingleDestinationCodeMatches
    {
        public string RouteCode { get; set; }

        public CodeMatch SysCodeMatch { get; set; }

        public CodeMatchesBySupplierId CodeMatchesBySupplierId { get; set; }

        public List<CodeMatch> OrderedCodeMatches { get; set; }
    }


    public class CodeMatchComparer : IComparer<CodeMatch>
    {
        public int Compare(CodeMatch x, CodeMatch y)
        {
            return Decimal.Compare(x.SupplierRate.Rate, y.SupplierRate.Rate);
        }
    }

}

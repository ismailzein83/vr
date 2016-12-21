using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SalePLZoneChange
    {
        public int CountryId { get; set; }

        public string ZoneName { get; set; }

        public bool HasCodeChange { get; set; }

        public IEnumerable<int> CustomersHavingRateChange { get; set; }
    }

    public enum SalePLChangeType
    {
        [Description("None")]
        None,

        [Description("Code and Rate")]
        CodeAndRate,

        [Description("Rate")]
        Rate,

		[Description("Country and Rate")]
		CountryAndRate,
    }
}

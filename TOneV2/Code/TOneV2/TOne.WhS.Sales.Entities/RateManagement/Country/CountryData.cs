using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
    public class CountryData : Vanrise.BusinessProcess.Entities.IRuleTarget
    {
        public int CountryId { get; set; }
        public DateTime CountryBED { get; set; }
        public DateTime? CountryEED { get; set; }
        public bool IsCountryNew { get; set; }
        public Dictionary<long, DataByZone> ZoneDataByZoneId { get; set; }

        #region IRuleTarget
        public object Key
        {
            get { return CountryId; }
        }
        public string TargetType
        {
            get { return "CountryData"; }
        }
        #endregion
    }
}

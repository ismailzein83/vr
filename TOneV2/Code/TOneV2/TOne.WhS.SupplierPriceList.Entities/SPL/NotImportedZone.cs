using System;
using System.Collections.Generic;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.SupplierPriceList.Entities.SPL
{
    public class NotImportedZone : IRuleTarget
    {
        public long ZoneId { get; set; }
        public string ZoneName { get; set; }

        public int CountryId { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }

        public NotImportedZoneServiceGroup NotImportedZoneServiceGroup { get; set; }

        private List<NotImportedRate> _otherRates = new List<NotImportedRate>();

        public List<NotImportedRate> OtherRates
        {
            get
            {
                return this._otherRates;
            }
        }

        public NotImportedRate NormalRate { get; set; }

        public bool HasChanged { get; set; }

        public object Key
        {
            get { return this.ZoneName; }
        }

        public string TargetType
        {
            get { return "Zone"; }
        }
    }
}

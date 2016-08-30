using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.SupplierPriceList.Entities.SPL
{
    public class NotImportedZone : IRuleTarget
    {
        public string ZoneName { get; set; }

        public int CountryId { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }


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

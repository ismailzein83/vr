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


        private List<NotImportedRate> _otherSystemRates = new List<NotImportedRate>();

        public List<NotImportedRate> OtherSystemRates
        {
            get
            {
                return this._otherSystemRates;
            }
        }

        public ExistingRate NormalSystemRate { get; set; }

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

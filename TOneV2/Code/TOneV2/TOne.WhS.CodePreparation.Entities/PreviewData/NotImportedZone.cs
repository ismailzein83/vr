using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.CodePreparation.Entities
{
    public class NotImportedZone : IRuleTarget
    {
        public string ZoneName { get; set; }

        public int CountryId { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }

        private List<NotImportedRate> _notImportedNormalRates = new List<NotImportedRate>();

        public List<NotImportedRate> NotImportedNormalRates
        {
            get
            {
                return this._notImportedNormalRates;
            }
        }

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

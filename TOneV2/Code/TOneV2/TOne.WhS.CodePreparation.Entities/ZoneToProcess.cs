using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.CodePreparation.Entities.Processing
{
    public class ZoneToProcess : IRuleTarget
    {
        public ZoneToProcess()
        {
            CodesToAdd = new List<CodeToAdd>();
            CodesToMove = new List<CodeToMove>();
            CodesToClose = new List<CodeToClose>();
            AddedZones = new List<AddedZone>();
            ExistingZones = new List<ExistingZone>();

        }
        public string ZoneName { get; set; }

        public List<CodeToAdd> CodesToAdd { get; set; }

        public List<CodeToMove> CodesToMove { get; set; }

        public List<CodeToClose> CodesToClose { get; set; }

        public List<AddedZone> AddedZones { get; set; }

        public List<ExistingZone> ExistingZones { get; set; }


        private List<RateToAdd> _ratesToAdd = new List<RateToAdd>();

        public List<RateToAdd> RatesToAdd
        {
            get
            {
                return this._ratesToAdd;
            }
        }

        public string RecentZoneName { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }
        public ZoneChangeType ChangeType { get; set; }

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.NumberingPlan.Entities
{
    public class CountryToProcess : IRuleTarget
    {
        public int CountryId { get; set; }

        public List<CodeToAdd> CodesToAdd { get; set; }

        public List<CodeToMove> CodesToMove { get; set; }

        public List<CodeToClose> CodesToClose { get; set; }

        public List<ZoneToProcess> ZonesToProcess { get; set; }

        public object Key
        {
            get { return this.CountryId; }
        }

        public string TargetType
        {
            get { return "Country"; }
        }
    }

    public class AllCountriesToProcess : IRuleTarget
    {
        public IEnumerable<CountryToProcess> Countries { get; set; }

        public object Key
        {
            get { return null; }
        }

        public string TargetType
        {
            get { return "AllCountriesToProcess"; }
        }
    }
}

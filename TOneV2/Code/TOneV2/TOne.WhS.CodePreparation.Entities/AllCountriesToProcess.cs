using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.CodePreparation.Entities.Processing
{
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

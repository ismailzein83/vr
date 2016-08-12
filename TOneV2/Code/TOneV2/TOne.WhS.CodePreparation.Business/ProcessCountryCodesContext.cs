using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities.Processing;

namespace TOne.WhS.CodePreparation.Business
{
    public class ProcessCountryCodesContext : IProcessCountryCodesContext
    {
        public IEnumerable<CodeToAdd> CodesToAdd{ get; set; }

        public IEnumerable<CodeToMove> CodesToMove{ get; set; }

        public IEnumerable<CodeToClose> CodesToClose{ get; set; }

        public IEnumerable<ExistingCode> ExistingCodes{ get; set; }

        public IEnumerable<ExistingZone> ExistingZones { get; set; }

        public ZonesByName NewAndExistingZones { get; set; }

        public IEnumerable<AddedCode> NewCodes{ get; set; }

        public IEnumerable<ChangedCode> ChangedCodes{ get; set; }

        public IEnumerable<AddedZone> NewZones{ get; set; }
        
        public IEnumerable<ChangedZone> ChangedZones{ get; set; }

        public Dictionary<string,List<ExistingZone>> ClosedExistingZones { get; set; }

        public IEnumerable<ExistingCode> NotChangedCodes { get; set; }
    }
}

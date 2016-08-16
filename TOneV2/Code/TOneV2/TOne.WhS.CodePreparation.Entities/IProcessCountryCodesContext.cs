using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CodePreparation.Entities.Processing
{
    public interface IProcessCountryCodesContext
    {
        IEnumerable<CodeToAdd> CodesToAdd { get; }

        IEnumerable<CodeToMove> CodesToMove { get; }

        IEnumerable<CodeToClose> CodesToClose { get; }

        IEnumerable<ExistingCode> ExistingCodes { get; }

        IEnumerable<ExistingZone> ExistingZones { get; }

        ZonesByName NewAndExistingZones { set; }

        IEnumerable<AddedCode> NewCodes { get; set; }

        IEnumerable<ChangedCode> ChangedCodes { set; }

        IEnumerable<AddedZone> NewZones { set; }

        IEnumerable<ChangedZone> ChangedZones { set; }

        List<ExistingCode> NotChangedCodes { get; set; }

        Dictionary<string, List<ExistingZone>> ClosedExistingZones { get; set; }
    }
}

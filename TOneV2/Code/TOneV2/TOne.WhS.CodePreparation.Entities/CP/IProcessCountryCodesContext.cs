using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CodePreparation.Entities.CP.Processing
{
    public interface IProcessCountryCodesContext
    {
        IEnumerable<CodeToAdd> CodesToAdd { get; }

        IEnumerable<CodeToMove> CodesToMove { get; }

        IEnumerable<CodeToClose> CodesToClose { get; }

        IEnumerable<ExistingCode> ExistingCodes { get; }

        IEnumerable<ExistingZone> ExistingZones { get; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.SupplierPriceList.Entities.SPL
{
    public interface IProcessCountryCodesContext
    {
        IEnumerable<ImportedCode> ImportedCodes { get; }

        IEnumerable<ExistingCode> ExistingCodes { get; }

        IEnumerable<ExistingZone> ExistingZones { get; }

        DateTime DeletedCodesDate { get; }

        ZonesByName NewAndExistingZones { set; }

        IEnumerable<NewCode> NewCodes { set; }

        IEnumerable<ChangedCode> ChangedCodes { set; }

        IEnumerable<NewZone> NewZones { set; }

        IEnumerable<ChangedZone> ChangedZones { set; }
    }
}

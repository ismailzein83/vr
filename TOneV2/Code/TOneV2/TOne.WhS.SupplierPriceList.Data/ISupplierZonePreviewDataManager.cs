using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Entities;

namespace TOne.WhS.SupplierPriceList.Data
{
    public interface ISupplierZonePreviewDataManager : IDataManager
    {
        void Insert(int priceListId, IEnumerable<ZonePreview> zonePreviewList);
        Vanrise.Entities.BigResult<Entities.ZonePreview> GetZonePreviewFilteredFromTemp(Vanrise.Entities.DataRetrievalInput<Entities.SPLPreviewQuery> input);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Data;
using TOne.WhS.SupplierPriceList.Entities;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class SupplierZonePreviewManager
    {
        public void Insert(int priceListId, IEnumerable<ZonePreview> zonePreviewList)
        {
            ISupplierZonePreviewDataManager dataManager = SupPLDataManagerFactory.GetDataManager<ISupplierZonePreviewDataManager>();
            dataManager.Insert(priceListId, zonePreviewList);
        }
        public Vanrise.Entities.IDataRetrievalResult<ZonePreview> GetFilteredZonePreview(Vanrise.Entities.DataRetrievalInput<SPLPreviewQuery> input)
        {
            ISupplierZonePreviewDataManager dataManager = SupPLDataManagerFactory.GetDataManager<ISupplierZonePreviewDataManager>();
            return dataManager.GetZonePreviewFilteredFromTemp(input);
        }
    }
}

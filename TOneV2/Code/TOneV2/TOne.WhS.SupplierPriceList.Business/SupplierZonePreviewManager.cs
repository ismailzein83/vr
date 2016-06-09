using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Data;
using TOne.WhS.SupplierPriceList.Entities;
using Vanrise.Common;
using Vanrise.Entities;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class SupplierZonePreviewManager
    {

        public Vanrise.Entities.IDataRetrievalResult<ZoneRatePreviewDetail> GetFilteredZonePreview(Vanrise.Entities.DataRetrievalInput<SPLPreviewQuery> input)
        {
            ISupplierZonePreviewDataManager dataManager = SupPLDataManagerFactory.GetDataManager<ISupplierZonePreviewDataManager>();

            BigResult<ZoneRatePreviewDetail> zonesPreview = dataManager.GetZonePreviewFilteredFromTemp(input);
            BigResult<ZoneRatePreviewDetail> zonePreviewDetailResult = new BigResult<ZoneRatePreviewDetail>()
            {
                ResultKey = zonesPreview.ResultKey,
                TotalCount = zonesPreview.TotalCount,
                Data = zonesPreview.Data
            };

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, zonePreviewDetailResult);

        }

    }
}

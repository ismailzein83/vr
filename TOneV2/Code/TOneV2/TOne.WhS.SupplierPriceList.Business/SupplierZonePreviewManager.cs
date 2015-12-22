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
        public void Insert(int priceListId, IEnumerable<ZonePreview> zonePreviewList)
        {
            ISupplierZonePreviewDataManager dataManager = SupPLDataManagerFactory.GetDataManager<ISupplierZonePreviewDataManager>();
            dataManager.Insert(priceListId, zonePreviewList);
        }
        public Vanrise.Entities.IDataRetrievalResult<ZonePreviewDetail> GetFilteredZonePreview(Vanrise.Entities.DataRetrievalInput<SPLPreviewQuery> input)
        {
            ISupplierZonePreviewDataManager dataManager = SupPLDataManagerFactory.GetDataManager<ISupplierZonePreviewDataManager>();

            BigResult<ZonePreview> zonesPreview = dataManager.GetZonePreviewFilteredFromTemp(input);
            BigResult<ZonePreviewDetail> zonePreviewDetailResult = new BigResult<ZonePreviewDetail>()
            {
                ResultKey = zonesPreview.ResultKey,
                TotalCount = zonesPreview.TotalCount,
                Data = zonesPreview.Data.MapRecords(ZonePreviewDetailMapper)
            };

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, zonePreviewDetailResult);
        }

        private ZonePreviewDetail ZonePreviewDetailMapper(ZonePreview zonePreview)
        {
            ZonePreviewDetail zonePreviewDetail = new ZonePreviewDetail();

            zonePreviewDetail.Entity = zonePreview;
            var changeEnum = Utilities.GetEnumAttribute<ZoneChangeType, DescriptionAttribute>((ZoneChangeType)zonePreview.ChangeType);

            if (changeEnum.Description != null)
                zonePreviewDetail.ChangeTypeDecription = changeEnum.Description;
            else
                zonePreviewDetail.ChangeTypeDecription = changeEnum.ToString();
            return zonePreviewDetail;
        }
        

    }
}

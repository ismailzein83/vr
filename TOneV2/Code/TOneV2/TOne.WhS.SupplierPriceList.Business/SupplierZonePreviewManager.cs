using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Data;
using TOne.WhS.SupplierPriceList.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class SupplierZonePreviewManager
    {

        public Vanrise.Entities.IDataRetrievalResult<ZoneRatePreviewDetail> GetFilteredZonePreview(Vanrise.Entities.DataRetrievalInput<SPLPreviewQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new SupplierZoneRatePreviewRequestHandler());
        }


        #region Private Classes

        private class SupplierZoneRatePreviewRequestHandler : BigDataRequestHandler<SPLPreviewQuery, ZoneRatePreviewDetail, ZoneRatePreviewDetail>
        {
            public override ZoneRatePreviewDetail EntityDetailMapper(ZoneRatePreviewDetail entity)
            {
                SupplierZonePreviewManager manager = new SupplierZonePreviewManager();
                return manager.ZoneRatePreviewDetailMapper(entity);
            }

            public override IEnumerable<ZoneRatePreviewDetail> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<SPLPreviewQuery> input)
            {
                ISupplierZonePreviewDataManager dataManager = SupPLDataManagerFactory.GetDataManager<ISupplierZonePreviewDataManager>();
                return dataManager.GetFilteredZonePreview(input.Query);
            }
        }

        #endregion


        #region Private Mappers

        private ZoneRatePreviewDetail ZoneRatePreviewDetailMapper(ZoneRatePreviewDetail zoneRatePreviewDetail)
        {
            return zoneRatePreviewDetail;
        }

        #endregion


    }
}

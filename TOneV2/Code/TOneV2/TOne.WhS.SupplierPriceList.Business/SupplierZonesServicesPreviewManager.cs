using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Data;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class SupplierZonesServicesPreviewManager
    {
        public Vanrise.Entities.IDataRetrievalResult<ZoneServicePreview> GetFilteredZoneServicesPreview(Vanrise.Entities.DataRetrievalInput<SPLPreviewQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new SupplierZoneServicesPreviewRequestHandler());
        }


        #region Private Classes

        private class SupplierZoneServicesPreviewRequestHandler : BigDataRequestHandler<SPLPreviewQuery, ZoneServicePreview, ZoneServicePreview>
        {
            public override ZoneServicePreview EntityDetailMapper(ZoneServicePreview entity)
            {
                SupplierZonesServicesPreviewManager manager = new SupplierZonesServicesPreviewManager();
                return manager.zonesServicesPreviewDetailMapper(entity);
            }

            public override IEnumerable<ZoneServicePreview> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<SPLPreviewQuery> input)
            {
                ISupplierZoneServicePreviewDataManager dataManager = SupPLDataManagerFactory.GetDataManager<ISupplierZoneServicePreviewDataManager>();
                return dataManager.GetFilteredZonesServicesPreview(input.Query);
            }
        }

        #endregion


        #region Private Mappers
        private ZoneServicePreview zonesServicesPreviewDetailMapper(ZoneServicePreview zoneServicesPreview)
        {
            return zoneServicesPreview;
        }

        #endregion
    }
}

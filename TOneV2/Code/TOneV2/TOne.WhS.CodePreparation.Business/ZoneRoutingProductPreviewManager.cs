using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.CodePreparation.Data;
using TOne.WhS.CodePreparation.Entities;
using Vanrise.Common.Business;

namespace TOne.WhS.CodePreparation.Business
{
    public class ZoneRoutingProductPreviewManager
    {
        #region ctor/Local Variables

        SellingProductManager _sellingProductManager;
        CarrierAccountManager _carrierAccountManager;
        RoutingProductManager _routingProductManager;
        public ZoneRoutingProductPreviewManager()
        {
            _sellingProductManager = new SellingProductManager();
            _carrierAccountManager = new CarrierAccountManager();
            _routingProductManager = new RoutingProductManager();
        }

        #endregion


        #region Public Methods

        public Vanrise.Entities.IDataRetrievalResult<ZoneRoutingProductPreviewDetail> GetFilteredZonesRoutingProductsPreview(Vanrise.Entities.DataRetrievalInput<SPLPreviewQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new ZoneRoutingProductPreviewRequestHandler());
        }

        #endregion


        #region Private Classes

        private class ZoneRoutingProductPreviewRequestHandler : BigDataRequestHandler<SPLPreviewQuery, ZoneRoutingProductPreview, ZoneRoutingProductPreviewDetail>
        {
            public override ZoneRoutingProductPreviewDetail EntityDetailMapper(ZoneRoutingProductPreview entity)
            {
                ZoneRoutingProductPreviewManager manager = new ZoneRoutingProductPreviewManager();
                return manager.ZoneRoutingProductPreviewDetailMapper(entity);
            }

            public override IEnumerable<ZoneRoutingProductPreview> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<SPLPreviewQuery> input)
            {
                ISaleZoneRoutingProductPreviewDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<ISaleZoneRoutingProductPreviewDataManager>();
                return dataManager.GetFilteredZonesRoutingProductsPreview(input.Query);
            }
        }

        #endregion


        #region Private Mappers
        private ZoneRoutingProductPreviewDetail ZoneRoutingProductPreviewDetailMapper(ZoneRoutingProductPreview zoneRoutingProductPreview)
        {
            RoutingProductManager routingProductManager = new RoutingProductManager();
            ZoneRoutingProductPreviewDetail zoneRoutingProductPreviewDetail = new ZoneRoutingProductPreviewDetail();
            zoneRoutingProductPreviewDetail.Entity = zoneRoutingProductPreview;
            zoneRoutingProductPreviewDetail.OwnerName = GetOwnerName(zoneRoutingProductPreview.OwnerType,zoneRoutingProductPreview.OwnerId);
            zoneRoutingProductPreviewDetail.OwnerTypeDescription = Vanrise.Common.Utilities.GetEnumDescription(zoneRoutingProductPreview.OwnerType);
            zoneRoutingProductPreviewDetail.RoutingProductName = _routingProductManager.GetRoutingProductName(zoneRoutingProductPreview.RoutingProductId);
            zoneRoutingProductPreviewDetail.ChangeTypeDescription = Vanrise.Common.Utilities.GetEnumDescription(zoneRoutingProductPreview.ChangeType);
            zoneRoutingProductPreviewDetail.RoutingProductServicesIds = routingProductManager.GetDefaultServiceIds(zoneRoutingProductPreview.RoutingProductId);
            return zoneRoutingProductPreviewDetail;
        }

        #endregion


        #region Private Methods

        private string GetOwnerName(SalePriceListOwnerType ownerType, int ownerId)
        {
            return ownerType == BusinessEntity.Entities.SalePriceListOwnerType.SellingProduct ? _sellingProductManager.GetSellingProduct(ownerId).Name
                : _carrierAccountManager.GetCarrierAccountName(ownerId);
        }

        #endregion
    }
}

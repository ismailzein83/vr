using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SaleEntityServiceManager
    {
        public IEnumerable<SaleEntityDefaultService> GetDefaultServicesEffectiveAfter(SalePriceListOwnerType ownerType, int ownerId, DateTime minimumDate)
        {
            var dataManager = BEDataManagerFactory.GetDataManager<ISaleEntityServiceDataManager>();
            return dataManager.GetDefaultServicesEffectiveAfter(ownerType, ownerId, minimumDate);
        }

        public IEnumerable<SaleEntityZoneService> GetZoneServicesEffectiveAfter(SalePriceListOwnerType ownerType, int ownerId, DateTime minimumDate)
        {
            var dataManager = BEDataManagerFactory.GetDataManager<ISaleEntityServiceDataManager>();
            return dataManager.GetZoneServicesEffectiveAfter(ownerType, ownerId, minimumDate);
        }

        public Vanrise.Entities.IDataRetrievalResult<SaleEntityZoneServiceDetail> GetFilteredSaleEntityZoneServices(Vanrise.Entities.DataRetrievalInput<SaleEntityZoneServiceQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new SaleEntityZoneServiceRequestHandler());
        }
        public long ReserveIdRange(int numberOfIds)
        {
            long startingId;
            IDManager.Instance.ReserveIDRange(this.GetSaleEntityServiceType(), numberOfIds, out startingId);
            return startingId;
        }

        public int GetSaleEntityServiceTypeId()
        {
            return Vanrise.Common.Business.TypeManager.Instance.GetTypeId(this.GetSaleEntityServiceType());
        }

        public Type GetSaleEntityServiceType()
        {
            return this.GetType();
        }

        
        private class SaleEntityZoneServiceRequestHandler : BigDataRequestHandler<SaleEntityZoneServiceQuery, SaleEntityZoneServiceDetail, SaleEntityZoneServiceDetail>
        {
            public override SaleEntityZoneServiceDetail EntityDetailMapper(SaleEntityZoneServiceDetail entity)
            {
               // SaleEntityServiceManager manager = new SaleEntityServiceManager();
                return entity;
            }

            public override IEnumerable<SaleEntityZoneServiceDetail> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<SaleEntityZoneServiceQuery> input)
            {
                IEnumerable<SaleZone> saleZones = new SaleZoneManager().GetSaleZonesByOwner(input.Query.OwnerType,input.Query.OwnerId , input.Query.SellingNumberPlanId,input.Query.EffectiveOn);
                List<SaleEntityZoneServiceDetail> saleServiceDetails = new List<SaleEntityZoneServiceDetail>();

                var serviceLocator = new SaleEntityServiceLocator(new  SaleEntityServiceReadWithCache(input.Query.EffectiveOn));

               
                if (saleZones != null)
                {
                    var filteredSaleZone = saleZones.FindAllRecords(sz => input.Query.ZonesIds == null || input.Query.ZonesIds.Contains(sz.SaleZoneId));
                    if (input.Query.OwnerType == SalePriceListOwnerType.SellingProduct)
                    {
                        foreach (SaleZone saleZone in filteredSaleZone)
                        {
                            SaleEntityService service = serviceLocator.GetSellingProductZoneService(input.Query.OwnerId, saleZone.SaleZoneId);
                            if (service != null)
                            {
                                SaleEntityZoneServiceDetail saleEntityZoneServiceDetail = SaleEntityZoneServiceDetailMapper(service, service.Source != SaleEntityServiceSource.ProductZone, saleZone.SaleZoneId);
                                saleServiceDetails.Add(saleEntityZoneServiceDetail);
                            }

                        }
                    }
                    else
                    {
                        int? sellingProductId = null;

                        var customerSellingProductManager = new CustomerSellingProductManager();
                        sellingProductId = customerSellingProductManager.GetEffectiveSellingProductId(input.Query.OwnerId, input.Query.EffectiveOn, false);
                        if (!sellingProductId.HasValue)
                            throw new DataIntegrityValidationException(string.Format("Customer with Id {0} is not assigned to any selling product", input.Query.OwnerId));
                        foreach (SaleZone saleZone in filteredSaleZone)
                        {
                            SaleEntityService service = serviceLocator.GetCustomerZoneService(input.Query.OwnerId, sellingProductId.Value, saleZone.SaleZoneId);
                            if (service != null)
                            {
                                SaleEntityZoneServiceDetail saleEntityZoneServiceDetail = SaleEntityZoneServiceDetailMapper(service, service.Source != SaleEntityServiceSource.CustomerZone, saleZone.SaleZoneId);
                                saleServiceDetails.Add(saleEntityZoneServiceDetail);
                          
                            }
                        }
                    }
                }
                return saleServiceDetails;
            }

            private SaleEntityZoneServiceDetail SaleEntityZoneServiceDetailMapper(SaleEntityService saleEntityZoneService, bool IsServiceInherited, long ZoneId)
            {
                SaleEntityZoneServiceDetail detail = new SaleEntityZoneServiceDetail()
                {
                    Entity = saleEntityZoneService
                };
                SaleZone saleZone = new SaleZoneManager().GetSaleZone(ZoneId);
                if (saleZone == null)
                    throw new NullReferenceException();

                detail.ZoneName = saleZone.Name;
                detail.Services = saleEntityZoneService.Services.Select(x => x.ServiceId).ToList();
                detail.IsServiceInherited = IsServiceInherited;
                return detail;
            }
        }

    }
}

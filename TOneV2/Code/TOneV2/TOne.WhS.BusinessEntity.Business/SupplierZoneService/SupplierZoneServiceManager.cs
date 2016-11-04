using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.GenericData.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SupplierZoneServiceManager 
    {
        #region Public Methods

        public List<SupplierZoneService> GetSupplierZonesServicesEffectiveAfter(int supplierId, DateTime minimumDate)
        {
            ISupplierZoneServiceDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierZoneServiceDataManager>();
            return dataManager.GetSupplierZonesServicesEffectiveAfter(supplierId, minimumDate);
        }

        public Vanrise.Entities.IDataRetrievalResult<SupplierEntityServiceDetail> GetFilteredSupplierZoneServices(Vanrise.Entities.DataRetrievalInput<SupplierZoneServiceQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new SupplierZoneServiceRequestHandler());
        }


        public SupplierDefaultService GetSupplierDefaultServiceBySupplier(int supplierId, DateTime effectiveOn)
        {
            ISupplierZoneServiceDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierZoneServiceDataManager>();
            return dataManager.GetSupplierDefaultServiceBySupplier(supplierId, effectiveOn);
        }

        public long ReserveIDRange(int numberOfIDs)
        {
            long startingId;
            IDManager.Instance.ReserveIDRange(GetSupplierZoneServiceType(), numberOfIDs, out startingId);
            return startingId;
        }

        public bool HasSameServices(List<ZoneService> receivedServices, List<ZoneService> defaultServices)
        {
            if (receivedServices.Count != defaultServices.Count)
                return false;
            foreach (ZoneService zoneService in receivedServices)
            {
                if (!defaultServices.Any(item => item.ServiceId == zoneService.ServiceId))
                    return false;
            }

            return true;
        }

        public bool Insert(SupplierDefaultService supplierDefaultService)
        {
            supplierDefaultService.SupplierZoneServiceId = this.ReserveIDRange(1);

            ISupplierZoneServiceDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierZoneServiceDataManager>();
            return dataManager.Insert(supplierDefaultService); 

        }

        public bool CloseOverlappedDefaultService(long supplierZoneServiceId, SupplierDefaultService supplierDefaultService, DateTime effectiveDate)
        {
            supplierDefaultService.SupplierZoneServiceId = this.ReserveIDRange(1);

            ISupplierZoneServiceDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierZoneServiceDataManager>();
            return dataManager.CloseOverlappedDefaultService(supplierZoneServiceId, supplierDefaultService, effectiveDate);
        }

        public int GetSupplierZoneServiceTypeId()
        {
            return Vanrise.Common.Business.TypeManager.Instance.GetTypeId(this.GetSupplierZoneServiceType());
        }

        public Type GetSupplierZoneServiceType()
        {
            return this.GetType();
        }


        #endregion


        #region Private Methods
        private SupplierEntityServiceDetail SupplierEntityServiceDetailMapper(SupplierEntityService supplierEntityService)
        {
            SupplierEntityServiceDetail detail = new SupplierEntityServiceDetail()
            {
                Entity = supplierEntityService
               
            };

            return detail;
        }
        private class SupplierZoneServiceRequestHandler : BigDataRequestHandler<SupplierZoneServiceQuery, SupplierEntityServiceDetail, SupplierEntityServiceDetail>
        {
            public override SupplierEntityServiceDetail EntityDetailMapper(SupplierEntityServiceDetail entity)
            {
                return entity;
            }

            public override IEnumerable<SupplierEntityServiceDetail> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<SupplierZoneServiceQuery> input)
            {
                SupplierZoneManager zoneManager = new SupplierZoneManager();
                IEnumerable<SupplierZone> supplierZones = zoneManager.GetSupplierZones(input.Query.SupplierId, input.Query.EffectiveOn);

                if(input.Query.ZoneIds != null)
                     supplierZones = supplierZones.FindAllRecords(item => input.Query.ZoneIds.Contains(item.SupplierZoneId));

                SupplierZoneServiceLocator zoneServiceLocator = new SupplierZoneServiceLocator(new SupplierZoneServiceReadAllWithCache(input.Query.EffectiveOn));
                
                List<SupplierEntityServiceDetail> supplierEntityServicesDetail = new List<SupplierEntityServiceDetail>();
               
                foreach (SupplierZone supplierZone in supplierZones)
                {
                    SupplierEntityServiceDetail supplierEntityServiceDetail = new SupplierEntityServiceDetail();
                    supplierEntityServiceDetail.Entity = zoneServiceLocator.GetSupplierZoneServices(supplierZone.SupplierId, supplierZone.SupplierZoneId, input.Query.EffectiveOn);
                     supplierEntityServiceDetail.ZoneName = supplierZone.Name;
                     supplierEntityServiceDetail.Services = supplierEntityServiceDetail.Entity.Services.Select(x => x.ServiceId).ToList();         
                    supplierEntityServicesDetail.Add(supplierEntityServiceDetail);
                }
                return supplierEntityServicesDetail;
            }
        }
        #endregion
    }
}

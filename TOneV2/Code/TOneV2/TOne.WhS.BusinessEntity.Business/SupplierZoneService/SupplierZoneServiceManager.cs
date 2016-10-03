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

        public Vanrise.Entities.IDataRetrievalResult<SupplierZoneServiceDetail> GetFilteredSupplierZoneServices(Vanrise.Entities.DataRetrievalInput<SupplierZoneServiceQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new SupplierZoneServiceRequestHandler());
        }

        public long ReserveIDRange(int numberOfIDs)
        {
            long startingId;
            IDManager.Instance.ReserveIDRange(GetSupplierZoneServiceType(), numberOfIDs, out startingId);
            return startingId;
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
        private SupplierZoneServiceDetail SupplierZoneServiceDetailMapper(SupplierZoneService supplierZoneService)
        {
            SupplierZoneServiceDetail detail = new SupplierZoneServiceDetail()
            {
                Entity = supplierZoneService
            };

            SupplierZone supplierZone = new SupplierZoneManager().GetSupplierZone(supplierZoneService.ZoneId);
            if (supplierZone == null)
                throw new NullReferenceException();

            detail.SupplierZoneName = supplierZone.Name;
            detail.Services = supplierZoneService.EffectiveServices.Select(x => x.ServiceId).ToList();         

            return detail;
        }
        private class SupplierZoneServiceRequestHandler : BigDataRequestHandler<SupplierZoneServiceQuery, SupplierZoneService, SupplierZoneServiceDetail>
        {
            public override SupplierZoneServiceDetail EntityDetailMapper(SupplierZoneService entity)
            {
                SupplierZoneServiceManager manager = new SupplierZoneServiceManager();
                return manager.SupplierZoneServiceDetailMapper(entity);
            }

            public override IEnumerable<SupplierZoneService> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<SupplierZoneServiceQuery> input)
            {
                ISupplierZoneServiceDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierZoneServiceDataManager>();
                return dataManager.GetFilteredSupplierZoneServices(input.Query);
            }
        }
        #endregion
    }
}

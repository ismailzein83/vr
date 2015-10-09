using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Business
{
    public class CarrierAccountManager
    {
        public Vanrise.Entities.IDataRetrievalResult<CarrierAccountDetail> GetFilteredCarrierAccounts(Vanrise.Entities.DataRetrievalInput<CarrierAccountQuery> input)
        {
            ICarrierAccountDataManager dataManager = BEDataManagerFactory.GetDataManager<ICarrierAccountDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetFilteredCarrierAccounts(input));
        }
        public List<CarrierAccount> GetAllCustomers()
        {
            throw new NotImplementedException();
        }
        public CarrierAccountDetail GetCarrierAccount(int carrierAccountId)
        {
            ICarrierAccountDataManager dataManager = BEDataManagerFactory.GetDataManager<ICarrierAccountDataManager>();
            return dataManager.GetCarrierAccount(carrierAccountId);
        }
        public List<CarrierAccountInfo> GetCarrierAccounts(bool getCustomers, bool getSuppliers)
        {
            ICarrierAccountDataManager dataManager = BEDataManagerFactory.GetDataManager<ICarrierAccountDataManager>();
            return dataManager.GetCarrierAccounts(getCustomers, getSuppliers);
        }
        public List<Vanrise.Entities.TemplateConfig> GetCustomersGroupTemplates()
        {
            TemplateConfigManager manager = new TemplateConfigManager();
            return manager.GetTemplateConfigurations(Constants.CustomerGroupConfigType);
        }

        public List<int> GetCustomerIds(int customersGroupConfigId, CustomerGroupSettings customerGroupSettings)
        {
            TemplateConfigManager templateConfigManager = new TemplateConfigManager();
            CustomerGroupBehavior customerGroupBehavior = templateConfigManager.GetBehavior<CustomerGroupBehavior>(customersGroupConfigId);
            if (customerGroupBehavior != null)
                return customerGroupBehavior.GetCustomerIds(customerGroupSettings);
            else
                return null;
        }

        public List<Vanrise.Entities.TemplateConfig> GetSupplierGroupTemplates()
        {
            TemplateConfigManager manager = new TemplateConfigManager();
            return manager.GetTemplateConfigurations(Constants.SupplierGroupConfigType);
        }

        public List<Vanrise.Entities.TemplateConfig> GetCustomerGroupTemplates()
        {
            TemplateConfigManager manager = new TemplateConfigManager();
            return manager.GetTemplateConfigurations(Constants.CustomerGroupConfigType);
        }
    }
}

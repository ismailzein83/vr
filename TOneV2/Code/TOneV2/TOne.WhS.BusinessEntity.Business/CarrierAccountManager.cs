using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Business
{
    public class CarrierAccountManager
    {
        public List<CarrierAccount> GetAllCustomers()
        {
            throw new NotImplementedException();
        }

        public List<Vanrise.Entities.TemplateConfig> GetCustomersGroupTemplates()
        {
            TemplateConfigManager manager = new TemplateConfigManager();
            return manager.GetTemplateConfigurations(Constants.CustomersGroupConfigType);
        }

        private CustomerGroupBehavior GetCustomersGroupBehavior(int configId)
        {
            var allTemplates = GetCustomersGroupTemplates();
            if(allTemplates != null)
            {
                Vanrise.Entities.TemplateConfig templateConfig = allTemplates.FirstOrDefault(itm => itm.TemplateConfigID == configId);
                if(templateConfig != null)
                {
                    Type t = Type.GetType(templateConfig.BehaviorFQTN);
                    return Activator.CreateInstance(t) as CustomerGroupBehavior;
                }
            }
            return null;
        }

        public List<int> GetCustomerIds(int customersGroupConfigId, CustomerGroupSettings customerGroupSettings)
        {
            CustomerGroupBehavior customerGroupBehavior = GetCustomersGroupBehavior(customersGroupConfigId);
            if (customerGroupBehavior != null)
                return customerGroupBehavior.GetCustomerIds(customerGroupSettings);
            else
                return null;
        }
    }
}

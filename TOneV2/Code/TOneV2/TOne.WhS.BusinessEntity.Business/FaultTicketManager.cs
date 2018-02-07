using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.GenericData.Business;

namespace TOne.WhS.BusinessEntity.Business
{
    public class FaultTicketManager
    {
        public FaultTicketManager()
        {
        }
        public CustomerFaultTicketSettingsDetails GetCustomerFaultTicketDetails(CustomerFaultTicketSettingsInput customerFaultTicketInput)
        {
             List<CustomerFaultTicketDescriptionSettingDetails> descriptionSettingDetails = new List<CustomerFaultTicketDescriptionSettingDetails>();
            CustomerFaultTicketSettingsDetails customerFaultTicketDetail = new CustomerFaultTicketSettingsDetails();
            GenericBusinessEntityManager genericBusinessEntityManager = new GenericBusinessEntityManager();
            if (customerFaultTicketInput != null)
            {
                foreach (var descriptionSetting in customerFaultTicketInput.DescriptionSettings)
                {
                    var reasonDescription = genericBusinessEntityManager.GetGenericBusinessEntityName(descriptionSetting.ReasonId, customerFaultTicketInput.ReasonBEDefinitionId);
                    var descriptionSettingDetail = new CustomerFaultTicketDescriptionSettingDetails()
                    {
                        CodeNumber = descriptionSetting.CodeNumber,
                        ReasonId = descriptionSetting.ReasonId,
                        ReasonDescription = reasonDescription,
                        InternationalReleaseCodeId = descriptionSetting.InternationalReleaseCodeId,
                    };
                    if(descriptionSetting.InternationalReleaseCodeId != null)
                    {
                        var internationalReleaseCodeDescription = genericBusinessEntityManager.GetGenericBusinessEntityName(descriptionSetting.InternationalReleaseCodeId, customerFaultTicketInput.ReleaseCodeBEDefinitionId);
                        descriptionSettingDetail.InternationalReleaseCodeDescription = internationalReleaseCodeDescription;
                    }
                  
                    descriptionSettingDetails.Add(descriptionSettingDetail);
                }
                customerFaultTicketDetail.DescriptionSettings = descriptionSettingDetails;
            }
            return customerFaultTicketDetail;
        }
        public CustomerFaultTicketSettingsDetails GetSupplierFaultTicketDetails(CustomerFaultTicketSettingsInput customerFaultTicketInput)
        {
            List<CustomerFaultTicketDescriptionSettingDetails> descriptionSettingDetails = new List<CustomerFaultTicketDescriptionSettingDetails>();
            CustomerFaultTicketSettingsDetails customerFaultTicketDetail = new CustomerFaultTicketSettingsDetails();
            GenericBusinessEntityManager genericBusinessEntityManager = new GenericBusinessEntityManager();
            if (customerFaultTicketInput != null)
            {
                foreach (var descriptionSetting in customerFaultTicketInput.DescriptionSettings)
                {
                    var reasonDescription = genericBusinessEntityManager.GetGenericBusinessEntityName(descriptionSetting.ReasonId, customerFaultTicketInput.ReasonBEDefinitionId);
                    var descriptionSettingDetail = new CustomerFaultTicketDescriptionSettingDetails()
                    {
                        CodeNumber = descriptionSetting.CodeNumber,
                        ReasonId = descriptionSetting.ReasonId,
                        ReasonDescription = reasonDescription,
                        InternationalReleaseCodeId = descriptionSetting.InternationalReleaseCodeId,
                    };
                    if (descriptionSetting.InternationalReleaseCodeId != null)
                    {
                        var internationalReleaseCodeDescription = genericBusinessEntityManager.GetGenericBusinessEntityName(descriptionSetting.InternationalReleaseCodeId, customerFaultTicketInput.ReleaseCodeBEDefinitionId);
                        descriptionSettingDetail.InternationalReleaseCodeDescription = internationalReleaseCodeDescription;
                    }

                    descriptionSettingDetails.Add(descriptionSettingDetail);
                }
                customerFaultTicketDetail.DescriptionSettings = descriptionSettingDetails;
            }
            return customerFaultTicketDetail;
        }

    }
}

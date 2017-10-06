using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Invoice.Data;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.Business
{
    public class InvoiceAccountManager
    {
        public bool TryAddInvoiceAccount(VRInvoiceAccount invoiceAccount, out long insertedId)
        {
            IInvoiceAccountDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceAccountDataManager>();
           return dataManager.InsertInvoiceAccount(invoiceAccount, out insertedId);
        }

        public bool TryAddInvoiceAccount(Guid invoiceTypeId, string partnerId, DateTime? bed, DateTime? eed, VRAccountStatus status, bool isDeleted)
        {
            long insertedId = -1;
            return TryAddInvoiceAccount(new VRInvoiceAccount
            {
                BED = bed,
                Status = status,
                EED = eed,
                InvoiceTypeId = invoiceTypeId,
                IsDeleted = isDeleted,
                PartnerId = partnerId
            }, out insertedId);
        }
       
        public bool TryUpdateInvoiceAccountStatus(Guid invoiceTypeId, string partnerId, VRAccountStatus status, bool isDeleted)
        {
            IInvoiceAccountDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceAccountDataManager>();
            dataManager.TryUpdateInvoiceAccountStatus(invoiceTypeId, partnerId, status, isDeleted);
            return true;//no validation is needed
        }
        public bool TryUpdateInvoiceAccountEffectiveDate(List<Guid> invoiceTypeIds, string partnerId, DateTime? bed, DateTime? eed, out string errorMessage)
        {
            errorMessage = null;
            if (invoiceTypeIds != null)
            {
                foreach (var invoiceTypeId in invoiceTypeIds)
                {
                    if(!ValidateUpdateInvoiceAccountEffectiveDate(invoiceTypeId, partnerId, bed, eed, out  errorMessage))
                    {
                        return false;
                    }
                }
                IInvoiceAccountDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceAccountDataManager>();
                foreach (var invoiceTypeId in invoiceTypeIds)
                {
                    dataManager.TryUpdateInvoiceAccountEffectiveDate(invoiceTypeId, partnerId, bed, eed);
                }
            }
            return true;
        }
        public bool TryUpdateInvoiceAccountEffectiveDate(Guid invoiceTypeId, string partnerId, DateTime? bed, DateTime? eed, out string errorMessage)
        {
            var validationResult = ValidateUpdateInvoiceAccountEffectiveDate( invoiceTypeId,  partnerId, bed,  eed, out  errorMessage);
            if(validationResult)
            {
                IInvoiceAccountDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceAccountDataManager>();
                dataManager.TryUpdateInvoiceAccountEffectiveDate(invoiceTypeId, partnerId, bed, eed);
            }
            return validationResult;
        }

        bool ValidateUpdateInvoiceAccountEffectiveDate(Guid invoiceTypeId, string partnerId, DateTime? bed, DateTime? eed, out string errorMessage)
        {
            InvoiceManager invoiceManager = new InvoiceManager();
            errorMessage = null;
            var populatedPeriod = invoiceManager.GetInvoicesPopulatedPeriod(invoiceTypeId, partnerId);
            if(populatedPeriod != null)
            {     var systemDateTimeFormat = new GeneralSettingsManager().GetDateFormat(); 
                if (bed.HasValue && populatedPeriod.FromDate.HasValue && populatedPeriod.FromDate.Value < bed.Value)
                {
                    errorMessage = string.Format("BED should be less than first invoice date {0}", populatedPeriod.FromDate.Value.ToString(systemDateTimeFormat));
                    return false;
                }
                if (eed.HasValue && populatedPeriod.ToDate.HasValue && populatedPeriod.ToDate.Value > eed.Value)
                {
                    errorMessage = string.Format("EED should be greater than last invoice date {0}", populatedPeriod.ToDate.Value.ToString(systemDateTimeFormat));
                    return false;
                }
            }
           return true;
        }

        public List<VRInvoiceAccount> GetInvoiceAccountsByPartnerIds(Guid invoiceTypeId, IEnumerable<string> partnerIds)
        {
            IInvoiceAccountDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceAccountDataManager>();
            return dataManager.GetInvoiceAccountsByPartnerIds(invoiceTypeId,partnerIds);
        }
    }
}

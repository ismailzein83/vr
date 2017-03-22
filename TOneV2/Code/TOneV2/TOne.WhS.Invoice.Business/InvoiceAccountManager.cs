using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Invoice.Entities;
using Vanrise.Common;

namespace TOne.WhS.Invoice.Business
{
    public class InvoiceAccountManager
    {
        #region Public Methods

        public CarrierInvoiceAccountData GetCustCarrierInvoiceByFinAccId(int invoiceAccountId)
        {
            CarrierInvoiceAccountData carrierInvoiceAccountData = GetCachedCustCarrierInvoicesByFinAccId().GetRecord(invoiceAccountId);
            carrierInvoiceAccountData.ThrowIfNull("carrierInvoiceAccountData", invoiceAccountId);
            return carrierInvoiceAccountData;
        }
        public CarrierInvoiceAccountData GetSuppCarrierInvoiceByFinAccId(int invoiceAccountId)
        {
            CarrierInvoiceAccountData carrierInvoiceAccountData = GetCachedSuppCarrierInvoicesByFinAccId().GetRecord(invoiceAccountId);
            carrierInvoiceAccountData.ThrowIfNull("carrierInvoiceAccountData", invoiceAccountId);
            return carrierInvoiceAccountData;
        }
        public bool TryGetCustAccInvoiceAccountData(int customerAccountId, DateTime effectiveOn, out CarrierInvoiceAccountData invoiceAccountData)
        {
            IOrderedEnumerable<CarrierInvoiceAccountData> carrierInvoiceAccounts = GetCachedCustCarrierInvoicesByCarrAccId().GetRecord(customerAccountId);
            if (carrierInvoiceAccounts != null)
            {
                foreach (var acc in carrierInvoiceAccounts)
                {
                    if (acc.BED <= effectiveOn && acc.EED.VRGreaterThan(effectiveOn))
                    {
                        invoiceAccountData = acc;
                        return true;
                    }
                }
            }
            invoiceAccountData = null;
            return false;
        }
        public bool TryGetCustProfInvoiceAccountData(int customerProfileId, DateTime effectiveOn, out List<CarrierInvoiceAccountData> invoiceAccountsData)
        {
            throw new NotImplementedException();
        }
        public bool TryGetSuppAccInvoiceAccountData(int supplierAccountId, DateTime effectiveOn, out CarrierInvoiceAccountData invoiceAccountData)
        {
            IOrderedEnumerable<CarrierInvoiceAccountData> carrierInvoiceAccounts = GetCachedSuppCarrierInvoicesByCarrAccId().GetRecord(supplierAccountId);
            if (carrierInvoiceAccounts != null)
            {
                foreach (var acc in carrierInvoiceAccounts)
                {
                    if (acc.BED <= effectiveOn && acc.EED.VRGreaterThan(effectiveOn))
                    {
                        invoiceAccountData = acc;
                        return true;
                    }
                }
            }
            invoiceAccountData = null;
            return false;
        }
        public bool TryGetSuppProfInvoiceAccountData(int supplierProfileId, DateTime effectiveOn, out List<CarrierInvoiceAccountData> invoiceAccountsData)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Private Methods
        
        Dictionary<int, InvoiceAccount> GetCachedInvoiceAccounts()
        {
            throw new NotImplementedException();
        }

        Dictionary<Guid, List<InvoiceAccount>> GetCachedInvoiceAccountsByAccBalTypeId()
        {
            var invoiceAccountsByType = new Dictionary<Guid, List<InvoiceAccount>>();

            Dictionary<int, InvoiceAccount> cachedInvoiceAccounts = GetCachedInvoiceAccounts();

            if (cachedInvoiceAccounts != null)
            {
                foreach (InvoiceAccount invoiceAccount in cachedInvoiceAccounts.Values)
                {
                    List<InvoiceAccount> invoiceAccounts;

                    if (!invoiceAccountsByType.TryGetValue(invoiceAccount.Settings.InvoiceTypeId, out invoiceAccounts))
                    {
                        invoiceAccounts = new List<InvoiceAccount>();
                        invoiceAccountsByType.Add(invoiceAccount.Settings.InvoiceTypeId, invoiceAccounts);
                    }

                    invoiceAccounts.Add(invoiceAccount);
                }
            }

            return invoiceAccountsByType;
        }

        Dictionary<int, CarrierInvoiceAccountData> GetCachedCustCarrierInvoicesByFinAccId()
        {
            throw new NotImplementedException();
        }

        Dictionary<int, CarrierInvoiceAccountData> GetCachedSuppCarrierInvoicesByFinAccId()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// should return a list of applicable Invoice Account Data for each customer account ordered by BED desc
        /// </summary>
        /// <returns></returns>
        Dictionary<int, IOrderedEnumerable<CarrierInvoiceAccountData>> GetCachedCustCarrierInvoicesByCarrAccId()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// should return a list of applicable Invoice Account Data for each supplier account ordered by BED desc
        /// </summary>
        /// <returns></returns>
        Dictionary<int, IOrderedEnumerable<CarrierInvoiceAccountData>> GetCachedSuppCarrierInvoicesByCarrAccId()
        {
            throw new NotImplementedException();
        }
        
        #endregion
    }
}

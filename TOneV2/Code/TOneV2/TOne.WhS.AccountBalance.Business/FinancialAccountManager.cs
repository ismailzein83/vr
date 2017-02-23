using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.AccountBalance.Entities;
using Vanrise.Common;

namespace TOne.WhS.AccountBalance.Business
{
    public class FinancialAccountManager
    {
        #region Public Methods

        public FinancialAccount GetFinancialAccount(int financialAccountId)
        {
            throw new NotImplementedException();
        }

        public CarrierFinancialAccountData GetCustCarrierFinancialByFinAccId(int financialAccountId)
        {
            CarrierFinancialAccountData carrierFinancialAccountData = GetCachedCustCarrierFinancialsByFinAccId().GetRecord(financialAccountId);
            carrierFinancialAccountData.ThrowIfNull("carrierFinancialAccountData", financialAccountId);
            return carrierFinancialAccountData;
        }

        public CarrierFinancialAccountData GetSuppCarrierFinancialByFinAccId(int financialAccountId)
        {
            CarrierFinancialAccountData carrierFinancialAccountData = GetCachedSuppCarrierFinancialsByFinAccId().GetRecord(financialAccountId);
            carrierFinancialAccountData.ThrowIfNull("carrierFinancialAccountData", financialAccountId);
            return carrierFinancialAccountData;
        }

        public bool TryGetCustAccFinancialAccountData(int customerAccountId, DateTime effectiveOn, out CarrierFinancialAccountData financialAccountData)
        {
            IOrderedEnumerable<CarrierFinancialAccountData> carrierFinancialAccounts = GetCachedCustCarrierFinancialsByCarrAccId().GetRecord(customerAccountId);
            if(carrierFinancialAccounts != null)
            {
                foreach(var acc in carrierFinancialAccounts)
                {
                    if(acc.BED <= effectiveOn && acc.EED.VRGreaterThan(effectiveOn))
                    {
                        financialAccountData = acc;
                        return true;
                    }
                }
            }
            financialAccountData = null;
            return false;
        }

        public bool TryGetSuppAccFinancialAccountData(int supplierAccountId, DateTime effectiveOn, out CarrierFinancialAccountData financialAccountData)
        {
            IOrderedEnumerable<CarrierFinancialAccountData> carrierFinancialAccounts = GetCachedSuppCarrierFinancialsByCarrAccId().GetRecord(supplierAccountId);
            if (carrierFinancialAccounts != null)
            {
                foreach (var acc in carrierFinancialAccounts)
                {
                    if (acc.BED <= effectiveOn && acc.EED.VRGreaterThan(effectiveOn))
                    {
                        financialAccountData = acc;
                        return true;
                    }
                }
            }
            financialAccountData = null;
            return false;
        }

        #endregion

        #region Private Methods

        Dictionary<int, FinancialAccount> GetCachedFinancialAccounts()
        {
            throw new NotImplementedException();
        }

        Dictionary<Guid, List<FinancialAccount>> GetCachedFinancialAccountsByAccBalTypeId()
        {
            throw new NotImplementedException();
        }

        Dictionary<int, CarrierFinancialAccountData> GetCachedCustCarrierFinancialsByFinAccId()
        {
            throw new NotImplementedException();
        }

        Dictionary<int, CarrierFinancialAccountData> GetCachedSuppCarrierFinancialsByFinAccId()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// should return a list of applicable Financial Account Data for each customer account ordered by BED desc
        /// </summary>
        /// <returns></returns>
        Dictionary<int, IOrderedEnumerable<CarrierFinancialAccountData>> GetCachedCustCarrierFinancialsByCarrAccId()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// should return a list of applicable Financial Account Data for each supplier account ordered by BED desc
        /// </summary>
        /// <returns></returns>
        Dictionary<int, IOrderedEnumerable<CarrierFinancialAccountData>> GetCachedSuppCarrierFinancialsByCarrAccId()
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}

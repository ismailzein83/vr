using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Analytics.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace CP.WhS.Business
{
    public class RepeatedNumbersManager
    {
        public IDataRetrievalResult<RepeatedNumberDetail> GetFilteredBlockedAttempts(DataRetrievalInput<RepeatedNumberQuery> query)
        {
            var connectionSettings = new PortalConnectionManager().GetWhSConnectionSettings();
            var clonedInput = Utilities.CloneObject<DataRetrievalInput<RepeatedNumberQuery>>(query);
            clonedInput.IsAPICall = true;
            WhSCarrierAccountBEManager whSCarrierAccountBEManager = new WhSCarrierAccountBEManager();
            var accessibleCustomers = whSCarrierAccountBEManager.GetRemoteCarrierAccountsInfo(new Entities.ClientAccountInfoFilter() { GetCustomers = true });
            var accessibleSuppliers = whSCarrierAccountBEManager.GetRemoteCarrierAccountsInfo(new Entities.ClientAccountInfoFilter() { GetSuppliers = true });
            if (clonedInput.Query.Filter != null)
            {
                if (clonedInput.Query.Filter.CustomerIds != null && query.Query.Filter.CustomerIds.Count > 0)
                {
                    foreach (var customerId in query.Query.Filter.CustomerIds)
                    {
                        if (accessibleCustomers.FindRecord(x => x.AccountId == customerId) == null)
                            return null;
                    }
                }
                else if (clonedInput.Query.Filter.SupplierIds != null && query.Query.Filter.SupplierIds.Count > 0)
                {
                    foreach (var supplierId in query.Query.Filter.SupplierIds)
                    {
                        if (accessibleSuppliers.FindRecord(x => x.AccountId == supplierId) == null)
                            return null;
                    }
                }
                else
                {
                    clonedInput.Query.Filter.CustomerIds = accessibleCustomers.Select(x => x.AccountId).ToList();
                    clonedInput.Query.Filter.SupplierIds = accessibleSuppliers.Select(x => x.AccountId).ToList();
                }
            }
            else
            {
                clonedInput.Query.Filter = new RepeatedNumberFilter()
                {
                    CustomerIds = accessibleCustomers.Select(x => x.AccountId).ToList(),
                    SupplierIds = accessibleSuppliers.Select(x => x.AccountId).ToList()
                };
            }
            if (clonedInput.DataRetrievalResultType == DataRetrievalResultType.Excel)
            {
                return connectionSettings.Post<DataRetrievalInput<RepeatedNumberQuery>, RemoteExcelResult<RepeatedNumberDetail>>("/api/WhS_Analytics/RepeatedNumber/GetAllFilteredRepeatedNumbers", clonedInput);
            }
            else if (clonedInput.DataRetrievalResultType == DataRetrievalResultType.Normal)
            {
                return connectionSettings.Post<DataRetrievalInput<RepeatedNumberQuery>, BigResult<RepeatedNumberDetail>>("/api/WhS_Analytics/RepeatedNumber/GetAllFilteredRepeatedNumbers", clonedInput);
            }
            return null;
        }
    }
}

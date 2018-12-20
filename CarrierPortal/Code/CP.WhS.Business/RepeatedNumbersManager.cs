using CP.WhS.Entities;
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
        public IDataRetrievalResult<RepeatedNumberDetail> GetFilteredBlockedAttempts(DataRetrievalInput<ClientRepeatedNumberQuery> query)
        {
            var connectionSettings = new PortalConnectionManager().GetWhSConnectionSettings();
            var cdrType = CDRType.All;
            if (query.Query.CDRType == ClientCDRType.Failed)
                cdrType = CDRType.Failed;
            else if (query.Query.CDRType == ClientCDRType.Invalid)
                cdrType = CDRType.Invalid;
            else if (query.Query.CDRType == ClientCDRType.PartialPriced)
                cdrType = CDRType.PartialPriced;
            else if (query.Query.CDRType == ClientCDRType.Successful)
                cdrType = CDRType.Successful;
            WhSCarrierAccountBEManager whSCarrierAccountBEManager = new WhSCarrierAccountBEManager();
            var accessibleCustomers = whSCarrierAccountBEManager.GetRemoteCarrierAccountsInfo(new Entities.ClientAccountInfoFilter() { GetCustomers = true });
            var accessibleSuppliers = whSCarrierAccountBEManager.GetRemoteCarrierAccountsInfo(new Entities.ClientAccountInfoFilter() { GetSuppliers = true });
            var input = new DataRetrievalInput<RepeatedNumberQuery>()
            {
                DataRetrievalResultType = query.DataRetrievalResultType,
                FromRow = query.FromRow,
                GetSummary = query.GetSummary,
                IsAPICall = query.IsAPICall,
                IsSortDescending = query.IsSortDescending,
                ResultKey = query.ResultKey,
                SortByColumnName = query.SortByColumnName,
                ToRow = query.ToRow,
                Query = new RepeatedNumberQuery()
                {
                    To = query.Query.To,
                    From = query.Query.From,
                    CDRType = cdrType,
                    Filter = new RepeatedNumberFilter()
                    {
                        ColumnsToShow = query.Query.Filter.AccountType == AccountViewType.Customer ? new List<string>() { "CustomerName", "SaleZoneName", "Attempt", "DurationInMinutes", "PhoneNumber" } : new List<string>() { "SupplierName", "SaleZoneName", "Attempt", "DurationInMinutes", "PhoneNumber" }
                    },
                    PhoneNumber = query.Query.PhoneNumber,
                    PhoneNumberType = query.Query.PhoneNumberType,
                    RepeatedMorethan = query.Query.RepeatedMorethan
                }
            };
            if (query.Query.Filter != null)
            {
                if (query.Query.Filter.CustomerIds != null && query.Query.Filter.CustomerIds.Count > 0)
                {
                    foreach (var customerId in query.Query.Filter.CustomerIds)
                    {
                        if (!accessibleCustomers.Any(x => x.AccountId == customerId))
                            throw new Exception("You are not authorized to view this report.");
                    }
                    input.Query.Filter.CustomerIds = query.Query.Filter.CustomerIds;
                }
                else if (query.Query.Filter.SupplierIds != null && query.Query.Filter.SupplierIds.Count > 0)
                {
                    foreach (var supplierId in query.Query.Filter.SupplierIds)
                    {
                        if (!accessibleSuppliers.Any(x => x.AccountId == supplierId))
                            throw new Exception("You are not authorized to view this report.");
                    }
                    input.Query.Filter.SupplierIds = query.Query.Filter.SupplierIds;
                }
                else
                {
                    if (query.Query.Filter.AccountType == AccountViewType.Customer)
                    {
                        input.Query.Filter.CustomerIds = accessibleCustomers != null && accessibleCustomers.Count() > 0 ? accessibleCustomers.Select(x => x.AccountId).ToList() : null;
                    }
                    else if (query.Query.Filter.AccountType == AccountViewType.Supplier)
                    {
                        input.Query.Filter.SupplierIds = accessibleSuppliers != null && accessibleSuppliers.Count() > 0 ? accessibleSuppliers.Select(x => x.AccountId).ToList() : null;
                    }
                }
                input.Query.Filter.SwitchIds = query.Query.Filter.SwitchIds;
            }
            var clonedInput = Utilities.CloneObject<DataRetrievalInput<RepeatedNumberQuery>>(input);
            clonedInput.IsAPICall = true;
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

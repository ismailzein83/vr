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
    public class ReleaseCodeStatisticsManager
    {
        public IDataRetrievalResult<ReleaseCodeStatDetail> GetFilteredReleaseCodeStatistics(DataRetrievalInput<ReleaseCodeQuery> query)
        {
            var connectionSettings = new PortalConnectionManager().GetWhSConnectionSettings();
            var clonedInput = Utilities.CloneObject<DataRetrievalInput<ReleaseCodeQuery>>(query);
            clonedInput.IsAPICall = true;
            WhSCarrierAccountBEManager whSCarrierAccountBEManager = new WhSCarrierAccountBEManager();
            var accessibleCustomers = whSCarrierAccountBEManager.GetRemoteCarrierAccountsInfo(new Entities.ClientAccountInfoFilter() { GetCustomers = true });
            var accessibleSuppliers = whSCarrierAccountBEManager.GetRemoteCarrierAccountsInfo(new Entities.ClientAccountInfoFilter() { GetSuppliers = true });
            List<string> columnsToShow = new List<string>() { "ReleaseCode", "ReleaseSource", "Attempt", "FailedAttempt", "Percentage", "DurationInMinutes",
            "FirstAttempt", "LastAttempt"};
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
                    clonedInput.Query.Filter.CustomerIds = accessibleCustomers != null && accessibleCustomers.Count() > 0 ? accessibleCustomers.Select(x => x.AccountId).ToList() : null;
                    clonedInput.Query.Filter.SupplierIds = accessibleSuppliers != null && accessibleSuppliers.Count() > 0 ? accessibleSuppliers.Select(x => x.AccountId).ToList() : null;
                }
                clonedInput.Query.Filter.ColumnsToShow = columnsToShow;
            }
            else
            {
                clonedInput.Query.Filter = new ReleaseCodeFilter()
                {
                    CustomerIds = accessibleCustomers != null && accessibleCustomers.Count() > 0 ? accessibleCustomers.Select(x => x.AccountId).ToList() : null,
                    SupplierIds = accessibleSuppliers != null && accessibleSuppliers.Count() > 0 ? accessibleSuppliers.Select(x => x.AccountId).ToList() : null,
                    ColumnsToShow = columnsToShow
                };
            }
            if (clonedInput.DataRetrievalResultType == DataRetrievalResultType.Excel)
            {
                return connectionSettings.Post<DataRetrievalInput<ReleaseCodeQuery>, RemoteExcelResult<ReleaseCodeStatDetail>>("/api/WhS_Analytics/ReleaseCode/GetAllFilteredReleaseCodes", clonedInput);
            }
            else if (clonedInput.DataRetrievalResultType == DataRetrievalResultType.Normal)
            {
                return connectionSettings.Post<DataRetrievalInput<ReleaseCodeQuery>, BigResult<ReleaseCodeStatDetail>>("/api/WhS_Analytics/ReleaseCode/GetAllFilteredReleaseCodes", clonedInput);
            }
            return null;
        }
    }
}

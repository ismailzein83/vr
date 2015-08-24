using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Data;
using TOne.Analytics.Entities;
using TOne.BusinessEntity.Business;
using TOne.BusinessEntity.Entities;
using Vanrise.Security.Business;

namespace TOne.Analytics.Business
{
    public class DailyReportManager
    {
        private List<AssignedCarrier> assignedCustomers;
        private List<AssignedCarrier> assignedSuppliers;

        public Vanrise.Entities.IDataRetrievalResult<DailyReportCall> GetFilteredDailyReportCalls(Vanrise.Entities.DataRetrievalInput<DailyReportQuery> input)
        {
            GetAssigendCarriers();

            IDailyReportDataManager dataManager = AnalyticsDataManagerFactory.GetDataManager<IDailyReportDataManager>();
            Vanrise.Entities.BigResult<DailyReportCall> bigResult = dataManager.GetFilteredDailyReportCalls(input, GetCarrierAccountIDs(assignedCustomers), GetCarrierAccountIDs(assignedSuppliers));

            if (assignedCustomers.Count > 0 || assignedSuppliers.Count > 0)
            {
                // if the user has some assigned carriers, then overwrite the names and rates of the unassigned carriers
                foreach (DailyReportCall call in bigResult.Data)
                {
                    bool isCustomerAssigned = IsCarrierAssigned(assignedCustomers, call.CustomerID);
                    bool isSupplierAssigned = IsCarrierAssigned(assignedSuppliers, call.SupplierID);

                    if (!isCustomerAssigned)
                    {
                        call.CustomerName = "N/A";
                        call.SaleRate = -1;
                        call.SaleRateDescription = "N/A";
                    }
                    else if (!isSupplierAssigned)
                    {
                        call.SupplierName = "N/A";
                        call.CostRate = -1;
                        call.CostRateDescription = "N/A";
                    }
                }
            }

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, bigResult);
        }

        private void GetAssigendCarriers()
        {
            AccountManagerManager accountManagerManager = new AccountManagerManager();
            assignedCustomers = accountManagerManager.GetAssignedCarriers(SecurityContext.Current.GetLoggedInUserId(), true, CarrierType.Customer);
            assignedSuppliers = accountManagerManager.GetAssignedCarriers(SecurityContext.Current.GetLoggedInUserId(), true, CarrierType.Supplier);
        }

        private List<string> GetCarrierAccountIDs(List<AssignedCarrier> carriers)
        {
            List<string> ids = new List<string>();

            foreach (AssignedCarrier carrier in carriers)
                ids.Add(carrier.CarrierAccountId);

            return ids;
        }

        private bool IsCarrierAssigned(List<AssignedCarrier> assignedCarriers, string targetCarrierID)
        {
            if (assignedCarriers.Count == 0 // the user has the right to view all carriers including this one
                || targetCarrierID == null) // assume that the carrier is assigned
                return true;

            foreach (AssignedCarrier carrier in assignedCarriers)
            {
                if (targetCarrierID == carrier.CarrierAccountId)
                    return true;
            }

            return false;
        }
    }
}

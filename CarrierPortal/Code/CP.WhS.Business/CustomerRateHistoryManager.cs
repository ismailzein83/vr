using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Entities;

namespace CP.WhS.Business
{
	public class CustomerRateHistoryManager
	{
		public Vanrise.Entities.IDataRetrievalResult<SaleRateHistoryRecordDetail> GetFilteredCustomerRateHistoryRecords(Vanrise.Entities.DataRetrievalInput<SaleRateHistoryQuery> input)
		{
			var connectionSettings = new PortalConnectionManager().GetWhSConnectionSettings();
			var clonedInput = Utilities.CloneObject<DataRetrievalInput<SaleRateHistoryQuery>>(input);
			clonedInput.IsAPICall = true;
			WhSCarrierAccountBEManager whSCarrierAccountBEManager = new WhSCarrierAccountBEManager();
			var accessibleCarrierAccounts = whSCarrierAccountBEManager.GetRemoteCarrierAccountsInfo(new Entities.ClientAccountInfoFilter() { GetCustomers = true });
			if (accessibleCarrierAccounts.FindRecord(x => x.AccountId == clonedInput.Query.OwnerId) == null)
				return null;

			if (clonedInput.DataRetrievalResultType == DataRetrievalResultType.Excel)
			{
				return connectionSettings.Post<DataRetrievalInput<SaleRateHistoryQuery>, RemoteExcelResult<SaleRateHistoryRecordDetail>>("/api/WhS_BE/SaleRateHistory/GetFilteredSaleRateHistoryRecords", clonedInput);
			}
			else if (clonedInput.DataRetrievalResultType == DataRetrievalResultType.Normal)
			{
				return connectionSettings.Post<DataRetrievalInput<SaleRateHistoryQuery>, BigResult<SaleRateHistoryRecordDetail>>("/api/WhS_BE/SaleRateHistory/GetFilteredSaleRateHistoryRecords", clonedInput);
			}

			return null;
		}
	}
}

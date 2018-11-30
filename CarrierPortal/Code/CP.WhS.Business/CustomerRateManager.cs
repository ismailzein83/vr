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
	public class CustomerRateManager
	{
		public Vanrise.Entities.IDataRetrievalResult<SaleRateDetail> GetFilteredCustomerRates(DataRetrievalInput<SaleRateQuery> input)
		{
			var connectionSettings = new PortalConnectionManager().GetWhSConnectionSettings();
			var clonedInput = Utilities.CloneObject<DataRetrievalInput<SaleRateQuery>>(input);
			clonedInput.IsAPICall = true;
			WhSCarrierAccountBEManager whSCarrierAccountBEManager = new WhSCarrierAccountBEManager();
			var accessibleCarrierAccounts = whSCarrierAccountBEManager.GetRemoteCarrierAccountsInfo(new Entities.ClientAccountInfoFilter() { GetCustomers = true });
			if (accessibleCarrierAccounts.FindRecord(x => x.AccountId == clonedInput.Query.OwnerId) == null)
				return null;

			if (clonedInput.DataRetrievalResultType == DataRetrievalResultType.Excel)
			{
				return connectionSettings.Post<DataRetrievalInput<SaleRateQuery>, RemoteExcelResult<SaleRateDetail>>("/api/WhS_BE/SaleRate/GetFilteredSaleRates", clonedInput);
			}
			else if (clonedInput.DataRetrievalResultType == DataRetrievalResultType.Normal)
			{
				return connectionSettings.Post<DataRetrievalInput<SaleRateQuery>, BigResult<SaleRateDetail>>("/api/WhS_BE/SaleRate/GetFilteredSaleRates", clonedInput);
			}
			return null;
		}
	}
}

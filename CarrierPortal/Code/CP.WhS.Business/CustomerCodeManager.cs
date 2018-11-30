using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CP.WhS.Entities;
using TOne.WhS.BusinessEntity.APIEntities;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Entities;

namespace CP.WhS.Business
{
	public class CustomerCodeManager
	{
		public Vanrise.Entities.IDataRetrievalResult<SaleCodeDetail> GetRemoteFilteredCustomerCodes(Vanrise.Entities.DataRetrievalInput<SaleCodeQueryHandlerInfoWrapper> input)
		{
			var connectionSettings = new PortalConnectionManager().GetWhSConnectionSettings();

			WhSCarrierAccountBEManager whSCarrierAccountBEManager = new WhSCarrierAccountBEManager();
			var accessibleCarrierAccounts = whSCarrierAccountBEManager.GetRemoteCarrierAccountsInfo(new Entities.ClientAccountInfoFilter() { GetCustomers = true });
			if (accessibleCarrierAccounts.FindRecord(x => x.AccountId == input.Query.CustomerId) == null)
				return null;

			var clonedInput = new DataRetrievalInput<SaleCodeQueryHandlerInfo>
			{
				IsAPICall = true,
				GetSummary = input.GetSummary,
				IsSortDescending = input.IsSortDescending,
				ResultKey = input.ResultKey,
				SortByColumnName = input.SortByColumnName,
				DataRetrievalResultType = input.DataRetrievalResultType,
				FromRow = input.FromRow,
				ToRow = input.ToRow,
				Query = input.Query.SaleCodeQueryHandlerInfo,
			};

			if (clonedInput.DataRetrievalResultType == DataRetrievalResultType.Excel)
			{
				return connectionSettings.Post<DataRetrievalInput<SaleCodeQueryHandlerInfo>, RemoteExcelResult<SaleCodeDetail>>("/api/WhS_BE/SaleCode/GetSaleCodeQueryHandlerInfo", clonedInput);
			}
			else if (clonedInput.DataRetrievalResultType == DataRetrievalResultType.Normal)
			{
				return connectionSettings.Post<DataRetrievalInput<SaleCodeQueryHandlerInfo>, BigResult<SaleCodeDetail>>("/api/WhS_BE/SaleCode/GetSaleCodeQueryHandlerInfo", clonedInput);
			}

			return null;
		}
	}
}

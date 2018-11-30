using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CP.WhS.Entities;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Entities;

namespace CP.WhS.Business
{
	public class SupplierOtherRateManager
	{
		public Vanrise.Entities.IDataRetrievalResult<SupplierOtherRateDetail> GetFilteredSupplierOtherRates(Vanrise.Entities.DataRetrievalInput<SupplierOtherRateQueryWrapper> input)
		{
			var connectionSettings = new PortalConnectionManager().GetWhSConnectionSettings();

			WhSCarrierAccountBEManager whSCarrierAccountBEManager = new WhSCarrierAccountBEManager();
			var accessibleCarrierAccounts = whSCarrierAccountBEManager.GetRemoteCarrierAccountsInfo(new Entities.ClientAccountInfoFilter() { GetSuppliers = true });
			if (accessibleCarrierAccounts.FindRecord(x => x.AccountId == input.Query.SupplierId) == null)
				return null;

			var clonedInput = new DataRetrievalInput<SupplierOtherRateQuery>
			{
				IsAPICall = true,
				GetSummary = input.GetSummary,
				IsSortDescending = input.IsSortDescending,
				ResultKey = input.ResultKey,
				SortByColumnName = input.SortByColumnName,
				DataRetrievalResultType = input.DataRetrievalResultType,
				FromRow = input.FromRow,
				ToRow = input.ToRow,
				Query = input.Query.SupplierOtherRateQuery,
			};

			if (clonedInput.DataRetrievalResultType == DataRetrievalResultType.Excel)
			{
				return connectionSettings.Post<DataRetrievalInput<SupplierOtherRateQuery>, RemoteExcelResult<SupplierOtherRateDetail>>("/api/WhS_BE/SupplierOtherRate/GetFilteredSupplierOtherRates", clonedInput);
			}
			else if (clonedInput.DataRetrievalResultType == DataRetrievalResultType.Normal)
			{
				return connectionSettings.Post<DataRetrievalInput<SupplierOtherRateQuery>, BigResult<SupplierOtherRateDetail>>("/api/WhS_BE/SupplierOtherRate/GetFilteredSupplierOtherRates", clonedInput);
			}
			return null;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.APIEntities;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Entities;

namespace CP.WhS.Business
{
	public class SupplierRateManager
	{
		public IDataRetrievalResult<SupplierRateDetail> GetSupplierRateQueryHandlerInfo(DataRetrievalInput<SupplierRateQueryHandlerInfo> input)
		{
			var connectionSettings = new PortalConnectionManager().GetWhSConnectionSettings();
			var clonedInput = Utilities.CloneObject<DataRetrievalInput<SupplierRateQueryHandlerInfo>>(input);
			clonedInput.IsAPICall = true;
			WhSCarrierAccountBEManager whSCarrierAccountBEManager = new WhSCarrierAccountBEManager();
			var accessibleCarrierAccounts = whSCarrierAccountBEManager.GetRemoteCarrierAccountsInfo(new Entities.ClientAccountInfoFilter() { GetSuppliers = true });
			if (accessibleCarrierAccounts.FindRecord(x => x.AccountId == clonedInput.Query.SupplierId) == null)
				return null;

			if (clonedInput.DataRetrievalResultType == DataRetrievalResultType.Excel)
			{
				return connectionSettings.Post<DataRetrievalInput<SupplierRateQueryHandlerInfo>, RemoteExcelResult<SupplierRateDetail>>("/api/WhS_BE/SupplierRate/GetSupplierRateQueryHandlerInfo", clonedInput);
			}
			else if (clonedInput.DataRetrievalResultType == DataRetrievalResultType.Normal)
			{
				return connectionSettings.Post<DataRetrievalInput<SupplierRateQueryHandlerInfo>, BigResult<SupplierRateDetail>>("/api/WhS_BE/SupplierRate/GetSupplierRateQueryHandlerInfo", clonedInput);
			}

			return null;

		}
	}
}

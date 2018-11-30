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
	public class SupplierCodeManager
	{
		public Vanrise.Entities.IDataRetrievalResult<SupplierCodeDetail> GetFilteredSupplierCodes(Vanrise.Entities.DataRetrievalInput<SupplierCodeQuery> input)
		{
			var connectionSettings = new PortalConnectionManager().GetWhSConnectionSettings();
			var clonedInput = Utilities.CloneObject<DataRetrievalInput<SupplierCodeQuery>>(input);
			clonedInput.IsAPICall = true;
			WhSCarrierAccountBEManager whSCarrierAccountBEManager = new WhSCarrierAccountBEManager();
			var accessibleCarrierAccounts = whSCarrierAccountBEManager.GetRemoteCarrierAccountsInfo(new Entities.ClientAccountInfoFilter() { GetSuppliers = true });
			if (accessibleCarrierAccounts.FindRecord(x => x.AccountId == clonedInput.Query.SupplierId) == null)
				return null;

			if (clonedInput.DataRetrievalResultType == DataRetrievalResultType.Excel)
			{
				return connectionSettings.Post<DataRetrievalInput<SupplierCodeQuery>, RemoteExcelResult<SupplierCodeDetail>>("/api/WhS_BE/SupplierCode/GetFilteredSupplierCodes", clonedInput);
			}
			else if (clonedInput.DataRetrievalResultType == DataRetrievalResultType.Normal)
			{
				return connectionSettings.Post<DataRetrievalInput<SupplierCodeQuery>, BigResult<SupplierCodeDetail>>("/api/WhS_BE/SupplierCode/GetFilteredSupplierCodes", clonedInput);
			}
			return null;
		}
	}
}

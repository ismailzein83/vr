using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;

namespace CP.WhS.Business
{
	public class CustomerOtherRateManager
	{
		public IEnumerable<SaleRateDetail> GetCustomerOtherRates(OtherSaleRateQuery query)
		{
			var connectionSettings = new PortalConnectionManager().GetWhSConnectionSettings();
			WhSCarrierAccountBEManager whSCarrierAccountBEManager = new WhSCarrierAccountBEManager();
			var accessibleCarrierAccounts = whSCarrierAccountBEManager.GetRemoteCarrierAccountsInfo(new Entities.ClientAccountInfoFilter() { GetCustomers = true });
			if (accessibleCarrierAccounts.FindRecord(x => x.AccountId == query.OwnerId) == null)
				return null;

			return connectionSettings.Post<OtherSaleRateQuery, IEnumerable<SaleRateDetail>>("/api/WhS_BE/OtherSaleRate/GetOtherSaleRates", query);
		}
	}
}

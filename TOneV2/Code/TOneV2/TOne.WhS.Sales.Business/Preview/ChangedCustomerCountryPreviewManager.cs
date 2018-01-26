using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.Sales.Data;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.Business
{
	public class ChangedCustomerCountryPreviewManager
	{
		public Vanrise.Entities.IDataRetrievalResult<ChangedCustomerCountryPreviewDetail> GetFilteredChangedCustomerCountryPreviews(Vanrise.Entities.DataRetrievalInput<RatePlanPreviewQuery> input)
		{
			return Vanrise.Common.Business.BigDataManager.Instance.RetrieveData(input, new ChangedCustomerCountryPreviewRequestHandler());
		}

		#region Private Classes

		private class ChangedCustomerCountryPreviewRequestHandler
			: Vanrise.Common.Business.BigDataRequestHandler<RatePlanPreviewQuery, ChangedCustomerCountryPreview, ChangedCustomerCountryPreviewDetail>
		{
			private Vanrise.Common.Business.CountryManager _countryManager = new Vanrise.Common.Business.CountryManager();

			public override ChangedCustomerCountryPreviewDetail EntityDetailMapper(ChangedCustomerCountryPreview entity)
			{
                var carrierAccountManager = new CarrierAccountManager();
				return new ChangedCustomerCountryPreviewDetail()
				{
					Entity = entity,
					CountryName = _countryManager.GetCountryName(entity.CountryId),
                    CustomerName = carrierAccountManager.GetCarrierAccountName(entity.CustomerId)
				};
			}

			public override IEnumerable<ChangedCustomerCountryPreview> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<RatePlanPreviewQuery> input)
			{
				var dataManager = SalesDataManagerFactory.GetDataManager<IChangedCustomerCountryPreviewDataManager>();
				return dataManager.GetChangedCustomerCountryPreviews(input.Query);
			}
		}

		#endregion
	}
}

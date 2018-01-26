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
	public class NewCustomerCountryPreviewManager
	{
		public Vanrise.Entities.IDataRetrievalResult<NewCustomerCountryPreviewDetail> GetFilteredNewCustomerCountryPreviews(Vanrise.Entities.DataRetrievalInput<RatePlanPreviewQuery> input)
		{
			return Vanrise.Common.Business.BigDataManager.Instance.RetrieveData(input, new NewCustomerCountryPreviewRequestHandler());
		}

		#region Private Classes

		private class NewCustomerCountryPreviewRequestHandler
			: Vanrise.Common.Business.BigDataRequestHandler<RatePlanPreviewQuery, NewCustomerCountryPreview, NewCustomerCountryPreviewDetail>
		{
			private Vanrise.Common.Business.CountryManager _countryManager = new Vanrise.Common.Business.CountryManager();

			public override NewCustomerCountryPreviewDetail EntityDetailMapper(NewCustomerCountryPreview entity)
			{
                var carrierAccountManager = new CarrierAccountManager();
				return new NewCustomerCountryPreviewDetail()
				{
					Entity = entity,
					CountryName = _countryManager.GetCountryName(entity.CountryId),
                    CustomerName = carrierAccountManager.GetCarrierAccountName(entity.CustomerId)
				};
			}

			public override IEnumerable<NewCustomerCountryPreview> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<RatePlanPreviewQuery> input)
			{
				var dataManager = SalesDataManagerFactory.GetDataManager<INewCustomerCountryPreviewDataManager>();
				return dataManager.GetNewCustomerCountryPreviews(input.Query);
			}
		}

		#endregion
	}
}

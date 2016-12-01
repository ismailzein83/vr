using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.Data
{
	public interface INewCustomerCountryPreviewDataManager : IDataManager, Vanrise.Data.IBulkApplyDataManager<NewCustomerCountryPreview>
	{
		long ProcessInstanceId { set; }
		void ApplyNewCustomerCountryPreviewsToDB(IEnumerable<NewCustomerCountryPreview> newCustomerCountryPreviews);
		IEnumerable<NewCustomerCountryPreview> GetNewCustomerCountryPreviews(RatePlanPreviewQuery query);
	}
}

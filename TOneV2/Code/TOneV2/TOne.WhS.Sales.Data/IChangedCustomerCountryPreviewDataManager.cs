using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.Data
{
	public interface IChangedCustomerCountryPreviewDataManager : IDataManager, Vanrise.Data.IBulkApplyDataManager<ChangedCustomerCountryPreview>
	{
		long ProcessInstanceId { set; }

		void ApplyChangedCustomerCountryPreviewsToDB(IEnumerable<ChangedCustomerCountryPreview> changedCustomerCountryPreviews);

		IEnumerable<ChangedCustomerCountryPreview> GetChangedCustomerCountryPreviews(RatePlanPreviewQuery query);
	}
}

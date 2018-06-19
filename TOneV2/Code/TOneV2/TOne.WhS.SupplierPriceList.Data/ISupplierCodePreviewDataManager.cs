using System;
using System.Collections.Generic;
using Vanrise.Data;
using TOne.WhS.SupplierPriceList.Entities;

namespace TOne.WhS.SupplierPriceList.Data
{
	public interface ISupplierCodePreviewDataManager : IDataManager, IBulkApplyDataManager<CodePreview>
	{
		long ProcessInstanceId { set; }

		void ApplyPreviewCodesToDB(object preparedCodes);

		IEnumerable<CodePreview> GetFilteredCodePreview(SPLPreviewQuery query);
	}
}
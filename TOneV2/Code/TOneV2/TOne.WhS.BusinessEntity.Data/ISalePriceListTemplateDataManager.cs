using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data
{
	public interface ISalePriceListTemplateDataManager : IDataManager
	{
		IEnumerable<SalePriceListTemplate> GetAll();

		bool Insert(SalePriceListTemplate salePriceListTemplate, out int insertedId);

		bool Update(SalePriceListTemplate salePriceListTemplate);

		bool AreSalePriceListTemplatesUpdated(ref object updateHandle);
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Sales.Entities
{
	public interface IRatePlanContext
	{
		SalePriceListOwnerType OwnerType { get; }
		DateTime RetroactiveDate { get; }
	}
}

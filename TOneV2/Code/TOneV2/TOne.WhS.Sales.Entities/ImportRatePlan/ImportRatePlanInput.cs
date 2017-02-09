using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Sales.Entities
{
	public class ImportRatePlanInput
	{
		public SalePriceListOwnerType OwnerType { get; set; }

		public int OwnerId { get; set; }

		public long FileId { get; set; }

		public bool HeaderRowExists { get; set; }
	}
}

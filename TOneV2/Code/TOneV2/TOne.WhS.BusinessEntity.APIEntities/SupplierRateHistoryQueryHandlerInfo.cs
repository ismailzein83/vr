using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.APIEntities
{
	public class SupplierRateHistoryQueryHandlerInfo
	{
		public DateTime EffectiveOn { get; set; }
		public int SupplierId { get; set; }
		public string SupplierZoneName { get; set; }

	}
}

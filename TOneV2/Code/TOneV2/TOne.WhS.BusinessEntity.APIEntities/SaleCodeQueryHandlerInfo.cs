using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.APIEntities
{
	public class SaleCodeQueryHandlerInfo
	{
		public List<long> ZonesIds { get; set; }
		public DateTime EffectiveOn { get; set; }

	}
}

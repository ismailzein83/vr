using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
	public abstract class MappedValue
	{
		public Guid ConfigId { get; set; }

		public abstract void Execute(IBasicSalePriceListTemplateSettingsMappedValueContext context);
	}
}

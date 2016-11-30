using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
	public abstract class MappedValue
	{
        public abstract Guid ConfigId { get; }

		public abstract void Execute(IBasicSalePriceListTemplateSettingsMappedValueContext context);
	}
}

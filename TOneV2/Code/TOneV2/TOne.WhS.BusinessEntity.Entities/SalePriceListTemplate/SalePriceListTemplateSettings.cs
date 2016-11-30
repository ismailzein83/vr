using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
	public abstract class SalePriceListTemplateSettings
	{
        public abstract Guid ConfigId { get; }

		public abstract byte[] Execute(ISalePriceListTemplateSettingsContext context);
	}
}

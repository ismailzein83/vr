using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions
{
	public class ConstantMappedValue : MappedValue
	{
		public string Value { get; set; }

		public override void Execute(IBasicSalePriceListTemplateSettingsMappedValueContext context)
		{
			context.Value = Value;
		}
	}
}

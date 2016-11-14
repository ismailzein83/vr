using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
	public class BasicSalePriceListTemplateSettingsMappedValueContext : IBasicSalePriceListTemplateSettingsMappedValueContext
	{
		public string Zone { get; set; }
		public string Code { get; set; }
		public DateTime? CodeBED { get; set; }
		public DateTime? CodeEED { get; set; }
		public decimal? Rate { get; set; }
		public DateTime? RateBED { get; set; }
		public DateTime? RateEED { get; set; }
		public object Value { get; set; }
	}
}

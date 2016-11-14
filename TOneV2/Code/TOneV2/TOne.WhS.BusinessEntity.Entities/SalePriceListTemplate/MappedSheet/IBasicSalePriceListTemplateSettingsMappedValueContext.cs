using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
	public interface IBasicSalePriceListTemplateSettingsMappedValueContext
	{
		string Zone { get; set; }
		string Code { get; set; }
		DateTime? CodeBED { get; set; }
		DateTime? CodeEED { get; set; }
		decimal? Rate { get; set; }
		DateTime? RateBED { get; set; }
		DateTime? RateEED { get; set; }
		object Value { get; set; }
	}
}

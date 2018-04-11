using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class DateTimeRange
    {
        public DateTime From { get; set; }

        public DateTime To { get; set; }
    }

	public class VRDateTimeRange : IDateEffectiveSettings
	{
		public DateTime BED { get; set; }

		public DateTime? EED { get; set; }
	}
}
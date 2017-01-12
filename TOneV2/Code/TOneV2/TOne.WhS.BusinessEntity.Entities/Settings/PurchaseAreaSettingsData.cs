using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
	public class PurchaseAreaSettingsData : Vanrise.Entities.SettingData
	{
		public int EffectiveDateDayOffset { get; set; }

		public int RetroactiveDayOffset { get; set; }
	}
}

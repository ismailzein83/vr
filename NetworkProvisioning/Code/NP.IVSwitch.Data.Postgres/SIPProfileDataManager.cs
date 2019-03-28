using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NP.IVSwitch.Entities.SIPProfile;
using TOne.WhS.RouteSync.IVSwitch;
using Vanrise.Data.Postgres;

namespace NP.IVSwitch.Data.Postgres
{
	class SIPProfileDataManager : BasePostgresDataManager, ISIPProfileDataManager
	{
		public TOne.WhS.RouteSync.IVSwitch.BuiltInIVSwitchSWSync IvSwitchSync { get; set; }
		protected override string GetConnectionString()
		{
			return IvSwitchSync.MasterConnectionString;
		}
		public List<SIPProfile> GetSIPProfiles()
		{

			var cmdText = "select profile_name,description,display_order from sip_profiles";
			return GetItemsText(cmdText, SIPProfileMapper, (cmd) => { });
		}
		SIPProfile SIPProfileMapper(IDataReader reader)
		{

			return new SIPProfile() {
				ProfileName = reader["profile_name"] as string,
				Description = reader["description"] as string,
				DisplayOrder = (int)reader["display_order"]
			};
		}
	}
}

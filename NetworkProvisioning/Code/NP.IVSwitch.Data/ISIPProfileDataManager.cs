using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NP.IVSwitch.Entities.SIPProfile;

namespace NP.IVSwitch.Data
{
	public interface ISIPProfileDataManager: IDataManager
	{
		List<SIPProfile> GetSIPProfiles();
	}
}

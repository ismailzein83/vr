using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NP.IVSwitch.Data;
using NP.IVSwitch.Entities.SIPProfile;
using Vanrise.Common;

namespace NP.IVSwitch.Business
{
	public class SIPProfileManager
	{

		#region Public Methods
		public IEnumerable<SIPProfileInfo> GetSIPProfilesInfo()
		{
			var allSIPProfiles = GetCachedSIPProfiles();
			if (allSIPProfiles != null)
				return allSIPProfiles.MapRecords(SIPProfileInfoMapper, cmd => { return true; });
			return null;
		}
		#endregion


		#region Private Methods
		private Dictionary<string, SIPProfile> GetCachedSIPProfiles() {
			return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetSIPProfiles", () =>
			{
				ISIPProfileDataManager dataManager = IVSwitchDataManagerFactory.GetDataManager<ISIPProfileDataManager>();
				Helper.SetSwitchConfig(dataManager);
				return dataManager.IvSwitchSync != null ? dataManager.GetSIPProfiles().ToDictionary(x => x.ProfileName, x => x):new Dictionary<string, SIPProfile>();
			});
		}
		#endregion

		#region Private Classes
		private class CacheManager : Vanrise.Caching.BaseCacheManager
		{
			protected override bool IsTimeExpirable => true;
			protected override bool UseCentralizedCacheRefresher => true;
			protected override bool ShouldSetCacheExpired(object parameter)
			{
				return base.ShouldSetCacheExpired();
			}
		}
		#endregion

		#region Mappers
		private SIPProfileInfo SIPProfileInfoMapper(SIPProfile sipProfile)
		{
			return new SIPProfileInfo()
			{
				ProfileName = sipProfile.ProfileName,
				Description = sipProfile.Description,
				DisplayOrder = sipProfile.DisplayOrder
			};
		}
		#endregion
	}
}

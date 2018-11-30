using System;
using System.Collections.Generic;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
	public class VRRestCountryManager
	{
		#region Public Methods

		public IEnumerable<CountryInfo> GetRemoteCountriesInfo(Guid connectionId, string filter = null)
		{


			return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetRemoteCountriesInfo",
			   () =>
			   {
				   VRInterAppRestConnection connectionSettings = VRRestInterAppRestConnectionManager.GetVRInterAppRestConnection(connectionId);
				   return connectionSettings.Get<IEnumerable<CountryInfo>>(string.Format("/api/VRCommon/Country/GetCountriesInfo?filter={0}", filter));
			   });
		}

		#endregion

		#region Private Classes

		private class CacheManager : Vanrise.Caching.BaseCacheManager
		{
			protected override bool IsTimeExpirable
			{
				get { return true; }
			}
		}
		#endregion
	}

}

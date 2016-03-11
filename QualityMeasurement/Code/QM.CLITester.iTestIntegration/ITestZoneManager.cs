using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Vanrise.Common;

namespace QM.CLITester.iTestIntegration
{
    public class ITestZoneManager 
    {
        public Dictionary<string, ITestZone> GetAllZones()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetAllZones",
                () =>
                {
                    ServiceActions serviceActions = new ServiceActions();
                    string zoneResponse = serviceActions.PostRequest("1022", null);
                    if (zoneResponse == null)
                        throw new NullReferenceException("zoneResponse");
                    return ParseZoneResponse(zoneResponse);
                });
        }

        public ITestZone GetMatchZone(IEnumerable<string> codes)
        {
            return null;
        }


        public ITestZone GetZone(string itestZoneId)
        {
            return GetAllZones().GetRecord(itestZoneId);
        }

        #region Private Methods

        private Dictionary<string, ITestZone> ParseZoneResponse(string breakoutResponse)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(breakoutResponse);

            Dictionary<string, ITestZone> zones = new Dictionary<string, ITestZone>();
            XmlNodeList xnList = xml.SelectNodes("/NDB_List/Breakout");
            if (xnList != null)
                foreach (XmlNode xn in xnList)
                {
                    ITestZone zone = new ITestZone
                    {
                        ZoneId = xn["Breakout_ID"] != null ? xn["Breakout_ID"].InnerText : "",
                        ZoneName = xn["Breakout_Name"] != null ? xn["Breakout_Name"].InnerText : "",
                        CountryId = xn["Country_ID"] != null ? xn["Country_ID"].InnerText : "",
                        CountryName = xn["Country_Name"] != null ? xn["Country_Name"].InnerText : ""
                    };
                    if (!zones.ContainsKey(zone.ZoneId))
                        zones.Add(zone.ZoneId, zone);
                }
            return zones;
        }

        #endregion

        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            protected override bool IsTimeExpirable
            {
                get
                {
                    return true;
                }
            }
        }

        #endregion
    }
}

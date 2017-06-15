using QM.BusinessEntity.Business;
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
                    //string zoneResponse = serviceActions.PostRequest("1022", null);
                    string zoneResponse = serviceActions.PostRequestBeta("1025", null);
                    if (zoneResponse == null)
                        throw new NullReferenceException("zoneResponse");
                    //return ParseZoneResponse(zoneResponse);
                    return ParseZoneResponseBeta(zoneResponse);
                });
        }

        public ITestZone GetMatchZone(IEnumerable<string> codes)
        {
            if (codes == null)
                return null;
            ConnectorZoneInfoManager zoneInfoManager = new ConnectorZoneInfoManager();
            var zoneInfo = zoneInfoManager.GetLongestMatchZone(Constants.CONNECTOR_TYPE, codes);
            if (zoneInfo != null)
                return GetZone(zoneInfo.ConnectorZoneId);
            else
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
        private Dictionary<string, ITestZone> ParseZoneResponseBeta(string breakoutResponse)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(breakoutResponse);

            XmlNode zoneXMLNode = xml.SelectSingleNode("/NDB_List");
            if (zoneXMLNode == null)
                throw new Exception(breakoutResponse);

            Dictionary<string, ITestZone> zones = new Dictionary<string, ITestZone>();
            XmlNodeList xnList = xml.SelectNodes("/NDB_List/Breakout");
            if (xnList != null)
            {
                foreach (XmlNode xn in xnList)
                {
                    ITestZone zone = new ITestZone
                    {
                        ZoneId = xn["B_CLI_ID"] != null ? xn["B_CLI_ID"].InnerText : "",
                        ZoneName = xn["B"] != null ? xn["B"].InnerText : "",
                        CountryId = xn["C_CLI_ID"] != null ? xn["C_CLI_ID"].InnerText : "",
                        CountryName = xn["C"] != null ? xn["C"].InnerText : "",
                        IsOffline = !(xn["online"] != null && xn["online"].InnerText == "1")
                    };
                    if (!zones.ContainsKey(zone.ZoneId))
                        zones.Add(zone.ZoneId, zone);
                }
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

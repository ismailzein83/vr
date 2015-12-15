using QM.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace QM.CLITester.iTestIntegration
{
    public class ZoneITestReader : SourceZoneReader
    {
        public override bool UseSourceItemId
        {
            get { return false; }
        }

        ServiceActions _serviceActions = new ServiceActions();

        public override IEnumerable<SourceZone> GetChangedItems(ref object updatedHandle)
        {
            string breakoutResponse = _serviceActions.PostRequest("1022", null);
            return ParseZoneResponse(breakoutResponse);
        }

        private IEnumerable<SourceZone> ParseZoneResponse(string breakoutResponse)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(breakoutResponse);

            List<SourceZone> zones = new List<SourceZone>();
            XmlNodeList xnList = xml.SelectNodes("/NDB_List/Breakout");
            if (xnList != null)
                foreach (XmlNode xn in xnList)
                {
                    SourceZone zone = new SourceZone
                    {
                        SourceId = xn["Breakout_ID"] != null ? xn["Breakout_ID"].InnerText : "",
                        Name = xn["Breakout_Name"] != null ? xn["Breakout_Name"].InnerText : "",
                        SourceCountryId = xn["Country_ID"] != null ? xn["Country_ID"].InnerText : "",
                        CountryName = xn["Country_Name"] != null ? xn["Country_Name"].InnerText : "",
                        BeginEffectiveDate = DateTime.Today
                    };
                    zones.Add(zone);
                }
            return zones;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Vanrise.Entities;

namespace QM.CLITester.iTestIntegration
{
    public class CountryITestReader : Vanrise.Entities.SourceCountryReader
    {
        public override bool UseSourceItemId
        {
            get { return false; }
        }

        ServiceActions _serviceActions = new ServiceActions();

        public override IEnumerable<Vanrise.Entities.SourceCountry> GetChangedItems(ref object updatedHandle)
        {
            string breakoutResponse = _serviceActions.PostRequest("1022", null);
            return ParseCountryResponse(breakoutResponse);
        }

        private IEnumerable<Vanrise.Entities.SourceCountry> ParseCountryResponse(string breakoutResponse)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(breakoutResponse);

            Dictionary<string, SourceCountry> countries = new Dictionary<string, SourceCountry>();
            XmlNodeList xnList = xml.SelectNodes("/NDB_List/Breakout");
            if (xnList != null)
                foreach (XmlNode xn in xnList)
                {
                    string sourceCountryId = xn["Country_ID"] != null ? xn["Country_ID"].InnerText : "";
                    if(!countries.ContainsKey(sourceCountryId))
                    {
                        SourceCountry country = new SourceCountry
                        {
                            SourceId = sourceCountryId,
                            Name = xn["Country_Name"] != null ? xn["Country_Name"].InnerText : ""
                        };
                        countries.Add(sourceCountryId, country);
                    }
                }
            return countries.Values;
        }
    }
}

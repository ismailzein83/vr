using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using QM.CLITester.Entities;

namespace QM.CLITester.Business
{
    public class TestCallManager
    {
        public Dictionary<string, Country> dictionaryCountries = new Dictionary<string, Country>(); 
        private void GetTestCall()
        {
            var request = (HttpWebRequest)WebRequest.Create("https://api.i-test.net/?t=1022");

            var postData = "email=myahya2@vanrise.com";
            postData += "&pass=123456789";
            var data = Encoding.ASCII.GetBytes(postData);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();

            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            //Dictionary<string, Country> dictionaryCountries = new Dictionary<string, Country>();
            
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(responseString);

            XmlNodeList xnList = xml.SelectNodes("/NDB_List/Breakout");
            if (xnList != null)
            foreach (XmlNode xn in xnList)
            {
                Country countryNode = new Country
                {
                    Id = xn["Country_ID"] != null ? xn["Country_ID"].InnerText : "",
                    Name = xn["Country_Name"] != null ? xn["Country_Name"].InnerText : ""
                };
                Breakout breakoutNode = new Breakout
                {
                    Id = xn["Breakout_ID"] != null ? xn["Breakout_ID"].InnerText : "",
                    Name = xn["Breakout_Name"] != null ? xn["Breakout_Name"].InnerText : ""
                };

                if (countryNode.Name != "")
                {
                    if (!dictionaryCountries.ContainsKey(countryNode.Id))
                    {
                        countryNode.Breakouts = new List<Breakout>();
                        countryNode.Breakouts.Add(breakoutNode);
                        dictionaryCountries.Add(countryNode.Id, countryNode);
                    }
                    else
                    {
                        Country country = new Country();
                        if (dictionaryCountries.TryGetValue(countryNode.Id, out country))
                            if (!country.Breakouts.Exists(x => x.Id == breakoutNode.Id))
                                country.Breakouts.Add(breakoutNode);
                    }
                }
            }
        }

        public IEnumerable<Country> GetCachedCountries()
        {
            List<Country> lstCountries = new List<Country>();
            if (dictionaryCountries.Count == 0)
                GetTestCall();
            foreach (var v in dictionaryCountries)
            {
                Country country = new Country();
                country.Id = v.Value.Id;
                country.Name = v.Value.Name;
                lstCountries.Add(country);
            }
            return lstCountries;
        }

        public IEnumerable<Breakout> GetCachedBreakouts(string selectedCountry)
        {
            //List<Breakout> lstBreakouts = new List<Breakout>();
            if (dictionaryCountries.Count == 0)
                GetTestCall();
            Country country = new Country();
            if (dictionaryCountries.TryGetValue(selectedCountry, out country))
            {
                return country.Breakouts;
            }
            return null;
            //foreach (var v in dictionaryCountries)
            //{
                
            //    Breakout breakout = new Breakout();
            //    breakout.Id = v.Value.Id;
            //    breakout.Name = v.Value.Name;
            //    lstBreakouts.Add(breakout);
            //}
            //return lstBreakouts;
        }
    }
}

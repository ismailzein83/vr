using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using QM.CLITester.Data;
using QM.CLITester.Entities;

namespace QM.CLITester.Business
{
    public class TestCallManager
    {
        private readonly Dictionary<string, Country> _dictionaryCountries = new Dictionary<string, Country>();
        private readonly List<Supplier> _listSuppliers = new List<Supplier>();
        private string PostRequest(string functionCode, string parameters)
        {
            var request = (HttpWebRequest)WebRequest.Create("https://api.i-test.net/?t=" + functionCode + (parameters ?? ""));

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
            return new StreamReader(response.GetResponseStream()).ReadToEnd();
        }

        private void ResponseCountryBreakout(string response)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(response);

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
                        if (!_dictionaryCountries.ContainsKey(countryNode.Id))
                        {
                            countryNode.Breakouts = new List<Breakout>();
                            countryNode.Breakouts.Add(breakoutNode);
                            _dictionaryCountries.Add(countryNode.Id, countryNode);
                        }
                        else
                        {
                            Country country = new Country();
                            if (_dictionaryCountries.TryGetValue(countryNode.Id, out country))
                                if (!country.Breakouts.Exists(x => x.Id == breakoutNode.Id))
                                    country.Breakouts.Add(breakoutNode);
                        }
                }
        }

        private void ResponseSuppliers(string response)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(response);

            XmlNodeList xnList = xml.SelectNodes("/Vendors_List/Supplier");
            if (xnList != null)
                foreach (XmlNode xn in xnList)
                {
                    Supplier supplierNode = new Supplier
                    {
                        Id = xn["Supplier_ID"] != null ? xn["Supplier_ID"].InnerText : "",
                        Name = xn["Supplier_Name"] != null ? xn["Supplier_Name"].InnerText : "",
                        Prefix = xn["Prefix"] != null ? xn["Prefix"].InnerText : "",
                        Codec = xn["Codec"] != null ? xn["Codec"].InnerText : "",
                        ShortName = (xn["Supplier_Name"] != null ? xn["Supplier_Name"].InnerText : "") + " - " + (xn["Prefix"] != null ? xn["Prefix"].InnerText : "")
                    };
                    if (supplierNode.Id != "")
                        _listSuppliers.Add(supplierNode);
                }
        }
        const string GoodAmpersand = "&amp;";

        private TestCallResult ResponseTestCall(string response)
        {
            Regex badAmpersand = new Regex("&(?![a-zA-Z]{2,6};|#[0-9]{2,4};)");
            response = badAmpersand.Replace(response, GoodAmpersand);
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(response);

            XmlNodeList xnList = xml.SelectNodes("/Test_Initiation/Test");
            if (xnList != null)
            {
                TestCallResult testCallResultNode = new TestCallResult
                {
                    Test_ID = xnList[0]["Test_ID"] != null ? xnList[0]["Test_ID"].InnerText : ""
                };
                if (testCallResultNode.Test_ID != null)
                    return responseTestCallResult(PostRequest("3011", "&jid=" + testCallResultNode.Test_ID), testCallResultNode.Test_ID);
            }
            return null;
        }

        private TestCallResult responseTestCallResult(string response, string testId)
        {
            Regex badAmpersand = new Regex("&(?![a-zA-Z]{2,6};|#[0-9]{2,4};)");
            response = badAmpersand.Replace(response, GoodAmpersand);

            response = response.Replace("<" + testId + ">", "<_" + testId + ">");

            response = response.Replace("</" + testId + ">", "</_" + testId + ">");

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(response);

            XmlNodeList xnList = xml.SelectNodes("/Test_Status/_" + testId);
            if (xnList != null)
            {
                TestCallResult testCallResultNode = new TestCallResult
                {
                    Name = xnList[0]["Name"] != null ? xnList[0]["Name"].InnerText : "",
                    Calls_Total = xnList[0]["Calls_Total"] != null ? Int32.Parse(xnList[0]["Calls_Total"].InnerText) : 0,
                    Calls_Complete = xnList[0]["Calls_Complete"] != null ? Int32.Parse(xnList[0]["Calls_Complete"].InnerText) : 0,
                    CLI_Success = xnList[0]["CLI_Success"] != null ? Int32.Parse(xnList[0]["CLI_Success"].InnerText) : 0,
                    CLI_No_Result = xnList[0]["CLI_No_Result"] != null ? Int32.Parse(xnList[0]["CLI_No_Result"].InnerText) : 0,
                    CLI_Fail = xnList[0]["CLI_Fail"] != null ? Int32.Parse(xnList[0]["CLI_Fail"].InnerText) : 0,
                    PDD = xnList[0]["PDD"] != null ? Int32.Parse(xnList[0]["PDD"].InnerText) : 0,
                    Share_URL = xnList[0]["Share_URL"] != null ? xnList[0]["Share_URL"].InnerText : ""
                };
                return testCallResultNode;
            }
            return null;
        }

        public IEnumerable<Country> GetCachedCountries()
        {
            List<Country> lstCountries = new List<Country>();
            if (_dictionaryCountries.Count == 0)
                ResponseCountryBreakout(PostRequest("1022", null));

            foreach (var v in _dictionaryCountries)
            {
                Country country = new Country
                {
                    Id = v.Value.Id,
                    Name = v.Value.Name
                };

                lstCountries.Add(country);
            }
            return lstCountries;
        }

        public IEnumerable<Breakout> GetCachedBreakouts(string selectedCountry)
        {
            if (_dictionaryCountries.Count == 0)
                ResponseCountryBreakout(PostRequest("1022", null));
            Country country = new Country();
            if (_dictionaryCountries.TryGetValue(selectedCountry, out country))
            {
                return country.Breakouts;
            }
            return null;
        }

        public IEnumerable<Supplier> GetCachedSuppliers()
        {
            if (_listSuppliers.Count == 0)
                ResponseSuppliers(PostRequest("1012", null));
            return _listSuppliers;
        }
        public TestCallResult TestCall(string selectedCountry, string selectedBreakout, string selectedSupplier)
        {
            return ResponseTestCall(PostRequest("2012", "&profid=4992&vendid=" + selectedSupplier + "&ndbccgid=" + selectedCountry + "&ndbcgid=" + selectedBreakout));
        }
        public Vanrise.Entities.InsertOperationOutput<TestCallResult> AddNewTestCall(TestCallResult testCallResult)
        {
            Vanrise.Entities.InsertOperationOutput<TestCallResult> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<TestCallResult>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            int carrierAccountId = -1;

            ITestCallDataManager dataManager = CliTesterDataManagerFactory.GetDataManager<ITestCallDataManager>();
            bool insertActionSucc = dataManager.Insert(testCallResult, out carrierAccountId);
            if (insertActionSucc)
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = testCallResult;
            }

            return insertOperationOutput;
        }

        public List<TestCallResult> GetTestCalls()
        {
            ITestCallDataManager dataManager = CliTesterDataManagerFactory.GetDataManager<ITestCallDataManager>();
            return dataManager.GetTestCalls();
        }
    }

}

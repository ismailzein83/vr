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
using Vanrise.Common;
using Vanrise.Entities;
using Country = QM.CLITester.Entities.Country2;

namespace QM.CLITester.Business
{
    public class TestCallManager
    {
        private readonly Dictionary<string, Country> _dictionaryCountries = new Dictionary<string, Country>();
        //private readonly List<Supplier> _listSuppliers = new List<Supplier>();
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
                    Country2 countryNode = new Country
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
                            Country2 country = new Country();
                            if (_dictionaryCountries.TryGetValue(countryNode.Id, out country))
                                if (!country.Breakouts.Exists(x => x.Id == breakoutNode.Id))
                                    country.Breakouts.Add(breakoutNode);
                        }
                }
        }

        //private void ResponseSuppliers(string response)
        //{
        //    XmlDocument xml = new XmlDocument();
        //    xml.LoadXml(response);

        //    XmlNodeList xnList = xml.SelectNodes("/Vendors_List/Supplier");
        //    if (xnList != null)
        //        foreach (XmlNode xn in xnList)
        //        {
        //            Supplier supplierNode = new Supplier
        //            {
        //                Id = xn["Supplier_ID"] != null ? xn["Supplier_ID"].InnerText : "",
        //                Name = xn["Supplier_Name"] != null ? xn["Supplier_Name"].InnerText : "",
        //                Prefix = xn["Prefix"] != null ? xn["Prefix"].InnerText : "",
        //                Codec = xn["Codec"] != null ? xn["Codec"].InnerText : "",
        //                ShortName = (xn["Supplier_Name"] != null ? xn["Supplier_Name"].InnerText : "") + " - " + (xn["Prefix"] != null ? xn["Prefix"].InnerText : "")
        //            };
        //            if (supplierNode.Id != "")
        //                _listSuppliers.Add(supplierNode);
        //        }
        //}
        const string GoodAmpersand = "&amp;";

        //private TestCallResult ResponseTestCall(string response, TestCall testCall)
        //{
        //    Regex badAmpersand = new Regex("&(?![a-zA-Z]{2,6};|#[0-9]{2,4};)");
        //    response = badAmpersand.Replace(response, GoodAmpersand);
        //    XmlDocument xml = new XmlDocument();
        //    xml.LoadXml(response);

        //    XmlNodeList xnList = xml.SelectNodes("/Test_Initiation/Test");
        //    if (xnList != null)
        //    {
        //        TestCall testCallResultNode = new TestCall
        //        {
        //            Test_ID = xnList[0]["Test_ID"] != null ? xnList[0]["Test_ID"].InnerText : "",
        //            Id= testCall.Id,
        //            SupplierID = testCall.SupplierID,
        //            CountryID = testCall.CountryID,
        //            ZoneID = testCall.ZoneID,
        //            Status = 1
        //        };

        //        if (testCallResultNode.Test_ID != null && testCall.Test_ID != "")
        //            UpdateTestCallResult(testCallResultNode);
        //    }
        //    return null;
        //}

        //private TestCallResult responseTestCallResult(string response, TestCallResult testCallResult)
        //{
        //    Regex badAmpersand = new Regex("&(?![a-zA-Z]{2,6};|#[0-9]{2,4};)");
        //    response = badAmpersand.Replace(response, GoodAmpersand);

        //    response = response.Replace("<" + testCallResult.Test_ID + ">", "<_" + testCallResult.Test_ID + ">");

        //    response = response.Replace("</" + testCallResult.Test_ID + ">", "</_" + testCallResult.Test_ID + ">");

        //    XmlDocument xml = new XmlDocument();
        //    xml.LoadXml(response);

        //    XmlNodeList xnList = xml.SelectNodes("/Test_Status/_" + testCallResult.Test_ID);
        //    if (xnList != null)
        //    {
        //        TestCallResult testCallResultNode = new TestCallResult
        //        {
        //            Name = xnList[0]["Name"] != null ? xnList[0]["Name"].InnerText : "",
        //            Calls_Total = xnList[0]["Calls_Total"] != null ? Int32.Parse(xnList[0]["Calls_Total"].InnerText) : 0,
        //            Calls_Complete = xnList[0]["Calls_Complete"] != null ? Int32.Parse(xnList[0]["Calls_Complete"].InnerText) : 0,
        //            CLI_Success = xnList[0]["CLI_Success"] != null ? Int32.Parse(xnList[0]["CLI_Success"].InnerText) : 0,
        //            CLI_No_Result = xnList[0]["CLI_No_Result"] != null ? Int32.Parse(xnList[0]["CLI_No_Result"].InnerText) : 0,
        //            CLI_Fail = xnList[0]["CLI_Fail"] != null ? Int32.Parse(xnList[0]["CLI_Fail"].InnerText) : 0,
        //            PDD = xnList[0]["PDD"] != null ? Int32.Parse(xnList[0]["PDD"].InnerText) : 0,
        //            Share_URL = xnList[0]["Share_URL"] != null ? xnList[0]["Share_URL"].InnerText : "",
        //            Id = testCallResult.Id,
        //            Test_ID = testCallResult.Test_ID,
        //            SupplierID = testCallResult.SupplierID,
        //            CountryID = testCallResult.CountryID,
        //            ZoneID = testCallResult.ZoneID,
        //            Status = 2
        //        };

        //        if (testCallResultNode.Calls_Total == testCallResultNode.Calls_Complete)
        //            UpdateTestCallResult(testCallResultNode);   
                
        //        return testCallResultNode;
        //    }
        //    return null;
        //}

        public IEnumerable<Country> GetCachedCountries()
        {
            List<Country> lstCountries = new List<Country>();
            if (_dictionaryCountries.Count == 0)
                ResponseCountryBreakout(PostRequest("1022", null));

            foreach (var v in _dictionaryCountries)
            {
                Country2 country = new Country
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
            Country2 country = new Country();
            if (_dictionaryCountries.TryGetValue(selectedCountry, out country))
            {
                return country.Breakouts;
            }
            return null;
        }

        //public IEnumerable<Supplier> GetCachedSuppliers()
        //{
        //    if (_listSuppliers.Count == 0)
        //        ResponseSuppliers(PostRequest("1012", null));
        //    return _listSuppliers;
        //}
        public TestCallResult TestCall(TestCallResult testCallResult)
        {
            return null;
            //return ResponseTestCall(PostRequest("2012", "&profid=4992&vendid=" + testCallResult.SupplierID + "&ndbccgid=" + testCallResult.CountryID + "&ndbcgid=" + testCallResult.ZoneID), testCallResult);
        }

        public TestCallResult TestCallResult(TestCallResult testCallResult)
        {
            //if (testCallResult.Test_ID != null)
                //return responseTestCallResult(PostRequest("3011", "&jid=" + testCallResult.Test_ID), testCallResult);
            return null;
        }
        public Vanrise.Entities.InsertOperationOutput<TestCall> AddNewTestCall(TestCall testCallResult)
        {
            Vanrise.Entities.InsertOperationOutput<TestCall> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<TestCall>();

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

        public bool UpdateInitiateTest(string initiateTestOutput, CallTestStatus callTestStatus, long testCallID)
        {
            ITestCallDataManager dataManager = CliTesterDataManagerFactory.GetDataManager<ITestCallDataManager>();
            return dataManager.UpdateInitiateTest(initiateTestOutput, callTestStatus, testCallID);
        }

        public bool UpdateTestProgress(string initiateTestOutput, CallTestResult callTestResult, long testCallID)
        {
            ITestCallDataManager dataManager = CliTesterDataManagerFactory.GetDataManager<ITestCallDataManager>();
            return dataManager.UpdateTestProgress(initiateTestOutput, callTestResult, testCallID);
        }

        //public Vanrise.Entities.UpdateOperationOutput<TestCallResult> UpdateTestCallResult(TestCallResult testCallResult)
        //{
        //    ITestCallDataManager dataManager = CliTesterDataManagerFactory.GetDataManager<ITestCallDataManager>();

        //    bool updateActionSucc = dataManager.Update(testCallResult);
        //    Vanrise.Entities.UpdateOperationOutput<TestCallResult> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<TestCallResult>();

        //    updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
        //    updateOperationOutput.UpdatedObject = null;

        //    if (updateActionSucc)
        //    {
        //        updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
        //        updateOperationOutput.UpdatedObject = testCallResult;
        //    }
        //    else
        //    {
        //        updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
        //    }

        //    return updateOperationOutput;
        //}


        public IDataRetrievalResult<TestCall> GetFilteredTestCalls(DataRetrievalInput<TestCallResultQuery> input)
        {
            TestCallManager manager = new TestCallManager();
            var allTestCalls = manager.GetTestCalls();
            Func<TestCall, bool> filterExpression = (x) => (input.Query.Test_ID == null);
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allTestCalls.ToBigResult(input, filterExpression));
        }



        public List<TestCall> GetTestCalls()
        {
            ITestCallDataManager dataManager = CliTesterDataManagerFactory.GetDataManager<ITestCallDataManager>();
            return dataManager.GetTestCalls();
        }

        public List<TestCall> GetTestCalls(int callTestStatus)
        {
            ITestCallDataManager dataManager = CliTesterDataManagerFactory.GetDataManager<ITestCallDataManager>();
            return dataManager.GetTestCalls(callTestStatus);
        }


        //public List<TestCallResult> GetRequestedTestCallResults()
        //{
        //    ITestCallDataManager dataManager = CliTesterDataManagerFactory.GetDataManager<ITestCallDataManager>();
        //    return dataManager.GetRequestedTestCallResults();
        //}
    }

}

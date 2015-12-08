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
        private readonly List<Supplier2> _listSuppliers = new List<Supplier2>();
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
        private void ResponseSuppliers(string response)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(response);

            XmlNodeList xnList = xml.SelectNodes("/Vendors_List/Supplier");
            if (xnList != null)
                foreach (XmlNode xn in xnList)
                {
                    Supplier2 supplierNode = new Supplier2
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

        public IEnumerable<Supplier2> GetCachedSuppliers()
        {
            if (_listSuppliers.Count == 0)
                ResponseSuppliers(PostRequest("1012", null));
            return _listSuppliers;
        }
        public Vanrise.Entities.InsertOperationOutput<TestCall> AddNewTestCall(TestCall testCallResult)
        {
            testCallResult.CallTestStatus = CallTestStatus.New;
            testCallResult.CallTestResult = CallTestResult.NotCompleted;

            Vanrise.Entities.InsertOperationOutput<TestCall> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<TestCall>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            int testCallId = -1;

            ITestCallDataManager dataManager = CliTesterDataManagerFactory.GetDataManager<ITestCallDataManager>();
            bool insertActionSucc = dataManager.Insert(testCallResult, out testCallId);
            if (insertActionSucc)
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = testCallResult;
            }

            return insertOperationOutput;
        }

        public bool UpdateInitiateTest(long testCallId, Object initiateTestInformation, CallTestStatus callTestStatus)
        {
            ITestCallDataManager dataManager = CliTesterDataManagerFactory.GetDataManager<ITestCallDataManager>();
            return dataManager.UpdateInitiateTest(testCallId, initiateTestInformation, callTestStatus);
        }

        public bool UpdateTestProgress(long testCallId, Object testProgress, CallTestStatus callTestStatus, CallTestResult? callTestResult)
        {
            ITestCallDataManager dataManager = CliTesterDataManagerFactory.GetDataManager<ITestCallDataManager>();
            return dataManager.UpdateTestProgress(testCallId, testProgress, callTestStatus, callTestResult);
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

        public List<TestCall> GetTestCalls(List<int> callTestStatus)
        {
            ITestCallDataManager dataManager = CliTesterDataManagerFactory.GetDataManager<ITestCallDataManager>();
            return dataManager.GetTestCalls(callTestStatus);
        }
    }

}

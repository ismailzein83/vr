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
using Vanrise.Common.Business;
using Vanrise.Entities;
using Country = QM.CLITester.Entities.Country2;

namespace QM.CLITester.Business
{
    public class TestCallManager
    {
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

        public Vanrise.Entities.InsertOperationOutput<TestCallQueryInsert> AddNewTestCall(TestCallQueryInsert testCallResult)
        {
            testCallResult.CallTestStatus = CallTestStatus.New;
            testCallResult.CallTestResult = CallTestResult.NotCompleted;
            testCallResult.InitiationRetryCount = 0;
            testCallResult.GetProgressRetryCount = 0;
            testCallResult.UserID = Vanrise.Security.Business.SecurityContext.Current.GetLoggedInUserId();
            Vanrise.Entities.InsertOperationOutput<TestCallQueryInsert> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<TestCallQueryInsert>();

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

        public bool UpdateInitiateTest(long testCallId, Object initiateTestInformation, CallTestStatus callTestStatus, int initiationRetryCount, string failureMessage)
        {
            ITestCallDataManager dataManager = CliTesterDataManagerFactory.GetDataManager<ITestCallDataManager>();
            return dataManager.UpdateInitiateTest(testCallId, initiateTestInformation, callTestStatus, initiationRetryCount, failureMessage);
        }

        public bool UpdateTestProgress(long testCallId, Object testProgress, CallTestStatus callTestStatus, CallTestResult? callTestResult, int getProgressRetryCount, string failureMessage)
        {
            ITestCallDataManager dataManager = CliTesterDataManagerFactory.GetDataManager<ITestCallDataManager>();
            return dataManager.UpdateTestProgress(testCallId, testProgress, callTestStatus, callTestResult, getProgressRetryCount, failureMessage);
        }

        public LastCallUpdateOutput GetUpdatedTestCalls(ref byte[] maxTimeStamp)
        {
            LastCallUpdateOutput lastCallUpdateOutputs = new LastCallUpdateOutput();
            List<CallTestStatus> listPendingCallTestStatus = new List<CallTestStatus>();
            listPendingCallTestStatus.Add(CallTestStatus.Initiated);
            listPendingCallTestStatus.Add(CallTestStatus.InitiationFailedWithRetry);
            listPendingCallTestStatus.Add(CallTestStatus.New);
            listPendingCallTestStatus.Add(CallTestStatus.PartiallyCompleted);
            listPendingCallTestStatus.Add(CallTestStatus.GetProgressFailedWithRetry);

            ITestCallDataManager dataManager = CliTesterDataManagerFactory.GetDataManager<ITestCallDataManager>();
            lastCallUpdateOutputs.ListTestCallDetails = dataManager.GetUpdatedTestCalls(ref maxTimeStamp, listPendingCallTestStatus);
            lastCallUpdateOutputs.MaxTimeStamp = maxTimeStamp;
            return lastCallUpdateOutputs;
        }

        public List<TestCall> GetTestCalls(List<CallTestStatus> listCallTestStatus)
        {
            ITestCallDataManager dataManager = CliTesterDataManagerFactory.GetDataManager<ITestCallDataManager>();
            return dataManager.GetTestCalls(listCallTestStatus);
        }

        public IDataRetrievalResult<TestCallDetail> GetFilteredTestCalls(DataRetrievalInput<TestCallQuery> input)
        {
            ITestCallDataManager dataManager = CliTesterDataManagerFactory.GetDataManager<ITestCallDataManager>();
            Vanrise.Entities.BigResult<TestCallDetail> testCallDetails = dataManager.GetTestCallFilteredFromTemp(input);
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, testCallDetails);
        }

        public List<Vanrise.Entities.TemplateConfig> GetInitiateTestTemplates()
        {
            TemplateConfigManager manager = new TemplateConfigManager();
            return manager.GetTemplateConfigurations(Constants.CliTesterConnectorInitiateTest);
        }

        public List<Vanrise.Entities.TemplateConfig> GetTestProgressTemplates()
        {
            TemplateConfigManager manager = new TemplateConfigManager();
            return manager.GetTemplateConfigurations(Constants.CliTesterConnectorTestProgress);
        }

        #region Private Members

        private readonly Dictionary<string, Country> _dictionaryCountries = new Dictionary<string, Country>();
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
        #endregion
    }

}

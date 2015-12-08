using QM.CLITester.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace QM.CLITester.iTestIntegration
{
    public class CLITesterConnector : ICLITesterConnector
    {
        const string GoodAmpersand = "&amp;";
        public InitiateTestOutput InitiateTest(IInitiateTestContext context)
        {
            ServiceActions serviceActions = new ServiceActions();
            return ResponseTestCall(serviceActions.PostRequest("2012", "&profid=4992&vendid=" + context.Supplier.SupplierId + "&ndbccgid=" + context.Country.CountryId + "&ndbcgid=" + context.Zone.ZoneId));
        }

        public GetTestProgressOutput GetTestProgress(IGetTestProgressContext context)
        {
            ServiceActions serviceActions = new ServiceActions();
            return responseTestCallResult(serviceActions.PostRequest("3011", "&jid=" + ((InitiateTestInformation)(context.InitiateTestInformation)).Test_ID), ((InitiateTestInformation)(context.InitiateTestInformation)).Test_ID);
        }

        private InitiateTestOutput ResponseTestCall(string response)
        {
            InitiateTestOutput testOutput = new InitiateTestOutput();
            InitiateTestInformation initiateTestInformation = new InitiateTestInformation();
            Regex badAmpersand = new Regex("&(?![a-zA-Z]{2,6};|#[0-9]{2,4};)");
            response = badAmpersand.Replace(response, GoodAmpersand);
            XmlDocument xml = new XmlDocument();
            if (!String.IsNullOrEmpty(response))
            {
                xml.LoadXml(response);

                XmlNodeList xnList = xml.SelectNodes("/Test_Initiation/Test");
                if (xnList != null)
                {

                    initiateTestInformation.Test_ID = xnList[0]["Test_ID"] != null ? xnList[0]["Test_ID"].InnerText : "";
                    if (!String.IsNullOrEmpty(initiateTestInformation.Test_ID))
                    {
                        testOutput.InitiateTestInformation = initiateTestInformation;
                        testOutput.Result = InitiateTestResult.Created;
                    }
                    else
                    {
                        testOutput.Result = InitiateTestResult.FailedWithRetry;
                    }
                }
                return testOutput;
            }

            testOutput.Result = InitiateTestResult.FailedWithNoRetry;
            return testOutput;
        }

        private GetTestProgressOutput responseTestCallResult(string response, string test_Id)
        {
            GetTestProgressOutput testProgressOutput = new GetTestProgressOutput();
            Regex badAmpersand = new Regex("&(?![a-zA-Z]{2,6};|#[0-9]{2,4};)");
            response = badAmpersand.Replace(response, GoodAmpersand);

            response = response.Replace("<" + test_Id + ">", "<_" + test_Id + ">");

            response = response.Replace("</" + test_Id + ">", "</_" + test_Id + ">");

            XmlDocument xml = new XmlDocument();
            if (!String.IsNullOrEmpty(response))
            {
                xml.LoadXml(response);

                XmlNodeList xnList = xml.SelectNodes("/Test_Status/_" + test_Id);
                if (xnList != null)
                {
                    TestProgress testProgress = new TestProgress
                    {
                        Name = xnList[0]["Name"] != null ? xnList[0]["Name"].InnerText : "",
                        Calls_Total = xnList[0]["Calls_Total"] != null ? Int32.Parse(xnList[0]["Calls_Total"].InnerText) : 0,
                        Calls_Complete = xnList[0]["Calls_Complete"] != null ? Int32.Parse(xnList[0]["Calls_Complete"].InnerText) : 0,
                        CLI_Success = xnList[0]["CLI_Success"] != null ? Int32.Parse(xnList[0]["CLI_Success"].InnerText) : 0,
                        CLI_No_Result = xnList[0]["CLI_No_Result"] != null ? Int32.Parse(xnList[0]["CLI_No_Result"].InnerText) : 0,
                        CLI_Fail = xnList[0]["CLI_Fail"] != null ? Int32.Parse(xnList[0]["CLI_Fail"].InnerText) : 0,
                        PDD = xnList[0]["PDD"] != null ? Int32.Parse(xnList[0]["PDD"].InnerText) : 0,
                        Share_URL = xnList[0]["Share_URL"] != null ? xnList[0]["Share_URL"].InnerText : "",
                    };

                    testProgressOutput.TestProgress = testProgress;
                    if (testProgress.Calls_Total == testProgress.Calls_Complete)
                    {
                        testProgressOutput.Result = GetTestProgressResult.TestCompleted;
                        testProgressOutput.CallTestResult = CallTestResult.Succeeded;
                        return testProgressOutput;
                    }

                    testProgressOutput.Result = GetTestProgressResult.ProgressChanged;
                    //testProgressOutput.Result = (testProgress.Calls_Total == testProgress.Calls_Complete)
                    //    ? GetTestProgressResult.TestCompleted
                    //    : GetTestProgressResult.ProgressChanged;
                    //testProgressOutput.CallTestResult = CallTestResult.NotCompleted;
                    return testProgressOutput;
                }
            }

            testProgressOutput.Result = GetTestProgressResult.FailedWithNoRetry;
            return testProgressOutput;
        }
    }
}

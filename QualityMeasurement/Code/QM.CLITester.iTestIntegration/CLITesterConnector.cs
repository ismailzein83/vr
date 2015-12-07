using QM.CLITester.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Vanrise.Common;

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
            return responseTestCallResult(serviceActions.PostRequest("3011", "&jid=" + Vanrise.Common.Serializer.Deserialize<InitiateTestInformation>(context.InitiateTestInformation.ToString())), context);
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
                        testOutput.InitiateTestInformation = Vanrise.Common.Serializer.Serialize(initiateTestInformation);
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

        private GetTestProgressOutput responseTestCallResult(string response, IGetTestProgressContext context)
        {
            GetTestProgressOutput testProgressOutput = new GetTestProgressOutput();
            Regex badAmpersand = new Regex("&(?![a-zA-Z]{2,6};|#[0-9]{2,4};)");
            response = badAmpersand.Replace(response, GoodAmpersand);

            response = response.Replace("<" + context.InitiateTestInformation + ">", "<_" + context.InitiateTestInformation + ">");

            response = response.Replace("</" + context.InitiateTestInformation + ">", "</_" + context.InitiateTestInformation + ">");

            XmlDocument xml = new XmlDocument();
            if (!String.IsNullOrEmpty(response))
            {
                xml.LoadXml(response);

                XmlNodeList xnList = xml.SelectNodes("/Test_Status/_" + context.InitiateTestInformation);
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

                    testProgressOutput.TestProgress = Vanrise.Common.Serializer.Serialize(testProgress);
                    testProgressOutput.Result = (testProgress.Calls_Total == testProgress.Calls_Complete)
                        ? GetTestProgressResult.TestCompleted
                        : GetTestProgressResult.ProgressChanged;
                    return testProgressOutput;
                }
            }

            testProgressOutput.Result = GetTestProgressResult.FailedWithNoRetry;
            return testProgressOutput;
        }
    }
}

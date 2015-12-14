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
            string itestSupplierId = null;
            if(context.Supplier.Settings != null && context.Supplier.Settings.ExtendedSettings != null)
            {
                SupplierExtensionSettings supplierITestSettings = context.Supplier.Settings.ExtendedSettings.Where(itm => itm is SupplierExtensionSettings).FirstOrDefault() as SupplierExtensionSettings;
                if (supplierITestSettings != null)
                    itestSupplierId = supplierITestSettings.ITestSupplierId;
            }
            if (itestSupplierId == null)
                return new InitiateTestOutput
                {
                    Result = InitiateTestResult.FailedWithNoRetry,
                    FailureMessage = "Missing Supplier Configuration!"
                };

            return ResponseTestCall(serviceActions.PostRequest("2012", "&profid=4992&vendid=" + itestSupplierId + "&ndbccgid=" + context.Country.CountryId + "&ndbcgid=" + context.Zone.ZoneId));
        }

        public GetTestProgressOutput GetTestProgress(IGetTestProgressContext context)
        {
            ServiceActions serviceActions = new ServiceActions();
            return responseTestCallResult(serviceActions.PostRequest("3011", "&jid=" + ((InitiateTestInformation)(context.InitiateTestInformation)).Test_ID), ((InitiateTestInformation)(context.InitiateTestInformation)).Test_ID, context.RecentTestProgress);
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

            testOutput.Result = InitiateTestResult.FailedWithRetry;
            return testOutput;
        }

        private GetTestProgressOutput responseTestCallResult(string response, string testId, Object recentTestProgress)
        {
            GetTestProgressOutput testProgressOutput = new GetTestProgressOutput();
            Regex badAmpersand = new Regex("&(?![a-zA-Z]{2,6};|#[0-9]{2,4};)");
            response = badAmpersand.Replace(response, GoodAmpersand);

            response = response.Replace("<" + testId + ">", "<_" + testId + ">");

            response = response.Replace("</" + testId + ">", "</_" + testId + ">");

            XmlDocument xml = new XmlDocument();
            if (!String.IsNullOrEmpty(response))
            {
                xml.LoadXml(response);

                XmlNodeList xnList = xml.SelectNodes("/Test_Status/_" + testId);
                if (xnList != null)
                {
                    TestProgress testProgress = new TestProgress
                    {
                        Name = xnList[0]["Name"] != null ? xnList[0]["Name"].InnerText : "",
                        TotalCalls = xnList[0]["Calls_Total"] != null ? Int32.Parse(xnList[0]["Calls_Total"].InnerText) : 0,
                        CompletedCalls = xnList[0]["Calls_Complete"] != null ? Int32.Parse(xnList[0]["Calls_Complete"].InnerText) : 0,
                        CliSuccess = xnList[0]["CLI_Success"] != null ? Int32.Parse(xnList[0]["CLI_Success"].InnerText) : 0,
                        CliNoResult = xnList[0]["CLI_No_Result"] != null ? Int32.Parse(xnList[0]["CLI_No_Result"].InnerText) : 0,
                        CliFail = xnList[0]["CLI_Fail"] != null ? Int32.Parse(xnList[0]["CLI_Fail"].InnerText) : 0,
                        Pdd = xnList[0]["PDD"] != null ? Int32.Parse(xnList[0]["PDD"].InnerText) : 0,
                        ShareUrl = xnList[0]["Share_URL"] != null ? xnList[0]["Share_URL"].InnerText : ""
                    };

                    testProgressOutput.TestProgress = testProgress;
                    if (testProgress.TotalCalls == testProgress.CompletedCalls)
                    {
                        testProgressOutput.Result = GetTestProgressResult.TestCompleted;

                        testProgressOutput.CallTestResult = (testProgress.CliFail == testProgress.TotalCalls) ? CallTestResult.Failed :
                            (testProgress.CliSuccess == testProgress.TotalCalls) ? CallTestResult.Succeeded :
                            (testProgress.CliNoResult == testProgress.TotalCalls ? CallTestResult.NotAnswered : CallTestResult.PartiallySucceeded); 
                        
                        return testProgressOutput;
                    }

                    if(recentTestProgress != null)
                    testProgressOutput.Result = ((((TestProgress)recentTestProgress).CliFail == ((TestProgress)testProgressOutput.TestProgress).CliFail) &&
                    (((TestProgress)recentTestProgress).CliNoResult == ((TestProgress)testProgressOutput.TestProgress).CliNoResult) &&
                    (((TestProgress)recentTestProgress).CliSuccess == ((TestProgress)testProgressOutput.TestProgress).CliSuccess) &&
                    (((TestProgress)recentTestProgress).CompletedCalls == ((TestProgress)testProgressOutput.TestProgress).CompletedCalls) &&
                    (((TestProgress)recentTestProgress).Pdd == ((TestProgress)testProgressOutput.TestProgress).Pdd) &&
                    (((TestProgress)recentTestProgress).TotalCalls == ((TestProgress)testProgressOutput.TestProgress).TotalCalls)) ?
                        GetTestProgressResult.ProgressNotChanged : 
                        GetTestProgressResult.ProgressChanged;
                    else
                    {
                        testProgressOutput.Result =  testProgressOutput.TestProgress == null ? GetTestProgressResult.ProgressNotChanged :
                        GetTestProgressResult.ProgressChanged;
                    }
                    return testProgressOutput;
                }
            }

            testProgressOutput.Result = GetTestProgressResult.FailedWithRetry;
            return testProgressOutput;
        }
    }
}

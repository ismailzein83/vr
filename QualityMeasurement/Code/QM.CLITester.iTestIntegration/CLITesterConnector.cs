using QM.CLITester.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using QM.CLITester.Business;
using Vanrise.Common.Business;
using QM.BusinessEntity.Business;
namespace QM.CLITester.iTestIntegration
{
    public class CLITesterConnector : CLITesterConnectorBase
    {
        public override InitiateTestOutput InitiateTest(IInitiateTestContext context)
        {
            if (context.Country == null)
                throw new ArgumentNullException("context.Country");

            if (context.Zone == null)
                throw new ArgumentNullException("context.Zone");

            if (context.Profile == null)
                throw new ArgumentNullException("context.Profile");

            if (context.Supplier == null)
                throw new ArgumentNullException("context.Supplier");


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

            string itestProfileId = context.Profile.SourceId;
            //if (context.Profile.Settings != null && context.Profile.Settings.ExtendedSettings != null)
            //{
            //    ProfileExtensionSettings profileITestSettings = context.Profile.Settings.ExtendedSettings.Where(itm => itm is ProfileExtensionSettings).FirstOrDefault() as ProfileExtensionSettings;
            //    if (profileITestSettings != null)
            //        itestSupplierId = profileITestSettings.ITestProfileId;
            //}
            if (itestProfileId == null)
                return new InitiateTestOutput
                {
                    Result = InitiateTestResult.FailedWithNoRetry,
                    FailureMessage = "Missing Profile Configuration!"
                };

            string itestCountryId = context.Country.SourceId;//Temporary

            if (itestCountryId == null)
                return new InitiateTestOutput
                {
                    Result = InitiateTestResult.FailedWithNoRetry,
                    FailureMessage = "Missing Country Configuration!"
                };

            string itestBreakoutId = context.Zone.SourceId;

            if (itestBreakoutId == null)
                return new InitiateTestOutput
                {
                    Result = InitiateTestResult.FailedWithNoRetry,
                    FailureMessage = "Missing Breakout Configuration!"
                };
            return ResponseInitiateTest(serviceActions.PostRequest("2012", String.Format("&profid={0}&vendid={1}&ndbccgid={2}&ndbcgid={3}", itestProfileId, itestSupplierId, itestCountryId, itestBreakoutId)));
        }

        public override GetTestProgressOutput GetTestProgress(IGetTestProgressContext context)
        {
            ServiceActions serviceActions = new ServiceActions();
            return ResponseTestProgress(
                serviceActions.PostRequest("3011", String.Format("&jid={0}", ((InitiateTestInformation)(context.InitiateTestInformation)).Test_ID)),
                ((InitiateTestInformation)(context.InitiateTestInformation)).Test_ID, context.RecentTestProgress, context.RecentMeasure);
        }

        #region Private Members

        private InitiateTestOutput ResponseInitiateTest(string response)
        {
            InitiateTestOutput testOutput = new InitiateTestOutput();    
            InitiateTestInformation initiateTestInformation = new InitiateTestInformation();

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
                        testOutput.FailureMessage = response;
                        testOutput.Result = InitiateTestResult.FailedWithRetry;
                    }
                }
                return testOutput;
            }

            testOutput.Result = InitiateTestResult.FailedWithRetry;
            return testOutput;
        }

        private GetTestProgressOutput ResponseTestProgress(string response, string testId, Object recentTestProgress, Measure recentMeasure)
        {
            GetTestProgressOutput testProgressOutput = new GetTestProgressOutput();

            response = response.Replace("<" + testId + ">", "<_" + testId + ">");
            response = response.Replace("</" + testId + ">", "</_" + testId + ">");

            XmlDocument xml = new XmlDocument();
            if (!String.IsNullOrEmpty(response))
            {
                xml.LoadXml(response);
                
                XmlNodeList xnList = xml.SelectNodes("/Test_Status/_" + testId);
                
                if (xnList != null)
                {
                    var node = xnList[0];

                    Measure resultTestProgress = new Measure
                    {
                        Pdd = node["PDD"] != null ? Decimal.Parse(node["PDD"].InnerText) : 0,
                        Mos = 0,
                        Duration = null,
                        ReleaseCode = null,
                        ReceivedCli = null,
                        RingDuration = null
                    };

                    TestProgress testProgress = new TestProgress
                    {
                        Name = node["Name"] != null ? node["Name"].InnerText : "",
                        TotalCalls = node["Calls_Total"] != null ? Int32.Parse(node["Calls_Total"].InnerText) : 0,
                        CompletedCalls = node["Calls_Complete"] != null ? Int32.Parse(node["Calls_Complete"].InnerText) : 0,
                        CliSuccess = node["CLI_Success"] != null ? Int32.Parse(node["CLI_Success"].InnerText) : 0,
                        CliNoResult = node["CLI_No_Result"] != null ? Int32.Parse(node["CLI_No_Result"].InnerText) : 0,
                        CliFail = node["CLI_Fail"] != null ? Int32.Parse(node["CLI_Fail"].InnerText) : 0,
                        ShareUrl = node["Share_URL"] != null ? node["Share_URL"].InnerText : ""
                    };

                    testProgressOutput.Measure = resultTestProgress;
                    testProgressOutput.TestProgress = testProgress;
                    if (testProgress.TotalCalls == testProgress.CompletedCalls)
                    {
                        testProgressOutput.Result = GetTestProgressResult.TestCompleted;

                        testProgressOutput.CallTestResult = (testProgress.CliFail == testProgress.TotalCalls) ? CallTestResult.Failed :
                            (testProgress.CliSuccess == testProgress.TotalCalls) ? CallTestResult.Succeeded :
                            (testProgress.CliNoResult == testProgress.TotalCalls ? CallTestResult.NotAnswered : CallTestResult.PartiallySucceeded); 
                        
                        return testProgressOutput;
                    }

                    if(recentTestProgress != null || recentMeasure != null)
                        testProgressOutput.Result = CompareTestProgress((TestProgress)recentTestProgress, (TestProgress)testProgressOutput.TestProgress, recentMeasure, testProgressOutput.Measure) ?
                            GetTestProgressResult.ProgressNotChanged : GetTestProgressResult.ProgressChanged;
                    else
                        testProgressOutput.Result =  (testProgressOutput.TestProgress == null && testProgressOutput.Measure == null) ? GetTestProgressResult.ProgressNotChanged :
                        GetTestProgressResult.ProgressChanged;

                    return testProgressOutput;
                }
            }

            testProgressOutput.Result = GetTestProgressResult.FailedWithRetry;
            return testProgressOutput;
        }

        private bool CompareTestProgress(TestProgress recentTestProgress, TestProgress testProgress, Measure recentMeasure, Measure measure)
        {
            bool same = true;
            if (recentTestProgress != null)
            {
                same = ((recentTestProgress.CliFail == testProgress.CliFail) && (recentTestProgress.CliNoResult == testProgress.CliNoResult) &&
                    (recentTestProgress.CliSuccess == testProgress.CliSuccess) && (recentTestProgress.CompletedCalls == testProgress.CompletedCalls)
                    && (recentTestProgress.TotalCalls == testProgress.TotalCalls)
                    );
            }

            if (recentMeasure != null)
            {
                same = ((recentMeasure.Pdd == measure.Pdd) && (recentMeasure.Mos == measure.Mos) &&

                    (recentMeasure.Duration == measure.Duration) && (recentMeasure.ReleaseCode == measure.ReleaseCode) &&
                    (recentMeasure.ReceivedCli == measure.ReceivedCli) && (recentMeasure.RingDuration == measure.RingDuration)
                    );
            }

            return same;
        }
        #endregion
    }
}

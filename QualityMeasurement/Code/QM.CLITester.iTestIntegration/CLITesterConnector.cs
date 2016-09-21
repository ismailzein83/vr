using QM.CLITester.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using QM.CLITester.Business;
using Vanrise.Common.Business;
using QM.BusinessEntity.Business;
using QM.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Entities;

namespace QM.CLITester.iTestIntegration
{
    public class CLITesterConnector : CLITesterConnectorBase
    {
        public override Guid ConfigId { get { return new Guid("9f336216-bf7a-4e5a-a327-cc19de2362d5"); } }

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
            ITestExtendedSupplierSetting itestSupplierSettings = null;
            if (context.Supplier.Settings != null && context.Supplier.Settings.ExtendedSettings != null
                && context.Supplier.Settings.ExtendedSettings.ContainsKey(ITestExtendedSupplierSettingBehavior.EXTENDEDSUPPLIERSETTING_KEYNAME))
                itestSupplierSettings = context.Supplier.Settings.ExtendedSettings[ITestExtendedSupplierSettingBehavior.EXTENDEDSUPPLIERSETTING_KEYNAME] as ITestExtendedSupplierSetting;

            if (itestSupplierSettings == null)
                return new InitiateTestOutput
                {
                    Result = InitiateTestResult.FailedWithNoRetry,
                    FailureMessage = "Missing Supplier Configuration!"
                };

            string itestProfileId = context.Profile.SourceId;
            int itestQuantity = context.Quantity;
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

            ITestExtendedZoneSetting itestZoneSettings = null;
            if (context.Zone.Settings != null && context.Zone.Settings.ExtendedSettings != null
                && context.Zone.Settings.ExtendedSettings.ContainsKey(ITestExtendedZoneSettingBehavior.EXTENDEDZONESETTING_KEYNAME))
                itestZoneSettings = context.Zone.Settings.ExtendedSettings[ITestExtendedZoneSettingBehavior.EXTENDEDZONESETTING_KEYNAME] as ITestExtendedZoneSetting;

            if (itestZoneSettings == null)
                return new InitiateTestOutput
                {
                    Result = InitiateTestResult.FailedWithNoRetry,
                    FailureMessage = "Missing Breakout Configuration!"
                };

            return ResponseInitiateTest(serviceActions.PostRequest("2012", String.Format("&profid={0}&vendid={1}&ndbccgid={2}&ndbcgid={3}&ndbqty={4}", itestProfileId, itestSupplierSettings.ITestSupplierId, itestZoneSettings.ITestCountryId, itestZoneSettings.ITestZoneId, itestQuantity)));
        }

        public override GetTestProgressOutput GetTestProgress(IGetTestProgressContext context)
        {
            ServiceActions serviceActions = new ServiceActions();
            //return ResponseTestProgress(
            //    serviceActions.PostRequest("3011", String.Format("&jid={0}", ((InitiateTestInformation)(context.InitiateTestInformation)).Test_ID)),
            //    ((InitiateTestInformation)(context.InitiateTestInformation)).Test_ID, context.RecentTestProgress, context.RecentMeasure);

            return ResponseTestProgressBeta(
                serviceActions.PostRequestBeta("3024", String.Format("&jid={0}", ((InitiateTestInformation)(context.InitiateTestInformation)).Test_ID)),
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
                        //ReleaseCode = null,
                        //ReceivedCli = null,
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
                        ShareUrl = node["Share_URL"] != null ? node["Share_URL"].InnerText : "",
                        XmlResponse = response
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
        private GetTestProgressOutput ResponseTestProgressBeta(string response, string testId, Object recentTestProgress, Measure recentMeasure)
        {
            GetTestProgressOutput testProgressOutput = new GetTestProgressOutput();
            
            XmlDocument xml = new XmlDocument();
            if (!String.IsNullOrEmpty(response))
            {
                xml.LoadXml(response);

                XmlNodeList xnListCall = xml.SelectNodes("/Test_Status/Call");
                XmlNodeList xnListTestOverView = xml.SelectNodes("/Test_Status/Test_Overview");
                if (xnListCall != null && xnListTestOverView!= null)
                {
                    string xmlResponse = Regex.Replace(response, @"\t|\n|\r", "");
                    
                    var node = xnListTestOverView[0];
                    
                    var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    long unixTime = node["Init"] != null ? long.Parse(node["Init"].InnerText) : 0;
                    decimal pdd = 0;
                    decimal mos = 0;
                    int countNotCompleted = 0; //Processing
                    int countSucceeded = 0; // CLI Succcess
                    int countFailed = 0; //CLI Failed
                    int countNotAnswered = 0; //Call Failed
                    int countFas = 0;
                    int countTotalCalls = xnListCall.Count;
                    TestProgress testProgress = new TestProgress
                    {
                        Name = node["Name"] != null ? node["Name"].InnerText : "",
                        //Result = node["Result"] != null ? node["Result"].InnerText : "",
                        CallResults = new List<CallResult>(),
                        XmlResponse = xmlResponse
                    };

                    foreach (XmlNode callNode in xnListCall)
                    {
                        pdd = pdd + Decimal.Parse(callNode["PDD"].InnerText);
                        mos = mos + Decimal.Parse(callNode["MOS"].InnerText);
                        CallResult callResult = new CallResult
                        {
                            Source = callNode["Source"] != null ? callNode["Source"].InnerText : "",
                            Destination = callNode["Destination"] != null ? callNode["Destination"].InnerText : "",
                            Ring = callNode["Ring"] != null ? callNode["Ring"].InnerText : "",
                            Call = callNode["Call"] != null ? callNode["Call"].InnerText : "",
                            ReleaseCode = callNode["Last_Code"] != null ? callNode["Last_Code"].InnerText : "",
                            ReceivedCli = callNode["CLI"] != null ? callNode["CLI"].InnerText : "",
                            Pdd = callNode["PDD"] != null ? callNode["PDD"].InnerText : "",
                            Mos = callNode["MOS"] != null ? callNode["MOS"].InnerText : "",
                            CallTestResult = CallTestResult.PartiallySucceeded
                        };
                        string callTestResult = callNode["Result"] != null ? callNode["Result"].InnerText : "";
                        callResult.CallTestResult = GetCallTestResult(callTestResult);

                        callResult.CallTestResultDescription =
                            Utilities.GetEnumAttribute<CallTestResult, DescriptionAttribute>(callResult.CallTestResult).Description;

                        switch (callResult.CallTestResult)
                        {
                            case CallTestResult.Succeeded:
                                countSucceeded++;
                                countNotCompleted++;
                                break;
                            case CallTestResult.Failed:
                                countFailed++;
                                countNotCompleted++;
                                break;
                            case CallTestResult.NotAnswered:
                                countNotAnswered++;
                                countNotCompleted++;
                                break;
                            case CallTestResult.Fas:
                                countFas++;
                                countNotCompleted++;
                                break;
                        }
                        testProgress.CallResults.Add(callResult);
                    }

                    Measure resultTestProgress = new Measure
                    {
                        Pdd = pdd / countTotalCalls,
                        Mos = mos / countTotalCalls,
                        Duration = epoch.AddSeconds(unixTime),
                        RingDuration = null
                    };
                    testProgressOutput.Measure = resultTestProgress;
                    testProgressOutput.TestProgress = testProgress;

                    if (countNotCompleted == countTotalCalls)
                    {
                        testProgressOutput.Result = GetTestProgressResult.TestCompleted;

                        testProgressOutput.CallTestResult =
                            (countSucceeded == countTotalCalls)
                                ? CallTestResult.Succeeded
                                : (countFailed == countTotalCalls)
                                    ? CallTestResult.Failed
                                    : (countNotAnswered == countTotalCalls)
                                        ? CallTestResult.NotAnswered
                                        : (countFas == countTotalCalls)
                                            ? CallTestResult.Fas
                                            : CallTestResult.PartiallySucceeded;
                        return testProgressOutput;
                    }

                    if (recentTestProgress != null || recentMeasure != null)
                        testProgressOutput.Result = CompareTestProgressBeta((TestProgress)recentTestProgress, (TestProgress)testProgressOutput.TestProgress, recentMeasure, testProgressOutput.Measure) ?
                            GetTestProgressResult.ProgressNotChanged : GetTestProgressResult.ProgressChanged;
                    else
                        testProgressOutput.Result = (testProgressOutput.TestProgress == null && testProgressOutput.Measure == null) ? GetTestProgressResult.ProgressNotChanged :
                        GetTestProgressResult.ProgressChanged;

                    return testProgressOutput;
                }
            }

            testProgressOutput.Result = GetTestProgressResult.FailedWithRetry;
            return testProgressOutput;
        }

        private CallTestResult GetCallTestResult(string testProgressResult)
        {
            var connectorResultMappingManager = new ConnectorResultMappingManager();

            var existingZones = connectorResultMappingManager.GetConnectorResultMappings(Constants.CONNECTOR_TYPE);

            ConnectorResultMapping notCompletedMapping = existingZones != null
                ? existingZones.FirstOrDefault(itm => itm.ResultId == (int)CallTestResult.NotCompleted) : null;

            ConnectorResultMapping notAnsweredMapping = existingZones != null
                ? existingZones.FirstOrDefault(itm => itm.ResultId == (int)CallTestResult.NotAnswered) : null;

            ConnectorResultMapping failedMapping = existingZones != null
                ? existingZones.FirstOrDefault(itm => itm.ResultId == (int)CallTestResult.Failed) : null;

            ConnectorResultMapping succeededMapping = existingZones != null
                ? existingZones.FirstOrDefault(itm => itm.ResultId == (int)CallTestResult.Succeeded) : null;

            ConnectorResultMapping fasMapping = existingZones != null
                ? existingZones.FirstOrDefault(itm => itm.ResultId == (int)CallTestResult.Fas) : null;

            return notCompletedMapping.ConnectorResults.Exists(connRes => testProgressResult == connRes) ? CallTestResult.NotCompleted :
                failedMapping.ConnectorResults.Exists(connRes => testProgressResult == connRes) ? CallTestResult.Failed :
                succeededMapping.ConnectorResults.Exists(connRes => testProgressResult == connRes) ? CallTestResult.Succeeded :
                notAnsweredMapping.ConnectorResults.Exists(connRes => testProgressResult == connRes) ? CallTestResult.NotAnswered :
                fasMapping.ConnectorResults.Exists(connRes => testProgressResult == connRes) ? CallTestResult.Fas :
                CallTestResult.PartiallySucceeded;
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

                    (recentMeasure.Duration == measure.Duration) && 
                    //(recentMeasure.ReleaseCode == measure.ReleaseCode) &&
                    //(recentMeasure.ReceivedCli == measure.ReceivedCli) && 
                    (recentMeasure.RingDuration == measure.RingDuration)
                    );
            }

            return same;
        }
        private bool CompareTestProgressBeta(TestProgress recentTestProgress, TestProgress testProgress, Measure recentMeasure, Measure measure)
        {
            bool same = true;
            if (recentTestProgress != null)
            {
                var firstNotSecond = recentTestProgress.CallResults.Except(testProgress.CallResults).ToList();
                var secondNotFirst = testProgress.CallResults.Except(recentTestProgress.CallResults).ToList();

                same = ((recentTestProgress.Result == testProgress.Result) && (firstNotSecond.Count == 0) && (secondNotFirst.Count == 0));
            }

            if (recentMeasure != null)
            {
                same = ((recentMeasure.Pdd == measure.Pdd) && (recentMeasure.Mos == measure.Mos) &&
                        (recentMeasure.Duration == measure.Duration) && (recentMeasure.RingDuration == measure.RingDuration));
            }

            return same;
        }
        #endregion

        public override void ConvertResultToExcelData(Vanrise.Entities.IConvertResultToExcelDataContext<TestCallDetail> context)
        {
            if (context.BigResult == null)
                throw new ArgumentNullException("context.BigResult");
            if (context.BigResult.Data == null)
                throw new ArgumentNullException("context.BigResult.Data");
            ExportExcelSheet sheet = new ExportExcelSheet();
            sheet.Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() };
            sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Supplier Name" });
            sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Country Name" });
            sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Zone Name" });
            sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Source" });
            sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Destination" });
            sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Received Cli" });
            sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Release Code" });
            sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "PDD" });
            sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "MOS" });
            sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Call Test Result" });
            

            sheet.Rows = new List<ExportExcelRow>();
            foreach (var record in context.BigResult.Data)
            {
                TestProgress testProgress = (TestProgress)record.Entity.TestProgress;
                if (testProgress != null)
                {
                    List<CallResult> callResults = testProgress.CallResults;
                    foreach (CallResult callResult in callResults)
                    {
                        var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                        sheet.Rows.Add(row);
                        row.Cells.Add(new ExportExcelCell { Value = record.SupplierName });
                        row.Cells.Add(new ExportExcelCell { Value = record.CountryName });
                        row.Cells.Add(new ExportExcelCell { Value = record.ZoneName });
                        row.Cells.Add(new ExportExcelCell { Value = callResult.Source });
                        row.Cells.Add(new ExportExcelCell { Value = callResult.Destination });
                        row.Cells.Add(new ExportExcelCell { Value = callResult.ReceivedCli });
                        row.Cells.Add(new ExportExcelCell { Value = callResult.ReleaseCode });
                        row.Cells.Add(new ExportExcelCell { Value = callResult.Pdd });
                        row.Cells.Add(new ExportExcelCell { Value = callResult.Mos });
                        row.Cells.Add(new ExportExcelCell { Value = callResult.CallTestResultDescription });
                    }
                }
            }
            context.MainSheet = sheet;
        }
    }
}

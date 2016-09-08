using QM.BusinessEntity.Entities;
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
    internal class CallTestManager
    {
        internal bool TryInitiateTest(string profileId, string supplierId, string countryId, string zoneId, int quantity,
            out  InitiateTestInformation initiateTestInformation, out string failureMessage)
        {
            ServiceActions serviceActions = new ServiceActions();
            var responseString = serviceActions.PostRequest("2012", String.Format("&profid={0}&vendid={1}&ndbccgid={2}&ndbcgid={3}&ndbqty={4}", profileId, supplierId, countryId, zoneId, quantity));
            

            XmlDocument xml = new XmlDocument();
            if (!String.IsNullOrEmpty(responseString))
            {
                xml.LoadXml(responseString);

                XmlNodeList xnList = xml.SelectNodes("/Test_Initiation/Test");
                if (xnList != null)
                {
                    if (xnList[0]["Test_ID"] != null)
                    {
                        initiateTestInformation = new InitiateTestInformation
                        {
                            Test_ID = xnList[0]["Test_ID"].InnerText
                        };
                        failureMessage = null;
                        return true;
                    }
                    else
                    {
                        initiateTestInformation = null;
                        failureMessage = responseString;
                        return false;
                    }
                }
            }
            initiateTestInformation = null;
            failureMessage = null;
            return false;
        }

        internal string GetProfileITestId(Profile profile)
        {
            return profile.SourceId;
        }

        internal string GetSupplierITestId(Supplier supplier)
        {
            ITestExtendedSupplierSetting itestSupplierSettings = null;
            if (supplier.Settings != null && supplier.Settings.ExtendedSettings != null
                && supplier.Settings.ExtendedSettings.ContainsKey(ITestExtendedSupplierSettingBehavior.EXTENDEDSUPPLIERSETTING_KEYNAME))
                itestSupplierSettings = supplier.Settings.ExtendedSettings[ITestExtendedSupplierSettingBehavior.EXTENDEDSUPPLIERSETTING_KEYNAME] as ITestExtendedSupplierSetting;

            if (itestSupplierSettings == null)
                throw new NullReferenceException(String.Format("itestSupplierSettings. Supplier Id '{0}'", supplier.SupplierId));
            return itestSupplierSettings.ITestSupplierId;   
        }

        internal bool TryTestProgress(string testId, Object recentTestProgress, Measure recentMeasure,
            out Measure measure, out TestProgress testProgress, out GetTestProgressResult result, out CallTestResult? callTestResult)
        {
            ServiceActions serviceActions = new ServiceActions();

            var responseString = serviceActions.PostRequestBeta("3024", String.Format("&jid={0}", testId));

            XmlDocument xml = new XmlDocument();
            if (!String.IsNullOrEmpty(responseString))
            {
                xml.LoadXml(responseString);

                XmlNodeList xnListCall = xml.SelectNodes("/Test_Status/Call");
                XmlNodeList xnListTestOverView = xml.SelectNodes("/Test_Status/Test_Overview");
                if (xnListCall != null && xnListTestOverView != null)
                {
                    string xmlResponse = Regex.Replace(responseString, @"\t|\n|\r", "");

                    var node = xnListCall[0];
                    var node2 = xnListTestOverView[0];

                    var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    long unixTime = node["Start"] != null ? long.Parse(node["Start"].InnerText) : 0;

                    Measure resultTestProgress = new Measure
                    {
                        Pdd = node["PDD"] != null ? Decimal.Parse(node["PDD"].InnerText) : 0,
                        Mos = node["MOS"] != null ? Decimal.Parse(node["MOS"].InnerText) : 0,
                        Duration = epoch.AddSeconds(unixTime),
                        RingDuration = null
                    };

                    testProgress = new TestProgress
                    {
                        Name = node2["Name"] != null ? node2["Name"].InnerText : "",
                        Result = node["Result"] != null ? node["Result"].InnerText : "",
                        CallResults = new List<CallResult>(),
                        XmlResponse = xmlResponse
                    };
                    foreach (XmlNode callNode in xnListCall)
                    {
                        CallResult callResult = new CallResult
                        {
                            Source = callNode["Source"] != null ? callNode["Source"].InnerText : "",
                            Destination = callNode["Destination"] != null ? callNode["Destination"].InnerText : "",
                            Ring = callNode["Ring"] != null ? callNode["Ring"].InnerText : "",
                            Call = callNode["Call"] != null ? callNode["Call"].InnerText : "",
                            ReleaseCode = callNode["Last_Code"] != null ? callNode["Last_Code"].InnerText : "",
                            ReceivedCli = callNode["CLI"] != null ? callNode["CLI"].InnerText : ""
                        };
                        testProgress.CallResults.Add(callResult);
                    }
                    measure = resultTestProgress;
                    if (testProgress.Result != "Processing" && testProgress.Result != "Awaiting CLI Result")
                    {
                        result = GetTestProgressResult.TestCompleted;

                        callTestResult = (testProgress.Result == "CLI Failure") ? CallTestResult.Failed :
                            (testProgress.Result == "CLI Success") ? CallTestResult.Succeeded :
                            (testProgress.Result == "Call Failure" || testProgress.Result == "Call Timeout" || testProgress.Result == "No answer") ? CallTestResult.NotAnswered :
                            (testProgress.Result == "Terminated elsewhere") ? CallTestResult.Fas :
                            CallTestResult.PartiallySucceeded;

                        return true;
                    }

                    if (recentTestProgress != null || recentMeasure != null)
                        result = CompareTestProgressBeta((TestProgress)recentTestProgress, testProgress, recentMeasure, measure) ?
                            GetTestProgressResult.ProgressNotChanged : GetTestProgressResult.ProgressChanged;
                    else
                        result = (testProgress == null && measure == null) ? GetTestProgressResult.ProgressNotChanged :
                        GetTestProgressResult.ProgressChanged;
                    callTestResult = null;
                    return true;
                }
            }
            result = GetTestProgressResult.FailedWithRetry;
            callTestResult = null;
            measure = null;
            testProgress = null;
            return false;
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
    }
}

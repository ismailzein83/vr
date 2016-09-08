using System;
using System.CodeDom;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QM.BusinessEntity.Business;
using QM.BusinessEntity.Entities;
using QM.CLITester.Business;
using QM.CLITester.Entities;
using Vanrise.Common.Business;
using Vanrise.Runtime.Entities;
using System.Threading;
using System.Threading.Tasks;
using Vanrise.Common;

namespace QM.CLITester.iTestIntegration
{

    public class VIConnectorSyncTaskAction : SchedulerTaskAction
    {
        public override SchedulerTaskExecuteOutput Execute(SchedulerTask task, BaseTaskActionArgument taskActionArgument, Dictionary<string, object> evaluatedExpressions)
        {
            VIConnectorSyncTaskActionArgument vIConnectorSyncTaskActionArgument = taskActionArgument as VIConnectorSyncTaskActionArgument;
  
            CallTestManager callTestManager = new CallTestManager();

            BusinessEntity.Business.SupplierManager supplierManager = new SupplierManager();
            var supplier = supplierManager.GetSupplier(vIConnectorSyncTaskActionArgument.SupplierId);
            if(supplier == null)
                throw new ArgumentNullException(String.Format("supplier '{0}'",vIConnectorSyncTaskActionArgument.SupplierId ));

            string supplierITestId = callTestManager.GetSupplierITestId(supplier);

            ProfileManager profileManager = new ProfileManager();
            var profile = profileManager.GetProfile(vIConnectorSyncTaskActionArgument.ProfileId);
            if(profile == null)
                throw new ArgumentNullException(String.Format("profile '{0}'", vIConnectorSyncTaskActionArgument.ProfileId));

            string profileITestId = callTestManager.GetProfileITestId(profile);

            var zoneInfoManager = new ConnectorZoneInfoManager();

            ITestZoneManager iTestZoneManager = new ITestZoneManager();
            Dictionary<string, ITestZone> allITestZones = iTestZoneManager.GetAllZones();

            if (allITestZones != null)
            {
                ConcurrentQueue<ITestZone> qITestZones = new ConcurrentQueue<ITestZone>(allITestZones.Values);
                Parallel.For(0, vIConnectorSyncTaskActionArgument.ParallelThreadsCount, (i) =>
                    {
                        ITestZone itestZone;
                        while(qITestZones.TryDequeue(out itestZone))
                        {
                            InitiateTestInformation initiateTestInformation;
                            string failureMessage;
                            Measure measure = new Measure();
                            TestProgress testProgress = new TestProgress();

                            if(callTestManager.TryInitiateTest(profileITestId, supplierITestId, itestZone.CountryId, itestZone.ZoneId,  vIConnectorSyncTaskActionArgument.Quantity,
                                out initiateTestInformation, out failureMessage))
                            {
                                int retryCount = 0;
                                while(retryCount <= vIConnectorSyncTaskActionArgument.MaximumRetryCount)
                                {
                                    GetTestProgressResult result;
                                    CallTestResult? callTestResult;
                                    bool tryTestProgress = callTestManager.TryTestProgress(
                                        initiateTestInformation.Test_ID, null, null,
                                        out measure, out testProgress, out result, out callTestResult);
                                    if (tryTestProgress && callTestResult!= null)
                                    {
                                        //string code = (testProgress.CallResults.First().Destination).Substring(supplier.Settings.Prefix.Length,
                                        //    ((testProgress.CallResults.First().Destination).Length - supplier.Settings.Prefix.Length));
                                       List<ConnectorZoneInfoToUpdate> listConnectorZoneInfoToUpdates = new List<ConnectorZoneInfoToUpdate>()
                                        {
                                            new ConnectorZoneInfoToUpdate()
                                            {
                                                ConnectorZoneId = itestZone.ZoneId,
                                                Codes = new List<string>() { testProgress.CallResults.First().Destination }
                                            }
                                        };

                                       zoneInfoManager.UpdateZones(Constants.CONNECTOR_TYPE, listConnectorZoneInfoToUpdates);

                                        LogInformation("Test Zone added: '{0} - {1}'", itestZone.CountryName, itestZone.ZoneName);
                                        retryCount = vIConnectorSyncTaskActionArgument.MaximumRetryCount + 1;
                                    }
                                    if (!tryTestProgress)
                                        retryCount++;
                                    Thread.Sleep(vIConnectorSyncTaskActionArgument.DownloadResultWaitTime * 1000);
                                }
                            }
                            else
                            {
                                LogWarning("Test Initiation failed to Zone '{0} - {1}'", itestZone.CountryName, itestZone.ZoneName);
                            }
                        }
                    });
            }

            SchedulerTaskExecuteOutput output = new SchedulerTaskExecuteOutput();

            output = new SchedulerTaskExecuteOutput()
            {
                Result = ExecuteOutputResult.Completed
            };
            return output;
        }

        private void LogWarning(string messageFormat, params object[] args)
        {
            Vanrise.Common.LoggerFactory.GetLogger().WriteWarning(messageFormat, args);
        }

        private void LogInformation(string messageFormat, params object[] args)
        {
            Vanrise.Common.LoggerFactory.GetLogger().WriteInformation(messageFormat, args);
        }

        //private static ConnectorZoneInfoToUpdate TestCall(VIConnectorSyncTaskActionArgument vIConnectorSyncTaskActionArgument,
        //    QM.BusinessEntity.Entities.Zone zone)
        //{
        //    ConnectorZoneInfoToUpdate zoneinfo = new ConnectorZoneInfoToUpdate();

        //    ProfileManager profileManager = new ProfileManager();
        //    SupplierManager supplierManager = new SupplierManager();
        //    CountryManager countryManager = new CountryManager();

        //    DateTime creationDate = DateTime.Now;
        //        var initiateTestContext = new InitiateTestContext()
        //    {
        //        Supplier = supplierManager.GetSupplier(vIConnectorSyncTaskActionArgument.SupplierId),
        //        Profile = profileManager.GetProfile(vIConnectorSyncTaskActionArgument.ProfileId),
        //        Country = countryManager.GetCountry(zone.CountryId),
        //        Zone = zone
        //    };

        //    InitiateTestOutput initiateTestOutput = new InitiateTestOutput();
        //    try
        //    {
        //        initiateTestOutput =
        //            vIConnectorSyncTaskActionArgument.CLITestConnector.InitiateTest(initiateTestContext);
        //    }
        //    catch (Exception ex)
        //    {
        //        initiateTestOutput.Result = InitiateTestResult.FailedWithRetry;
        //        initiateTestOutput.FailureMessage = ex.Message;
        //    }

        //    CallTestStatus callTestStatus;

        //    switch (initiateTestOutput.Result)
        //    {
        //        case InitiateTestResult.Created:
        //            callTestStatus = CallTestStatus.Initiated;
        //            break;
        //        case InitiateTestResult.FailedWithRetry:
        //            {
        //                callTestStatus = CallTestStatus.InitiationFailedWithRetry;
        //                break;
        //            }

        //        case InitiateTestResult.FailedWithNoRetry:
        //            callTestStatus = CallTestStatus.InitiationFailedWithNoRetry;
        //            break;
        //        default:
        //            callTestStatus = CallTestStatus.InitiationFailedWithRetry;
        //            break;
        //    }
        //    if (callTestStatus == CallTestStatus.Initiated)
        //    {
        //        bool done = true;
        //        while (done)
        //        {

        //            var getTestProgressContext = new GetTestProgressContext()
        //            {
        //                InitiateTestInformation = initiateTestOutput.InitiateTestInformation,
        //                RecentTestProgress = null,
        //                RecentMeasure = null
        //            };

        //            GetTestProgressOutput testProgressOutput = new GetTestProgressOutput();
        //            try
        //            {
        //                DateTime startTime = creationDate;
        //                DateTime endTime = DateTime.Now;
        //                TimeSpan span = endTime.Subtract(startTime);

        //                if (span.TotalMinutes >= vIConnectorSyncTaskActionArgument.TimeOut)
        //                {
        //                    testProgressOutput.Result = GetTestProgressResult.FailedWithNoRetry;
        //                    testProgressOutput.TestProgress = null;
        //                    testProgressOutput.Measure = null;
        //                    testProgressOutput.CallTestResult = CallTestResult.Failed;
        //                }
        //                else
        //                    testProgressOutput = vIConnectorSyncTaskActionArgument.CLITestConnector.GetTestProgress(getTestProgressContext);
        //            }
        //            catch (Exception ex)
        //            {
        //                testProgressOutput.Result = GetTestProgressResult.FailedWithRetry;
        //            }

        //            if (testProgressOutput.Result == GetTestProgressResult.TestCompleted || testProgressOutput.Result == GetTestProgressResult.FailedWithNoRetry)
        //            {
        //                //Insert into the table the results
        //                zoneinfo.ConnectorZoneId = zone.SourceId;
        //                TestProgress testProgress = testProgressOutput.TestProgress as TestProgress;
        //                List<string> codes = new List<string>();

        //                foreach (var callResult in testProgress.CallResults)
        //                  codes.Add(callResult.Destination);
        //                zoneinfo.Codes = codes;
        //                done = false;
        //                //cq.TryDequeue(out zone);
        //            }
        //        }
        //    }
        //    return zoneinfo;
        //}
    }
}

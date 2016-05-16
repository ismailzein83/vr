﻿using System;
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

namespace QM.CLITester.iTestIntegration
{

    public class VIConnectorSyncTaskAction : SchedulerTaskAction
    {
        private static ConcurrentQueue<Zone> cq = new ConcurrentQueue<Zone>();
        private static ConcurrentQueue<ConnectorZoneInfoToUpdate> cq2 = new ConcurrentQueue<ConnectorZoneInfoToUpdate>();
        public override SchedulerTaskExecuteOutput Execute(SchedulerTask task, BaseTaskActionArgument taskActionArgument, Dictionary<string, object> evaluatedExpressions)
        {
            VIConnectorSyncTaskActionArgument vIConnectorSyncTaskActionArgument = taskActionArgument as VIConnectorSyncTaskActionArgument;

            ITestZoneManager iTestZoneManager = new ITestZoneManager();
            Dictionary<string, ITestZone> allZones = new Dictionary<string, ITestZone>();

            ZoneManager zoneManager = new ZoneManager();
            allZones = iTestZoneManager.GetAllZones();
            List<ConnectorZoneInfoToUpdate> listConnectorZoneInfoToUpdates = new List<ConnectorZoneInfoToUpdate>();
            Task<ConnectorZoneInfoToUpdate>[] taskArray = new Task<ConnectorZoneInfoToUpdate>[4];
            ConnectorZoneInfoToUpdate zoneInfo = new ConnectorZoneInfoToUpdate();


            foreach (var zone in allZones.Where(x => (x.Value.ZoneName == "ALFA" || x.Value.ZoneName == "TOUCH")))
            {
                QM.BusinessEntity.Entities.Zone z = new QM.BusinessEntity.Entities.Zone();
                z = zoneManager.GetZonebySourceId(zone.Value.ZoneId);
                int i = 0;
                cq.Enqueue(z);
                taskArray[i] = Task<ConnectorZoneInfoToUpdate>.Factory.StartNew(() => TestCall(vIConnectorSyncTaskActionArgument, z));
                i = i + 1;
            }

            SchedulerTaskExecuteOutput output = new SchedulerTaskExecuteOutput();
            while (!cq.IsEmpty)
            {

            }
            foreach (var task1 in taskArray)
            {
                listConnectorZoneInfoToUpdates.Add(task1.Result);
            }
            ConnectorZoneInfoManager connectorZoneInfoManager = new ConnectorZoneInfoManager();
            connectorZoneInfoManager.UpdateZones("VIConnector", listConnectorZoneInfoToUpdates);

            output = new SchedulerTaskExecuteOutput()
            {
                Result = ExecuteOutputResult.Completed
            };
            return output;
        }

        private static ConnectorZoneInfoToUpdate TestCall(VIConnectorSyncTaskActionArgument vIConnectorSyncTaskActionArgument,
            QM.BusinessEntity.Entities.Zone zone)
        {
            ConnectorZoneInfoToUpdate zoneinfo = new ConnectorZoneInfoToUpdate();

            ProfileManager profileManager = new ProfileManager();
            SupplierManager supplierManager = new SupplierManager();
            CountryManager countryManager = new CountryManager();

            DateTime creationDate = DateTime.Now;
                var initiateTestContext = new InitiateTestContext()
            {
                Supplier = supplierManager.GetSupplier(vIConnectorSyncTaskActionArgument.SupplierId),
                Profile = profileManager.GetProfile(vIConnectorSyncTaskActionArgument.ProfileId),
                Country = countryManager.GetCountry(zone.CountryId),
                Zone = zone
            };

            InitiateTestOutput initiateTestOutput = new InitiateTestOutput();
            try
            {
                initiateTestOutput =
                    vIConnectorSyncTaskActionArgument.CLITestConnector.InitiateTest(initiateTestContext);
            }
            catch (Exception ex)
            {
                initiateTestOutput.Result = InitiateTestResult.FailedWithRetry;
                initiateTestOutput.FailureMessage = ex.Message;
            }

            CallTestStatus callTestStatus;

            switch (initiateTestOutput.Result)
            {
                case InitiateTestResult.Created:
                    callTestStatus = CallTestStatus.Initiated;
                    break;
                case InitiateTestResult.FailedWithRetry:
                    {
                        callTestStatus = CallTestStatus.InitiationFailedWithRetry;
                        break;
                    }

                case InitiateTestResult.FailedWithNoRetry:
                    callTestStatus = CallTestStatus.InitiationFailedWithNoRetry;
                    break;
                default:
                    callTestStatus = CallTestStatus.InitiationFailedWithRetry;
                    break;
            }
            if (callTestStatus == CallTestStatus.Initiated)
            {
                bool done = true;
                while (done)
                {

                    var getTestProgressContext = new GetTestProgressContext()
                    {
                        InitiateTestInformation = initiateTestOutput.InitiateTestInformation,
                        RecentTestProgress = null,
                        RecentMeasure = null
                    };

                    GetTestProgressOutput testProgressOutput = new GetTestProgressOutput();
                    try
                    {
                        DateTime startTime = creationDate;
                        DateTime endTime = DateTime.Now;
                        TimeSpan span = endTime.Subtract(startTime);

                        if (span.TotalMinutes >= vIConnectorSyncTaskActionArgument.TimeOut)
                        {
                            testProgressOutput.Result = GetTestProgressResult.FailedWithNoRetry;
                            testProgressOutput.TestProgress = null;
                            testProgressOutput.Measure = null;
                            testProgressOutput.CallTestResult = CallTestResult.Failed;
                        }
                        else
                            testProgressOutput = vIConnectorSyncTaskActionArgument.CLITestConnector.GetTestProgress(getTestProgressContext);
                    }
                    catch (Exception ex)
                    {
                        testProgressOutput.Result = GetTestProgressResult.FailedWithRetry;
                    }

                    if (testProgressOutput.Result == GetTestProgressResult.TestCompleted || testProgressOutput.Result == GetTestProgressResult.FailedWithNoRetry)
                    {
                        //Insert into the table the results
                        zoneinfo.ConnectorZoneId = zone.SourceId;
                        TestProgress testProgress = testProgressOutput.TestProgress as TestProgress;
                        List<string> codes = new List<string>();

                        foreach (var callResult in testProgress.CallResults)
                          codes.Add(callResult.Destination);
                        zoneinfo.Codes = codes;
                        done = false;
                        cq.TryDequeue(out zone);
                    }
                }
            }
            return zoneinfo;
        }
    }
}

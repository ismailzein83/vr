using Retail.Data.Business;
using Retail.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;
using Vanrise.GenericData.QueueActivators;
using Vanrise.Queueing.Entities;
using Vanrise.Reprocess.Entities;

namespace Retail.Data.MainExtensions.QueueActivators
{
    public class PrepareDataActivator : QueueActivator
    {
        public List<string> OutputStages { get; set; }

        #region QueueActivator

        public override void OnDisposed()
        {
        }

        public override void ProcessItem(PersistentQueueItem item, ItemsToEnqueue outputItems)
        {
        }

        public override void ProcessItem(IQueueActivatorExecutionContext context)
        {
            DataRecordBatch dataRecordBatch = context.ItemToProcess as DataRecordBatch;
            var queueItemType = context.CurrentStage.QueueItemType as DataRecordBatchQueueItemType;
            if (queueItemType == null)
                throw new Exception("current stage QueueItemType is not of type DataRecordBatchQueueItemType");
            var recordTypeId = queueItemType.DataRecordTypeId;
            var batchRecords = dataRecordBatch.GetBatchRecords(recordTypeId);

            Dictionary<string, UserSessionData> userSessionDataByUserSession = new Dictionary<string, UserSessionData>();
            Dictionary<string, List<dynamic>> recordsByUserSession = new Dictionary<string, List<dynamic>>();
            foreach (var record in batchRecords)
            {
                DateTime recordDateTime = record.GetFieldValue("RecordDateTime").Date;
                string userName = record.GetFieldValue("UserName");
                string sessionId = record.GetFieldValue("SessionId");
                string userSession = string.Concat(userName, "_", sessionId);

                UserSessionData userSessionData;
                if (userSessionDataByUserSession.TryGetValue(userSession, out userSessionData))
                {
                    if (recordDateTime < userSessionData.StartDate)
                        userSessionData.StartDate = recordDateTime;
                }
                else
                {
                    userSessionData = new UserSessionData() { UserSession = userSession, StartDate = recordDateTime };
                    userSessionDataByUserSession.Add(userSession, userSessionData);
                }

                List<dynamic> userSessionDataList = recordsByUserSession.GetOrCreateItem(userSession, () => { return new List<dynamic>(); });
                userSessionDataList.Add(record);
            }

            List<UserSessionData> updatedUserSessionDataList = new UserSessionManager().UpdateAndGetUserSessionData(userSessionDataByUserSession.Values.ToList());
            Dictionary<string, UserSessionData> updatedUserSessionDataDict = updatedUserSessionDataList.ToDictionary(itm => itm.UserSession, itm => itm);

            List<dynamic> userSessionsData = new List<dynamic>();
            foreach (var kvp in recordsByUserSession)
            {
                string userSession = kvp.Key;
                IEnumerable<dynamic> userSessionRecords = kvp.Value;

                UserSessionData userSessionData;
                if (!updatedUserSessionDataDict.TryGetValue(userSession, out userSessionData))
                    throw new NullReferenceException(string.Format("updatedUserSessionDataDict for userSession: {0}", userSession));

                foreach (var record in userSessionRecords)
                {
                    record.SetFieldValue("UserSession", userSession);
                    record.SetFieldValue("UserSessionStartDate", userSessionData.StartDate);
                }
            }

            DataRecordBatch transformedBatch = DataRecordBatch.CreateBatchFromRecords(batchRecords, queueItemType.BatchDescription, recordTypeId);

            foreach (var outputStage in this.OutputStages)
                context.OutputItems.Add(outputStage, transformedBatch);
        }

        #endregion
    }
}

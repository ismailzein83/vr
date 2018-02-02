using System.Collections.Generic;
using Vanrise.BusinessProcess.Data;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using Vanrise.Entities;

namespace Vanrise.BusinessProcess.Business
{
    public class BPInstanceTrackingManager
    {
        #region public methods

        public Vanrise.Entities.IDataRetrievalResult<BPTrackingMessageDetail> GetFilteredBPInstanceTracking(Vanrise.Entities.DataRetrievalInput<BPTrackingQuery> input)
        {
            IBPTrackingDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPTrackingDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetFilteredBPInstanceTracking(input));
        }

        public IEnumerable<BPTrackingMessage> GetBPInstanceTrackingMessages(long processInstanceId, List<LogEntryType> Severities)
        {
            IBPTrackingDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPTrackingDataManager>();
            return dataManager.GetBPInstanceTrackingMessages(processInstanceId, Severities);
        }

        public List<BPTrackingMessageDetail> GetBeforeId(BPTrackingBeforeIdInput input)
        {
            IBPTrackingDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPTrackingDataManager>();

            List<BPTrackingMessage> bpTrackingMessages = dataManager.GetBeforeId(input);
            List<BPTrackingMessageDetail> bpTrackingMessageDetails = new List<BPTrackingMessageDetail>();
            foreach (BPTrackingMessage bpTrackingMessage in bpTrackingMessages)
            {
                bpTrackingMessageDetails.Add(BPTrackingMessageDetailMapper(bpTrackingMessage));
            }
            return bpTrackingMessageDetails;
        }

        public BPTrackingUpdateOutput GetUpdated(BPTrackingUpdateInput input)
        {
            BPTrackingUpdateOutput bpTrackingUpdateOutput = new BPTrackingUpdateOutput();

            IBPTrackingDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPTrackingDataManager>();

            List<BPTrackingMessage> bpTrackingMessages = dataManager.GetUpdated(input);
            List<BPTrackingMessageDetail> bpTrackingMessageDetails = new List<BPTrackingMessageDetail>();
            foreach (BPTrackingMessage bpTrackingMessage in bpTrackingMessages)
            {
                bpTrackingMessageDetails.Add(BPTrackingMessageDetailMapper(bpTrackingMessage));
            }

            bpTrackingUpdateOutput.ListBPInstanceTrackingDetails = bpTrackingMessageDetails;
            return bpTrackingUpdateOutput;
        }

        public List<BPTrackingMessage> GetRecentBPInstanceTrackings(long processInstanceId, int nbOfRecords, long? lessThanId, List<LogEntryType> severities)
        {
            IBPTrackingDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPTrackingDataManager>();
            return dataManager.GetRecentBPInstanceTrackings(processInstanceId, nbOfRecords, lessThanId, severities);
        }

        #endregion

        #region mapper
        private BPTrackingMessageDetail BPTrackingMessageDetailMapper(BPTrackingMessage bpTrackingMessage)
        {
            if (bpTrackingMessage == null)
                return null;
            return new BPTrackingMessageDetail() 
            {
                Entity = bpTrackingMessage
            };
        }
        #endregion
    }
}

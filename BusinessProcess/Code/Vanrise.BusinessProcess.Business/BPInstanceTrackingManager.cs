﻿using System.Collections.Generic;
using Vanrise.BusinessProcess.Data;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Entities;

namespace Vanrise.BusinessProcess.Business
{
    public class BPInstanceTrackingManager
    {
        #region public methods

        public IEnumerable<BPTrackingMessage> GetBPInstanceTrackingMessages(long processInstanceId, List<LogEntryType> severities)
        {
            IBPTrackingDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPTrackingDataManager>();
            return dataManager.GetBPInstanceTrackingMessages(processInstanceId, severities);
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
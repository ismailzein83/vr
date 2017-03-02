﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Data;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess
{
    public class BPValidationMessageManager
    {
        public void Insert(IEnumerable<BPValidationMessage> messages)
        {
            IBPValidationMessageDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPValidationMessageDataManager>();
            dataManager.Insert(messages);
        }

        public List<BPValidationMessageDetail> GetBeforeId(BPValidationMessageBeforeIdInput input)
        {
            IBPValidationMessageDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPValidationMessageDataManager>();

            List<BPValidationMessage> bpValidationMessages = dataManager.GetBeforeId(input);
            List<BPValidationMessageDetail> bpValidationMessageDetails = new List<BPValidationMessageDetail>();
            foreach (BPValidationMessage bpValidationMessage in bpValidationMessages)
            {
                bpValidationMessageDetails.Add(BPValidationMessageDetailMapper(bpValidationMessage));
            }
            return bpValidationMessageDetails;
        }

        public BPValitaionMessageUpdateOutput GetUpdated(BPValidationMessageUpdateInput input)
        {
            BPValitaionMessageUpdateOutput bpValitaionMessageUpdateOutput = new BPValitaionMessageUpdateOutput();

            IBPValidationMessageDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPValidationMessageDataManager>();

            List<BPValidationMessage> bpValidationMessages = dataManager.GetUpdated(input);
            List<BPValidationMessageDetail> bpValidationMessageDetails = new List<BPValidationMessageDetail>();
            foreach (BPValidationMessage bpValidationMessage in bpValidationMessages)
            {
                bpValidationMessageDetails.Add(BPValidationMessageDetailMapper(bpValidationMessage));
            }

            bpValitaionMessageUpdateOutput.ListValidationMessageDetails = bpValidationMessageDetails;
            return bpValitaionMessageUpdateOutput;
        }

        public Vanrise.Entities.IDataRetrievalResult<BPValidationMessageDetail> GetFilteredBPValidationMessage(Vanrise.Entities.DataRetrievalInput<BPValidationMessageQuery> input)
        {
            IBPValidationMessageDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPValidationMessageDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetFilteredBPValidationMessage(input));
        }

        private BPValidationMessageDetail BPValidationMessageDetailMapper(BPValidationMessage bpValidationMessage)
        {
            if (bpValidationMessage == null)
                return null;
            return new BPValidationMessageDetail()
            {
                Entity = bpValidationMessage
            };
        }


    }
}

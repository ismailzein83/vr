using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.BusinessProcess.Data;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess.Business
{
    public class BPVisualEventManager
    {
        IBPVisualEventDataManager s_dataManager = BPDataManagerFactory.GetDataManager<IBPVisualEventDataManager>();

        #region public Methods

        public void InsertVisualEvent(long processInstanceId, Guid activityId, string title, Guid eventTypeId, BPVisualEventPayload eventPayload)
        {
            s_dataManager.InsertVisualEvent(processInstanceId, activityId, title, eventTypeId, Serializer.Serialize(eventPayload));
        }

        public BPVisuaIEventDetailUpdateOutput GetAfterId(BPVisualEventDetailUpdateInput input)
        {
            BPVisuaIEventDetailUpdateOutput bpVisualItemUpdateOutput = new BPVisuaIEventDetailUpdateOutput();

            List<BPVisualEvent> bpVisualEvents = s_dataManager.GetAfterId(input);
            List<BPVisualEventDetail> bpVisualItemDetails = new List<BPVisualEventDetail>();
            foreach (BPVisualEvent bpVisualEvent in bpVisualEvents)
            {
                bpVisualItemDetails.Add(BPVisualEventDetailMapper(bpVisualEvent));
            }

            bpVisualItemUpdateOutput.ListBPVisualEventDetails = bpVisualItemDetails;
            return bpVisualItemUpdateOutput;
        }
        #endregion

        #region Mappers
        private BPVisualEventDetail BPVisualEventDetailMapper(BPVisualEvent bpVisualEvent)
        {
            return new BPVisualEventDetail()
            {
                ActivityId = bpVisualEvent.ActivityId,
                BPVisualEventId= bpVisualEvent.BPVisualEventId,
                CreatedTime= bpVisualEvent.CreatedTime,
                EventPayload= bpVisualEvent.EventPayload,
                EventTypeId= bpVisualEvent.EventTypeId,
                ProcessInstanceId= bpVisualEvent.ProcessInstanceId,
                Title= bpVisualEvent.Title
            };
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data;
using Vanrise.Entities;

namespace Vanrise.Common.Data
{
    public interface IVRReceivedMailMessageDataManager : IDataManager, IBulkApplyDataManager<VRReceivedMailMessage>
    {
        DateTime GetLastMessageSendTime(Guid connectionId, string senderIdentifier);
        void Insert(Guid connectionId, string senderIdentifier, List<VRReceivedMailMessage> messages);
        List<string> GetReceivedMailMessagesIdsFromSpecificTime(Guid connectionId, string senderIdentifier, DateTime fromDate);
    }
}

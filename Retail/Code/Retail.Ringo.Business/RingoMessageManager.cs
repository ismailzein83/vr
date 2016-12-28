using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retail.Ringo.Data;
using Retail.Ringo.Entities;

namespace Retail.Ringo.Business
{
    public class RingoMessageManager
    {
        public long GetRingoMessageTotal(DateTime from, DateTime to)
        {
            IRingoMessageDataManager dataManager = RingoDataManagerFactory.GetDataManager<IRingoMessageDataManager>();
            return dataManager.GetTotal(new RingoMessageFilter
             {
                 From = from,
                 To = to,
                 Sender = "ICSI",
                 MessageTypes = new List<int> { 1 }
             });
        }

        public Dictionary<string, RingoMessageCountEntity> GetSenderRingoMessageRecords(RingoMessageFilter filter)
        {
            IRingoMessageDataManager dataManager = RingoDataManagerFactory.GetDataManager<IRingoMessageDataManager>();
            return dataManager.GetRingoMessageCountEntityBySender(filter).ToDictionary(k => k.Name, v => v);
        }

        public Dictionary<string, RingoMessageCountEntity> GetRecipientRingoMessageRecords(RingoMessageFilter filter)
        {
            IRingoMessageDataManager dataManager = RingoDataManagerFactory.GetDataManager<IRingoMessageDataManager>();
            return dataManager.GetRingoMessageCountEntityByRecipient(filter).ToDictionary(k => k.Name, v => v);
        }

    }
}

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
        public long GetRingoMessageTotal(RingoMessageFilter filter)
        {
            IRingoMessageDataManager dataManager = RingoDataManagerFactory.GetDataManager<IRingoMessageDataManager>();
            return dataManager.GetTotal(filter);
        }

        public Dictionary<string, RingoMessageCountEntity> GetSenderRingoMessageRecords(RingoMessageFilter filter)
        {
            IRingoMessageDataManager dataManager = RingoDataManagerFactory.GetDataManager<IRingoMessageDataManager>();
            return dataManager.GetRingoMessageCountEntityBySender(filter).ToDictionary(k => k.Name, v => v);
        }

        public Dictionary<string, RingoMessageCountEntity> GetRecipientRingoMessageRecords_CTE(RingoMessageFilter filter)
        {
            IRingoMessageDataManager dataManager = RingoDataManagerFactory.GetDataManager<IRingoMessageDataManager>();
            return dataManager.GetRingoMessageCountEntityByRecipient_CTE(filter).ToDictionary(k => k.Name, v => v);
        }

        public Dictionary<string, RingoMessageCountEntity> GetRecipientRingoMessageRecords(RingoMessageFilter filter)
        {
            IRingoMessageDataManager dataManager = RingoDataManagerFactory.GetDataManager<IRingoMessageDataManager>();
            return dataManager.GetRingoMessageCountEntityByRecipient(filter).ToDictionary(k => k.Name, v => v);
        }
        public Dictionary<string, RingoMessageCountEntity> GetRingoMessageCountEntityBySender_LastDay(RingoMessageFilter filter)
        {
            IRingoMessageDataManager dataManager = RingoDataManagerFactory.GetDataManager<IRingoMessageDataManager>();
            return dataManager.GetRingoMessageCountEntityBySender_LastDay(filter).ToDictionary(k => k.Name, v => v);
        }
    }
}

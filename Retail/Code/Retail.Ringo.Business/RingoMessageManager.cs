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

        public Dictionary<string, RingoMessageCountEntity> GetSenderRingoMessageRecords_CTE(RingoMessageFilter filter)
        {
            IRingoMessageDataManager dataManager = RingoDataManagerFactory.GetDataManager<IRingoMessageDataManager>();
            return dataManager.GetSenderRingoMessageRecords_CTE(filter).ToDictionary(k => k.Name, v => v);
        }

        public Dictionary<string, RingoMessageCountEntity> GetSenderRingoMessageRecords_EightSheet(RingoMessageFilter firstFilter, RingoMessageFilter secondFilter)
        {
            IRingoMessageDataManager dataManager = RingoDataManagerFactory.GetDataManager<IRingoMessageDataManager>();
            return dataManager.GetSenderRingoMessageRecords_EightSheet(firstFilter, secondFilter).ToDictionary(k => k.Name, v => v);
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
        public IEnumerable<SintesiRingoMessageEntity> GetSintesiRingoMessageEntityByRecipient(TCRRingoReportFilter filter)
        {
            IRingoMessageDataManager dataManager = RingoDataManagerFactory.GetDataManager<IRingoMessageDataManager>();
            return dataManager.GetSintesiRingoMessageEntityByRecipient(filter);
        }
        public IEnumerable<SintesiRingoMessageEntity> GetSintesiRingoMessageEntityBySender(TCRRingoReportFilter filter)
        {
            IRingoMessageDataManager dataManager = RingoDataManagerFactory.GetDataManager<IRingoMessageDataManager>();
            return dataManager.GetSintesiRingoMessageEntityBySender(filter);
        }
        public IEnumerable<DettaglioRingoMessageEntity> GetDettaglioRingoMessageEntityByRecipient(TCRRingoReportFilter filter)
        {
            IRingoMessageDataManager dataManager = RingoDataManagerFactory.GetDataManager<IRingoMessageDataManager>();
            return dataManager.GetDettaglioRingoMessageEntityByRecipient(filter);
        }
        public IEnumerable<DettaglioRingoMessageEntity> GetDettaglioRingoMessageEntityBySender(TCRRingoReportFilter filter)
        {
            IRingoMessageDataManager dataManager = RingoDataManagerFactory.GetDataManager<IRingoMessageDataManager>();
            return dataManager.GetDettaglioRingoMessageEntityBySender(filter);
        }
    }
}

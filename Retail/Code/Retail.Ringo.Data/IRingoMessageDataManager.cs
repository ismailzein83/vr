﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retail.Ringo.Entities;

namespace Retail.Ringo.Data
{
    public interface IRingoMessageDataManager : IDataManager
    {
        long GetTotal(RingoMessageFilter filter);
        IEnumerable<RingoMessageCountEntity> GetRingoMessageCountEntityByRecipient(RingoMessageFilter filter);
        IEnumerable<RingoMessageCountEntity> GetRingoMessageCountEntityBySender(RingoMessageFilter filter);
        IEnumerable<RingoMessageCountEntity> GetRingoMessageCountEntityByRecipient_CTE(RingoMessageFilter filter);
        IEnumerable<RingoMessageCountEntity> GetRingoMessageCountEntityBySender_LastDay(RingoMessageFilter filter);
        IEnumerable<RingoMessageCountEntity> GetSenderRingoMessageRecords_CTE(RingoMessageFilter filter);
        IEnumerable<SintesiRingoMessageEntity> GetSintesiRingoMessageEntityByRecipient(TCRRingoReportFilter filter);
        IEnumerable<SintesiRingoMessageEntity> GetSintesiRingoMessageEntityBySender(TCRRingoReportFilter filter);
        IEnumerable<DettaglioRingoMessageEntity> GetDettaglioRingoMessageEntityByRecipient(TCRRingoReportFilter filter);
        IEnumerable<DettaglioRingoMessageEntity> GetDettaglioRingoMessageEntityBySender(TCRRingoReportFilter filter);
        IEnumerable<RingoMessageCountEntity> GetSenderRingoMessageRecords_EightSheet(RingoMessageFilter firstFilter, RingoMessageFilter secondFilter);
    }
}

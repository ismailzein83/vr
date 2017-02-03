using System;
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
    }
}

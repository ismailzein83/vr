using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SMSBusinessEntity.Entities;

namespace TOne.WhS.SMSBusinessEntity.Data
{
    public interface IProcessDraftDataManager : IDataManager
    {
        ProcessDraft GetChangesByProcessDraftID(long processDraftID);
        bool InsertOrUpdateChanges(ProcessEntityType processType, string changes, string entityID, int userID, out int? processDraftID);
        bool UpdateProcessStatus(long processDraftID, ProcessStatus newStatus, int userID);
        DraftStateResult CheckIfDraftExist(ProcessEntityType processType, string entityID);
    }
}

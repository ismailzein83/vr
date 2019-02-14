using TOne.WhS.SMSBusinessEntity.Entities;

namespace TOne.WhS.SMSBusinessEntity.Data
{
    public interface IProcessDraftDataManager : IDataManager
    {
        ProcessDraft GetChangesByProcessDraftID(long processDraftID);
        ProcessDraft GetChangesByEntityID(ProcessEntityType processType, string entityID);
        bool InsertOrUpdateChanges(ProcessEntityType processType, string changes, string entityID, int userID, out long? processDraftID);
        bool UpdateProcessStatus(long processDraftID, ProcessStatus newStatus, int userID);
    }
}

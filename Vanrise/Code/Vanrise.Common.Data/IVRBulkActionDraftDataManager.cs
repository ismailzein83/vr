using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Data
{
    public interface IVRBulkActionDraftDataManager:IDataManager
    {
        void ClearVRBulkActionDraft(Guid bulkActionDraftIdentifier, DateTime removeBeforeDate);
        void CreateVRBulkActionDrafts(IEnumerable<VRBulkActionDraft> vrBulkActionDrafts,Guid bulkActionDraftIdentifier);
        IEnumerable<VRBulkActionDraft> GetVRBulkActionDrafts(Guid bulkActionDraftIdentifier);
    }
}

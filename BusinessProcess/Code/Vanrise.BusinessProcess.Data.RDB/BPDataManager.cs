using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess.Data.RDB
{
    public class BPDataManager : IBPDataManager
    {
        public T GetDefinitionObjectState<T>(Guid definitionId, string objectKey)
        {
            throw new NotImplementedException();
        }

        public int InsertDefinitionObjectState(Guid definitionId, string objectKey, object objectValue)
        {
            throw new NotImplementedException();
        }

        public int UpdateDefinitionObjectState(Guid definitionId, string objectKey, object objectValue)
        {
            throw new NotImplementedException();
        }
    }
}

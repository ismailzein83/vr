using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess.Data.RDB
{
    public class BPInstancePersistenceDataManager : IBPInstancePersistenceDataManager
    {
        public void DeleteState(long processInstanceId)
        {
            throw new NotImplementedException();
        }

        public string GetInstanceState(long processInstanceId)
        {
            throw new NotImplementedException();
        }

        public void InsertOrUpdateInstance(long processInstanceId, string instanceState)
        {
            throw new NotImplementedException();
        }
    }
}

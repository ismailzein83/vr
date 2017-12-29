using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess.Data
{
    public interface IBPInstancePersistenceDataManager : IDataManager
    {
        void InsertOrUpdateInstance(long processInstanceId, string instanceState);
        string GetInstanceState(long processInstanceId);
        void DeleteState(long processInstanceId);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess.Data
{
    public interface IBPTaskTypeDataManager : IDataManager
    {
        IEnumerable<BPTaskType> GetBPTaskTypes();

        bool AreBPTaskTypesUpdated(ref object updateHandle);
    }
}

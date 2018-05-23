using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Queueing.Data
{
    public interface IExecutionControlDataManager : IDataManager
    {
        bool IsExecutionPaused();
        bool UpdateExecutionPaused(bool isPaused);
    }
}

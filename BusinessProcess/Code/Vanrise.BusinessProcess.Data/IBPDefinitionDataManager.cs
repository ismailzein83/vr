using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess.Data
{
    public interface IBPDefinitionDataManager : IDataManager
    {
        List<BPDefinition> GetBPDefinitions();

        bool AreBPDefinitionsUpdated(ref object updateHandle);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BEBridge.Entities;

namespace Vanrise.BEBridge.Data
{
    public interface IBEReceiveDefinitionDataManager : IDataManager
    {
        IEnumerable<BEReceiveDefinition> GetBEReceiveDefinitions();

        bool AreBEReceiveDefinitionsUpdated(ref object updateHandle);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess.Entities
{
    public interface IBPDefinitionManager : IBusinessManager
    {
        bool DoesUserHaveViewAccessInManagement(int userId);

        string GetDefinitionTitle(string processName);

        bool DoesUserHaveViewAccess(string bPDefinitionName);

        BPDefinition GetBPDefinition(Guid definitionId);
    }
}

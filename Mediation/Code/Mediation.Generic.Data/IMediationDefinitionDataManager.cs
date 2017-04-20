using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mediation.Generic.Entities;

namespace Mediation.Generic.Data
{
    public interface IMediationDefinitionDataManager : IDataManager
    {

        List<MediationDefinition> GetMediationDefinitions();

        bool AreMediationDefinitionsUpdated(ref object updateHandle);

        bool UpdateMediationDefinition(MediationDefinition mediationDefinition);

        bool InsertMediationDefinition(MediationDefinition mediationDefinition);
    }
}

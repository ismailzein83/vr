using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Data
{
    public interface IVRObjectTypeDefinitionDataManager : IDataManager
    {
        List<VRObjectTypeDefinition> GetVRObjectTypeDefinitions();

        bool AreVRObjectTypeDefinitionUpdated(ref object updateHandle);

        bool Insert(VRObjectTypeDefinition vrObjectTypeDefinitionItem);

        bool Update(VRObjectTypeDefinition vrObjectTypeDefinitionItem);
    }
}

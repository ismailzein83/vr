using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Data
{
    public interface IGenericEditorDataManager:IDataManager
    {
        bool UpdateGenericEditorDefinition(GenericEditorDefinition genericEditorDefinition);

        bool AddGenericEditorDefinition(GenericEditorDefinition genericEditorDefinition, out int genericEditorDefinitionId);
        List<GenericEditorDefinition> GetGenericEditorDefinitions();
        bool AreGenericEditorDefinitionUpdated(ref object updateHandle);
    }
}

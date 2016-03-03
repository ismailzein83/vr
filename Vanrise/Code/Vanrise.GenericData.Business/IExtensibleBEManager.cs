using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Business
{
    public interface IExtensibleBEManager
    {
        int GetBusinessEntityDefinitionId();
        ExtensibleBEItemRuntime GetExtensibleBEItemRuntime(int dataRecordTypeId, int businessEntityId);
        IEnumerable<DataRecordTypeInfo> GetDataRecordTypesInfo(int businessEntityId);
    }
}

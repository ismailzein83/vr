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
        Guid GetBusinessEntityDefinitionId();
        ExtensibleBEItemRuntime GetExtensibleBEItemRuntime(Guid dataRecordTypeId, Guid businessEntityId);
        IEnumerable<DataRecordTypeInfo> GetDataRecordTypesInfo(Guid businessEntityId);
    }
}

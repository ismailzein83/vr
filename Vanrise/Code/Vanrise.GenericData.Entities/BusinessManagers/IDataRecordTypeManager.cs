using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public interface IDataRecordTypeManager : IBusinessManager
    {
        Type GetDataRecordRuntimeType(Guid dataRecordTypeId);

        dynamic ConvertDynamicToDataRecord(dynamic dynamicObject, Guid dataRecordTypeId);

        string SerializeRecord(dynamic record, Guid dataRecordTypeId);

        dynamic DeserializeRecord(string serializedRecord, Guid dataRecordTypeId);
    }
}

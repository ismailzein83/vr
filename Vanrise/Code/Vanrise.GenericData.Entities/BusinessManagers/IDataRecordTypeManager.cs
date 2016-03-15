using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public interface IDataRecordTypeManager : IBusinessManager
    {
        Type GetDataRecordRuntimeType(int dataRecordTypeId);

        dynamic ConvertDynamicToDataRecord(dynamic dynamicObject, int dataRecordTypeId);

        string SerializeRecord(dynamic record, int dataRecordTypeId);

        dynamic DeserializeRecord(string serializedRecord, int dataRecordTypeId);
    }
}

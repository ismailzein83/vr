using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Data
{
    public interface IDataRecordTypeDataManager:IDataManager
    {
        List<DataRecordType> GetDataRecordTypes();
        bool AreDataRecordTypeUpdated(ref object updateHandle);
        bool UpdateDataRecordType(DataRecordType dataRecordType);

        bool AddDataRecordType(DataRecordType dataRecordType);
    }
}

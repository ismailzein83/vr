using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities.GenericDataRecord;

namespace Vanrise.Common.Data
{
    public interface IDataRecordTypeDataManager:IDataManager
    {
        List<DataRecordType> GetALllDataRecordTypes();
        bool AreDataRecordTypeUpdated(ref object updateHandle);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities.GenericDataRecord;

namespace Vanrise.Common.Data
{
    public interface IDataRecordFieldDataManager:IDataManager
    {
        List<DataRecordField> GetALllDataRecordFields();
        bool AreDataRecordFieldUpdated(ref object updateHandle);
        bool Update(DataRecordField dataRecordField);
        bool Delete(int dataRecordFieldId);
        bool Insert(DataRecordField dataRecordField, out int insertedId);
    }
}

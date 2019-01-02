using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.FileDataStorage
{
    public interface IDynamicManager
    {
        void FillRecordInfoFromDynamicRecord(Vanrise.GenericData.FileDataStorage.FileRecordStorageRecordInfo recordInfo, dynamic dynamicRecord);

        dynamic GetDynamicRecordFromRecordInfo(Vanrise.GenericData.FileDataStorage.FileRecordStorageRecordInfo recordInfo);
    }
}

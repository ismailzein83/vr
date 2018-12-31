using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.FileDataStorage
{
    public interface IDynamicManager
    {
        void AddRecordToCorrespondingBatch(dynamic dynamicRecord, Vanrise.GenericData.FileDataStorage.FileRecordStorageBatches batches);

        void FillRecordInfoFromDynamicRecord(Vanrise.GenericData.FileDataStorage.FileRecordStorageRecordInfo recordInfo, dynamic dynamicRecord);
    }
}

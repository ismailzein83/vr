using System;
using System.Linq;
using Vanrise.Common;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Business
{
    public static class Helper
    {
        public static RecordFilter ConvertToRecordFilter<T>(string dataRecordFieldName, DataRecordFieldType dataRecordFieldType, ListRecordFilter<T> listRecordFilter)
        {
            var dataRecordFieldTypeConvertToRecordFilterContext = new DataRecordFieldTypeConvertToRecordFilterContext
            {
                FieldName = dataRecordFieldName,
                FilterValues = listRecordFilter.Values.VRCast<object>().ToList(),
                StrictEqual = true
            };
            var convertedRecordFilter = dataRecordFieldType.ConvertToRecordFilter(dataRecordFieldTypeConvertToRecordFilterContext);
            if (convertedRecordFilter is ListRecordFilter<T>)
                ((ListRecordFilter<T>)convertedRecordFilter).CompareOperator = listRecordFilter.CompareOperator;
            return convertedRecordFilter;
        }

        public static string GetStorageName(Guid dataRecordStorageId)
        {
            DataRecordStorageManager drsManager = new DataRecordStorageManager();
            var dataRecordStorage = drsManager.GetDataRecordStorage(dataRecordStorageId);
            dataRecordStorage.ThrowIfNull("dataRecordStorage", dataRecordStorageId);

            DataStoreManager dsManager = new DataStoreManager();
            var dataStore = dsManager.GetDataStore(dataRecordStorage.DataStoreId);
            dataStore.ThrowIfNull("dataStore", dataRecordStorage.DataStoreId);

            return dataStore.Settings.GetStorageName(new GetStorageNameContext() { DataRecordStorage = dataRecordStorage });
        }
    }
}
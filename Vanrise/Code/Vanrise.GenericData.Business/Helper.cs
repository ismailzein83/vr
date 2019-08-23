using System;
using System.Collections.Generic;
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

        #region To Be Deleted
        public static Dictionary<string, GetDescriptionByIdsDicValueOutput> ArrangeRecordsByFieldName(List<DataRecordField> dataRecordFields)
        {
            if (dataRecordFields == null || dataRecordFields.Count == 0)
                return null;

            Dictionary<string, GetDescriptionByIdsDicValueOutput> fieldValuesByFieldName = new Dictionary<string, GetDescriptionByIdsDicValueOutput>();
            foreach (var dataRecordField in dataRecordFields)
            {
                if (dataRecordField.Type.CanGetDescriptionByIds(new DataRecordFieldTypeCanGetDescriptionByIdsContext()))
                    fieldValuesByFieldName.Add(dataRecordField.Name, new GetDescriptionByIdsDicValueOutput { FieldType = dataRecordField.Type, Values = new List<object>(), Descriptions = new Dictionary<object, List<ISetRecordDescription>>() });
            }

            return fieldValuesByFieldName;
        }

        public static void FillDescriptionsForGroupedValues(Dictionary<string, GetDescriptionByIdsDicValueOutput> arrangedFieldsByName, HashSet<string> fieldsToGetDescription = null)
        {
            if (arrangedFieldsByName == null || arrangedFieldsByName.Count == 0)
                return;

            Dictionary<string, GetDescriptionByIdsDicValueOutput> desciptionsByNameForGroupedValues = null;// GetDescriptionsForGroupedValues(arrangedFieldsByName, fieldsToGetDescription);

            if (desciptionsByNameForGroupedValues == null || desciptionsByNameForGroupedValues.Count == 0)
                return;

            foreach (var descriptionFieldName in desciptionsByNameForGroupedValues.Keys)
            {
                GetDescriptionByIdsDicValueOutput getDescriptionByIdsDicValueOutput = null;
                //if (arrangedFieldsByName.TryGetValue(descriptionFieldName, out getDescriptionByIdsDicValueOutput))
                //{
                //    getDescriptionByIdsDicValueOutput.SetDescription(desciptionsByNameForGroupedValues.GetRecord(descriptionFieldName));
                //}
            }
        }

        public static void CollectFieldValues(Dictionary<string, GetDescriptionByIdsDicValueOutput> arrangedFieldNames, string fieldName, Object value, ISetRecordDescription recordGroupedValue)
        {
            if (arrangedFieldNames == null || arrangedFieldNames.Count == 0)
                return;

            GetDescriptionByIdsDicValueOutput getDescriptionByIdsDicValueOutput = null;
            if (value != null && arrangedFieldNames.TryGetValue(fieldName, out getDescriptionByIdsDicValueOutput))
            {
                if (value.GetType() == typeof(Guid))
                    value = value.ToString();

                getDescriptionByIdsDicValueOutput.Values.Add(value);
                var groupedRecords = getDescriptionByIdsDicValueOutput.Descriptions.GetOrCreateItem(value);
                groupedRecords.Add(recordGroupedValue);
            }
        }
        #endregion

        public static Dictionary<string, DataRecordFieldType> GetFieldTypesByFieldNameForDescriptionByIds(List<DataRecordField> dataRecordFields)
        {
            if (dataRecordFields == null || dataRecordFields.Count == 0)
                return null;

            Dictionary<string, DataRecordFieldType> fieldTypesByFieldName = new Dictionary<string, DataRecordFieldType>();
            foreach (var dataRecordField in dataRecordFields)
            {
                if (dataRecordField.Type.CanGetDescriptionByIds(new DataRecordFieldTypeCanGetDescriptionByIdsContext()))
                    fieldTypesByFieldName.Add(dataRecordField.Name, dataRecordField.Type);
            }

            return fieldTypesByFieldName.Count > 0 ? fieldTypesByFieldName : null;
        }

        public static void GroupFieldValuesForGetDescriptionByIds(Dictionary<string, DataRecordFieldType> fieldTypesByFieldName, string fieldName, Object value,
            ISetRecordDescription recordGroupedValue, Dictionary<string, GroupedValuesForGetDescriptionByIds> groupedValuesByFieldName)
        {
            if (fieldTypesByFieldName == null || fieldTypesByFieldName.Count == 0 || groupedValuesByFieldName == null)
                return;

            if (value != null && fieldTypesByFieldName.TryGetValue(fieldName, out DataRecordFieldType fieldType))
            {
                if (value.GetType() == typeof(Guid))
                    value = value.ToString();

                var groupedValuesForGetDescriptionByIds = groupedValuesByFieldName.GetOrCreateItem(fieldName, () => { return new GroupedValuesForGetDescriptionByIds() { FieldType = fieldType, RecordsByValue = new Dictionary<object, List<ISetRecordDescription>>() }; });
                var recordGroupedValues = groupedValuesForGetDescriptionByIds.RecordsByValue.GetOrCreateItem(value);
                recordGroupedValues.Add(recordGroupedValue);
            }
        }

        public static void FillDescriptionsForGroupedValues(Dictionary<string, GroupedValuesForGetDescriptionByIds> groupedValuesByFieldName)
        {
            FillDescriptionsForGroupedValues(groupedValuesByFieldName, null);
        }

        /// <summary>
        /// If specificFieldsToGetDescription is null, this function will set description for all fields in groupedValuesByFieldName
        /// </summary>
        /// <param name="groupedValuesByFieldName"></param>
        /// <param name="specificFieldsToGetDescription"></param>
        public static void FillDescriptionsForGroupedValues(Dictionary<string, GroupedValuesForGetDescriptionByIds> groupedValuesByFieldName, HashSet<string> specificFieldsToGetDescription)
        {
            if (groupedValuesByFieldName == null || groupedValuesByFieldName.Count == 0)
                return;

            foreach (var kvp in groupedValuesByFieldName)
            {
                var fieldName = kvp.Key;
                var groupedValuesForGetDescriptionByIds = kvp.Value;

                groupedValuesForGetDescriptionByIds.ThrowIfNull("groupedValuesForGetDescriptionByIds");
                groupedValuesForGetDescriptionByIds.RecordsByValue.ThrowIfNull("groupedValuesForGetDescriptionByIds.RecordsByValue");

                if (specificFieldsToGetDescription != null && !specificFieldsToGetDescription.Contains(fieldName))
                    continue;

                var descriptions = groupedValuesForGetDescriptionByIds.FieldType.GetDescriptionByIds(new DataRecordFieldTypeGetDescriptionByIdsContext() { Values = groupedValuesForGetDescriptionByIds.RecordsByValue.Keys });

                if (descriptions != null)
                    SetDescription(groupedValuesForGetDescriptionByIds.RecordsByValue, descriptions);
            }
        }

        #region Private Methods
        private static void SetDescription(Dictionary<object, List<ISetRecordDescription>> recordsByValue, Dictionary<object, string> descriptionsByFieldValue)
        {
            if (recordsByValue == null)
                return;

            foreach (var kvp in descriptionsByFieldValue)
            {
                var fieldValue = kvp.Key;
                var description = kvp.Value;

                List<ISetRecordDescription> recordGroupedValues = recordsByValue.GetRecord(fieldValue);
                recordGroupedValues.ThrowIfNull("recordGroupedValues", fieldValue);

                foreach (var recordGroupedValue in recordGroupedValues)
                {
                    recordGroupedValue.SetDescription(new SetRecordDescriptionContext { Description = description });
                }
            }
        }

        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Business;
using Vanrise.Analytic.Business;

namespace Vanrise.Analytic.MainExtensions.AutomatedReport.Queries
{
    public class RecordSearchQuerySettings : VRAutomatedReportQuerySettings
    {
        public List<RecordSearchQueryDataRecordStorage> DataRecordStorages { get; set; }

        public VRTimePeriod TimePeriod { get; set; }

        public List<RecordSearchQueryColumn> Columns { get; set; }

        public RecordFilterGroup FilterGroup { get; set; }

        public int LimitResult { get; set; }

        public OrderDirection Direction { get; set; }

        public List<SortColumn> SortColumns { get; set; }

        public List<DataRecordFilter> Filters { get; set; }

        public override VRAutomatedReportDataResult Execute(IVRAutomatedReportQueryExecuteContext context)
        {
            VRAutomatedReportDataResult automatedReportDataResult = new VRAutomatedReportDataResult
            {
                Lists = new Dictionary<string, VRAutomatedReportDataList>(),
                Fields = new Dictionary<string, VRAutomatedReportDataFieldValue>()
            };
            VRAutomatedReportDataList automatedreportDataList = new VRAutomatedReportDataList(){
                Items = new List<VRAutomatedReportDataItem>()
            };
            DataRecordStorageManager dataRecordStorageManager = new DataRecordStorageManager();
            List<Guid> dataRecordStorageIds = new List<Guid>();
            List<string> columnNames = new List<string>();

            if (this.DataRecordStorages != null && this.DataRecordStorages.Count != 0)
            {
                foreach (var dataRecordStorage in this.DataRecordStorages)
                {
                    dataRecordStorageIds.Add(dataRecordStorage.DataRecordStorageId);
                }
            }
            if (this.Columns != null && this.Columns.Count != 0)
            {
                foreach (var column in this.Columns)
                {
                    if (column != null)
                    {
                        columnNames.Add(column.ColumnName);
                    }
                }
            }
            DataRetrievalInput<DataRecordQuery> input = new DataRetrievalInput<DataRecordQuery>()
            {
                DataRetrievalResultType = DataRetrievalResultType.Normal,
                Query = new DataRecordQuery
                {
                    DataRecordStorageIds = dataRecordStorageIds,
                    Columns = columnNames,
                    ColumnTitles = columnNames,
                    FilterGroup = this.FilterGroup,
                    LimitResult = this.LimitResult,
                    Direction = this.Direction,
                    SortColumns = this.SortColumns,
                    Filters = this.Filters
                }
            };
            var dataRecords = dataRecordStorageManager.GetFilteredDataRecords(input) as BigResult<DataRecordDetail>;

            if (dataRecords != null && dataRecords.Data != null)
            {
                var data = dataRecords.Data;
                foreach (var dataItem in data)
                {
                    VRAutomatedReportDataItem item = new VRAutomatedReportDataItem();
                    Dictionary<string, VRAutomatedReportDataFieldValue> fields = new Dictionary<string, VRAutomatedReportDataFieldValue>();
                    if(dataItem.FieldValues!=null && dataItem.FieldValues.Count!=0){
                        foreach(var fieldValue in dataItem.FieldValues){
                            fields.GetOrCreateItem(fieldValue.Key,
                            () =>
                            {
                                VRAutomatedReportDataFieldValue dataFieldValue = new VRAutomatedReportDataFieldValue()
                                {
                                    Value = fieldValue.Value!=null ? fieldValue.Value.Value : null
                                };
                                return dataFieldValue;
                            });
                        }
                    }
                    item.Fields = fields;
                    automatedreportDataList.Items.Add(item);
                }
                automatedReportDataResult.Lists.Add("Main", automatedreportDataList);
            }
            return automatedReportDataResult;
        }

        public override VRAutomatedReportDataSchema GetSchema(IVRAutomatedReportQueryGetSchemaContext context)
        {
            VRAutomatedReportDataSchema automatedReportSchema = new VRAutomatedReportDataSchema();
            Dictionary<string, VRAutomatedReportDataListSchema> listSchema = new Dictionary<string, VRAutomatedReportDataListSchema>();
            List<string> columnNames = new List<string>();
            foreach(var column in this.Columns)
            {
                if (column != null)
                {
                    columnNames.Add(column.ColumnName);
                }
            }
            listSchema.GetOrCreateItem("Main",
            () =>
            {
                VRAutomatedReportDataListSchema listSchemaItem = new VRAutomatedReportDataListSchema();
                Dictionary<string, VRAutomatedReportDataFieldSchema> fieldSchema = new Dictionary<string, VRAutomatedReportDataFieldSchema>();
                VRAutomatedReportQueryDefinitionManager automatedReportQueryDefinitionManager = new VRAutomatedReportQueryDefinitionManager();
                var automatedReportQueryDefinitionSettings = automatedReportQueryDefinitionManager.GetVRAutomatedReportQueryDefinitionSettings(context.QueryDefinitionId);
                automatedReportQueryDefinitionSettings.ThrowIfNull("automatedReportQueryDefinitionSettings");
                automatedReportQueryDefinitionSettings.ExtendedSettings.ThrowIfNull("automatedReportQueryDefinitionSettings.ExtendedSettings");
                var recordSearchQueryDefinitionSettings = automatedReportQueryDefinitionSettings.ExtendedSettings.CastWithValidate<RecordSearchQueryDefinitionSettings>("automatedReportQueryDefinitionSettings.ExtendedSettings");
                recordSearchQueryDefinitionSettings.ThrowIfNull("recordSearchQueryDefinitionSettings");
                DataRecordTypeManager dataRecordTypeManager = new DataRecordTypeManager();
                var dataRecordTypeFields = dataRecordTypeManager.GetDataRecordTypeFields(recordSearchQueryDefinitionSettings.DataRecordTypeId);
                foreach (var field in dataRecordTypeFields)
                {
                    var fieldName = field.Key;
                    if (columnNames.Contains(fieldName))
                    {
                        fieldSchema.GetOrCreateItem(fieldName,
                        () =>
                        {
                            VRAutomatedReportDataFieldSchema fieldSchemaItem = new VRAutomatedReportDataFieldSchema
                            {
                                Field = field.Value
                            };
                            return fieldSchemaItem;
                        });
                    }
                }
                listSchemaItem.FieldSchemas = fieldSchema;
                return listSchemaItem;
            });
            automatedReportSchema.ListSchemas = listSchema;
            return automatedReportSchema;
        }
    }

    public class RecordSearchQueryDataRecordStorage
    {
        public Guid DataRecordStorageId { get; set; }
    }

    public class RecordSearchQueryColumn
    {
        public string ColumnName { get; set; }

        public string ColumnTitle { get; set; }
    }
}

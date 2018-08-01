using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;
using Vanrise.Common;

namespace Vanrise.Analytic.Entities
{
    public class VRAutomatedReportHandlerExecuteContext : IVRAutomatedReportHandlerExecuteContext
    {
        private List<VRAutomatedReportQuery> Queries { get; set; }

        public Guid? TaskId{get; set;}
        public IAutomatedReportEvaluatorContext EvaluatorContext { get; set; }
        public VRReportGenerationFilter Filter {get;set;}
        public VRReportGenerationRuntimeFilter RuntimeFilter { get; set; }
        public VRAutomatedReportHandlerExecuteContext(List<VRAutomatedReportQuery> queries, Guid? taskId, IAutomatedReportEvaluatorContext evaluatorContext, VRReportGenerationFilter filter, VRReportGenerationRuntimeFilter runtimeFilter)
        {
            Queries = queries;
            TaskId = taskId;
            this.EvaluatorContext = evaluatorContext;
            this.Filter = filter;
            this.RuntimeFilter = runtimeFilter;
        }

        public VRAutomatedReportResolvedDataList GetDataList(Guid vrAutomatedReportQueryId, string listName)
        {
            VRAutomatedReportResolvedDataList resolvedDataList = new VRAutomatedReportResolvedDataList()
            {
                FieldInfos = new Dictionary<string,VRAutomatedReportFieldInfo>(),
                Items = new List<VRAutomatedReportResolvedDataItem>()
            };
            if (this.Queries != null && this.Queries.Count > 0)
            {
                var matchingQuery = this.Queries.FindRecord(x => x.VRAutomatedReportQueryId == vrAutomatedReportQueryId);
                if (matchingQuery != null)
                {
                    matchingQuery.Settings.ThrowIfNull("matchingQuery.Settings", vrAutomatedReportQueryId);

                    var automatedReportDataResult = matchingQuery.Settings.Execute(new VRAutomatedReportQueryExecuteContext()
                    {
                        QueryDefinitionId = matchingQuery.DefinitionId,
                        Filter = Filter,
                        RuntimeFilter = RuntimeFilter
                    });

                    var querySchema = matchingQuery.Settings.GetSchema(new VRAutomatedReportQueryGetSchemaContext() 
                    { 
                        QueryDefinitionId = matchingQuery.DefinitionId
                    });

                    if (automatedReportDataResult != null && querySchema!=null)
                    {
                        if (automatedReportDataResult.Lists != null && automatedReportDataResult.Lists.Count > 0 && querySchema.ListSchemas != null && querySchema.ListSchemas.Count > 0)
                        {
                            var mainList = automatedReportDataResult.Lists.GetRecord(listName);
                            var mainSchema = querySchema.ListSchemas.GetRecord(listName);
                            if (mainList != null && mainSchema != null)
                            {
                                var fieldSchemas = mainSchema.FieldSchemas;
                                fieldSchemas.ThrowIfNull("fieldSchemas");
                                var subTablesSchemas = mainSchema.SubTablesSchemas;
                                if (mainList.Items != null && mainList.Items.Count > 0)
                                {
                                    foreach (var item in mainList.Items)
                                    {
                                        item.Fields.ThrowIfNull("item.Fields", matchingQuery.DefinitionId);
                                        VRAutomatedReportResolvedDataItem resolvedItem = new VRAutomatedReportResolvedDataItem()
                                        {
                                            Fields = new Dictionary<string, VRAutomatedReportResolvedDataFieldValue>()
                                        };
                                        foreach (var field in item.Fields)
                                        {
                                            if (field.Value != null)
                                            {
                                                var fieldInfo = fieldSchemas.GetRecord(field.Key);
                                                if (fieldInfo != null && fieldInfo.Field != null && fieldInfo.Field != null)
                                                {
                                                    VRAutomatedReportResolvedDataFieldValue resolvedFieldValue = new VRAutomatedReportResolvedDataFieldValue()
                                                    {
                                                        Description = fieldInfo.Field.Type.GetDescription(field.Value.Value),
                                                        Value = field.Value.Value
                                                    };

                                                    resolvedItem.Fields.Add(field.Key, resolvedFieldValue);
                                                    if (!resolvedDataList.FieldInfos.ContainsKey(field.Key))
                                                    {
                                                        var reportFieldInfo = new VRAutomatedReportFieldInfo()
                                                        {
                                                            FieldTitle = fieldInfo.Field.Title,
                                                            FieldType = fieldInfo.Field.Type
                                                        };
                                                        resolvedDataList.FieldInfos.Add(field.Key, reportFieldInfo);
                                                    }
                                                }
                                            }
                                        }
                                        if (item.SubTables != null && item.SubTables.Count > 0 && subTablesSchemas!=null && subTablesSchemas.Count>0)
                                        {
                                            resolvedItem.SubTables = new Dictionary<Guid, VRAutomatedReportResolvedDataItemSubTable>();
                               
                                            foreach (var subTable in item.SubTables)
                                            {
                                                if (subTable.Value != null)
                                                {
                                                    VRAutomatedReportResolvedDataItemSubTable resolvedDataItemSubTable = new VRAutomatedReportResolvedDataItemSubTable()
                                                    {
                                                        Fields = new Dictionary<string, VRAutomatedReportResolvedDataItemSubTableFieldInfo>()
                                                    };
                                                    var subTableSchema = subTablesSchemas.GetRecord(subTable.Key);
                                                    if (subTableSchema != null && subTableSchema.FieldSchemas!=null && subTable.Value.Fields != null && subTable.Value.Fields.Count > 0)
                                                    {
                                                        foreach (var measure in subTable.Value.Fields)
                                                        {
                                                            var measureSchema = subTableSchema.FieldSchemas.GetRecord(measure.Key);
                                                            if (measure.Value != null && measure.Value.FieldsValues!=null && measureSchema!=null && measureSchema.Field!=null)
                                                            {
                                                                VRAutomatedReportResolvedDataItemSubTableFieldInfo subTableFieldInfo = new VRAutomatedReportResolvedDataItemSubTableFieldInfo()
                                                                {
                                                                    FieldValues = new List<VRAutomatedReportResolvedDataFieldValue>()
                                                                };
                                                                foreach (var measureValue in measure.Value.FieldsValues)
                                                                {
                                                                    subTableFieldInfo.FieldValues.Add(new VRAutomatedReportResolvedDataFieldValue()
                                                                    {
                                                                        Description = measureSchema.Field.Type.GetDescription(measureValue.Value),
                                                                        Value = measureValue.Value
                                                                    });
                                                                }
                                                                resolvedDataItemSubTable.Fields.Add(measure.Key, subTableFieldInfo);
                                                            }
                                                        }
                                                        resolvedItem.SubTables.Add(subTable.Key, resolvedDataItemSubTable);
                                                    }
                                                }
                                            }
                                        }
                                        resolvedDataList.Items.Add(resolvedItem);
                                    }
                                }
                                if (mainList.ItemTables != null && mainList.ItemTables.Count > 0)
                                {
                                    resolvedDataList.SubTablesInfo = new Dictionary<Guid, VRAutomatedReportTableInfo>();
                               
                                    foreach (var subTable in mainList.ItemTables)
                                    {
                                        if(subTable.Value!=null)
                                        {
                                            VRAutomatedReportTableInfo reportTableInfo = new VRAutomatedReportTableInfo()
                                            {
                                                FieldsInfo = new Dictionary<string, VRAutomatedReportTableFieldInfo>(),
                                                FieldsOrder = new List<string>()
                                            };
                                            var dimensionsSchema = matchingQuery.Settings.GetSubTableFields(new VRAutomatedReportQueryGetSubTableFieldsContext()
                                            {
                                                QueryDefinitionId = matchingQuery.DefinitionId,
                                                SubTableId = subTable.Key
                                            });
                                            if (dimensionsSchema != null && subTable.Value.Fields != null && subTable.Value.Fields.Count > 0)
                                            {
                                                foreach (var dimension in subTable.Value.Fields)
                                                {
                                                    var dimensionSchema = dimensionsSchema.GetRecord(dimension.Key);
                                                    if (dimension.Value != null && dimension.Value.FieldsValues!=null && dimension.Value.FieldsValues.Count>0 && dimensionSchema != null && dimensionSchema.Field!=null)
                                                    {
                                                        VRAutomatedReportTableFieldInfo tableFieldInfo = new VRAutomatedReportTableFieldInfo()
                                                        {
                                                            FieldType = dimensionSchema.Field.Type,
                                                            FieldValues = new List<VRAutomatedReportResolvedDataFieldValue>(),
                                                        };
                                                        foreach (var dimensionValue in dimension.Value.FieldsValues)
                                                        {
                                                            tableFieldInfo.FieldValues.Add(new VRAutomatedReportResolvedDataFieldValue()
                                                            {
                                                                Description = dimensionSchema.Field.Type.GetDescription(dimensionValue.Value),
                                                                Value = dimensionValue.Value
                                                            });
                                                        }
                                                        reportTableInfo.FieldsOrder.Add(dimension.Key);
                                                        reportTableInfo.FieldsInfo.Add(dimension.Key, tableFieldInfo);
                                                    }
                                                }
                                            }
                                            resolvedDataList.SubTablesInfo.Add(subTable.Key, reportTableInfo);
                                        }
                                    }
                                }

                                if (mainList.SummaryDataItem != null)
                                {
                                    resolvedDataList.SummaryDataItem = new VRAutomatedReportResolvedDataItem();
                                    if (mainList.SummaryDataItem.Fields != null && mainList.SummaryDataItem.Fields.Count > 0)
                                    {
                                        resolvedDataList.SummaryDataItem.Fields = new Dictionary<string, VRAutomatedReportResolvedDataFieldValue>();
                                        foreach (var field in mainList.SummaryDataItem.Fields)
                                        {
                                            var fieldInfo = fieldSchemas.GetRecord(field.Key);
                                            if (field.Value != null && fieldInfo!=null && fieldInfo.Field!=null)
                                            {
                                                resolvedDataList.SummaryDataItem.Fields.Add(field.Key, new VRAutomatedReportResolvedDataFieldValue()
                                                {
                                                    Description = fieldInfo.Field.Type.GetDescription(field.Value.Value),
                                                    Value = field.Value.Value
                                                });
                                            }
                                        }

                                    }
                                    if (subTablesSchemas != null && subTablesSchemas.Count>0 && mainList.SummaryDataItem.SubTables != null && mainList.SummaryDataItem.SubTables.Count > 0)
                                    {
                                        resolvedDataList.SummaryDataItem.SubTables = new Dictionary<Guid, VRAutomatedReportResolvedDataItemSubTable>();
                                        foreach (var subtable in mainList.SummaryDataItem.SubTables)
                                        {
                                            if (subtable.Value != null && subtable.Value.Fields!=null && subtable.Value.Fields.Count>0)
                                            {
                                                VRAutomatedReportResolvedDataItemSubTable resolvedDataItemSubTable = new VRAutomatedReportResolvedDataItemSubTable()
                                                {
                                                    Fields = new Dictionary<string, VRAutomatedReportResolvedDataItemSubTableFieldInfo>()
                                                };
                                                var subTableSchema = subTablesSchemas.GetRecord(subtable.Key);
                                                if (subTableSchema != null && subTableSchema.FieldSchemas!=null && subTableSchema.FieldSchemas.Count>0)
                                                {
                                                    foreach (var measure in subtable.Value.Fields)
                                                    {
                                                        var measureSchema = subTableSchema.FieldSchemas.GetRecord(measure.Key);
                                                        if (measure.Value != null && measure.Value.FieldsValues!=null && measure.Value.FieldsValues.Count>0 && measureSchema != null && measureSchema.Field!=null)
                                                        {
                                                            VRAutomatedReportResolvedDataItemSubTableFieldInfo subTableFieldInfo = new VRAutomatedReportResolvedDataItemSubTableFieldInfo()
                                                            {
                                                                FieldValues = new List<VRAutomatedReportResolvedDataFieldValue>()
                                                            };
                                                            foreach (var measureValue in measure.Value.FieldsValues)
                                                            {
                                                                subTableFieldInfo.FieldValues.Add(new VRAutomatedReportResolvedDataFieldValue()
                                                                {
                                                                    Description = measureSchema.Field.Type.GetDescription(measureValue.Value),
                                                                    Value = measureValue.Value
                                                                });
                                                            }
                                                            resolvedDataItemSubTable.Fields.Add(measure.Key, subTableFieldInfo);
                                                        }
                                                    }
                                                    resolvedDataList.SummaryDataItem.SubTables.Add(subtable.Key, resolvedDataItemSubTable);
                                                }
                                            }
                                        }
                                    }

                                }
                            }
                        }
                    }
                }
            }
            return resolvedDataList;
        }

        public VRAutomatedReportResolvedDataFieldValue GetDataField(Guid vrAutomatedReportQueryId, string fieldName)
        {
            return null;
        }


    }
}

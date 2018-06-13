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

        public VRAutomatedReportHandlerExecuteContext(List<VRAutomatedReportQuery> queries)
        {
            Queries = queries;
        }

        public VRAutomatedReportResolvedDataList GetDataList(Guid vrAutomatedReportQueryId, string listName)
        {
            VRAutomatedReportResolvedDataList resolvedDataList = new VRAutomatedReportResolvedDataList()
            { 
                FieldInfos = new Dictionary<string,VRAutomatedReportFieldInfo>(),
                Items = new List<VRAutomatedReportResolvedDataItem>()
            };
            if (this.Queries != null && this.Queries.Count!=0)
            {
                var queries = this.Queries;
                var matchingQuery = queries.Find(x => x.VRAutomatedReportQueryId == vrAutomatedReportQueryId);
                if (matchingQuery != null)
                {
                    matchingQuery.Settings.ThrowIfNull("matchingQuery.Settings", vrAutomatedReportQueryId);
                    VRAutomatedReportQueryExecuteContext reportQueryExecuteContext = new VRAutomatedReportQueryExecuteContext(){
                        QueryDefinitionId = matchingQuery.DefinitionId
                    };
                    var automatedReportDataResult = matchingQuery.Settings.Execute(reportQueryExecuteContext);

                    VRAutomatedReportQueryGetSchemaContext getSchemaContext = new VRAutomatedReportQueryGetSchemaContext()
                    {
                        QueryDefinitionId = matchingQuery.DefinitionId
                    };
                    var querySchema = matchingQuery.Settings.GetSchema(getSchemaContext);

                    if (automatedReportDataResult != null && querySchema!=null)
                    {
                        if (automatedReportDataResult.Lists != null && automatedReportDataResult.Lists.Count != 0 && querySchema.ListSchemas != null && querySchema.ListSchemas.Count != 0)
                        {
                            var mainList = automatedReportDataResult.Lists.GetRecord(listName);
                            var mainSchema = querySchema.ListSchemas.GetRecord(listName);
                            if (mainList != null && mainSchema != null)
                            {
                                var items = mainList.Items;
                                var fieldSchemas = mainSchema.FieldSchemas;

                                List<VRAutomatedReportResolvedDataItem> resolvedItems = new List<VRAutomatedReportResolvedDataItem>();
                                if (items != null && items.Count != 0)
                                {
                                    foreach (var item in items)
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
                                                if (fieldInfo != null && fieldInfo.Field != null && fieldInfo.Field.Type != null)
                                                {
                                                    VRAutomatedReportResolvedDataFieldValue resolvedFieldValue = new VRAutomatedReportResolvedDataFieldValue()
                                                    {
                                                        Description = fieldInfo.Field.Type.GetDescription(field.Value.Value),
                                                        Value = field.Value.Value
                                                    };

                                                    resolvedItem.Fields.Add(field.Key, resolvedFieldValue);
                                                    resolvedDataList.FieldInfos.GetOrCreateItem(field.Key,
                                                        () =>
                                                        {
                                                            return new VRAutomatedReportFieldInfo()
                                                        {
                                                            FieldTitle = fieldInfo.Field.Title,
                                                            FieldType = fieldInfo.Field.Type
                                                        };
                                                        });
                                                }
                                            }
                                        }
                                        resolvedItems.Add(resolvedItem);
                                    }
                                    resolvedDataList.Items = resolvedItems;
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

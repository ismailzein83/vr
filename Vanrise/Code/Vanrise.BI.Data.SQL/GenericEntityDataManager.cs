using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BI.Entities;

namespace Vanrise.BI.Data.SQL
{
    public class GenericEntityDataManager : BaseDataManager, IGenericEntityDataManager
    {

        List<BIConfiguration<BIConfigurationMeasure>> _measureDefinitions;
        List<BIConfiguration<BIConfigurationEntity>> _entityDefinitions;
        public List<BIConfiguration<BIConfigurationMeasure>> MeasureDefinitions
        {
            set { _measureDefinitions = value; }
        }
        public List<BIConfiguration<BIConfigurationEntity>> EntityDefinitions
        {
            set { _entityDefinitions = value; }
        }

        public IEnumerable<TimeValuesRecord> GetMeasureValues(TimeDimensionType timeDimensionType, DateTime fromDate, DateTime toDate, List<String> customerIds, List<String> supplierIds,string customerColumnId, params string[] measureTypeNames)
        {
            return GetTimeValuesRecord(timeDimensionType, fromDate, toDate, null, measureTypeNames, customerIds, supplierIds, customerColumnId);
        }
        public IEnumerable<TimeValuesRecord> GetEntityMeasuresValues(List<string> entityTypeName, string entityId, TimeDimensionType timeDimensionType, DateTime fromDate, DateTime toDate, List<String> customerIds, List<String> supplierIds,string customerColumnId, params string[] measureTypesNames)
        {
            List<string> entityIdColumn;
            List<string> entityNameColumn;
            GetEntityColumns(entityTypeName, out entityIdColumn, out entityNameColumn);
            string[] additionalFilters = new string[] { BuildQueryColumnFilter(entityIdColumn, entityId) };
            return GetTimeValuesRecord(timeDimensionType, fromDate, toDate, additionalFilters, measureTypesNames, customerIds, supplierIds, customerColumnId);
        }
        public IEnumerable<EntityRecord> GetTopEntities(List<string> entityTypeName, string topByMeasureTypeName, DateTime fromDate, DateTime toDate, int topCount, List<String> queryFilter, params string[] measureTypesNames)
        {
            List<EntityRecord> rslt = new List<EntityRecord>();
            string topMeasureColExp;
            string topMeasureColumn = GetMeasureColumn(topByMeasureTypeName, out topMeasureColExp);

            List<string> entityIdColumn;
            List<string> entityNameColumn;
            GetEntityColumns(entityTypeName, out entityIdColumn, out entityNameColumn);

            string[] measureColumns;
            string expressionsPart;
            GetMeasuresColumns(measureTypesNames, out measureColumns, out expressionsPart);

            if (!measureTypesNames.Contains(topByMeasureTypeName))
            {
                if (expressionsPart == null) 
                    expressionsPart = topMeasureColExp;
                else
                    expressionsPart = string.Format(@"{0} 
                                                                {1}", expressionsPart, topMeasureColExp);
            }

            string columnsPart = BuildQueryColumnsPart(measureColumns);
            string rowsPart = "";
            if (queryFilter != null)
                rowsPart = BuildQueryTopRows(topMeasureColumn, topCount, queryFilter, entityIdColumn, entityNameColumn);
            else
                rowsPart = BuildQueryTopRowsParts(topMeasureColumn, topCount, entityNameColumn);
            string filtersPart = GetDateFilter(fromDate, toDate);
            string query = BuildQuery(columnsPart, rowsPart, filtersPart, expressionsPart);
            List<string> entityName;
            GetEntityName(entityTypeName, out entityName);
            ExecuteReaderMDX(query, (reader) =>
            {
                while (reader.Read())
                {
                    List<string> EntityId = new List<string>();

                    foreach (string entity in entityIdColumn)
                    {
                        if (queryFilter != null)
                            EntityId.Add(reader[GetRowColumnToRead(entity)] as string);
                    }
                    List<string> EntityName = new List<string>();
                    foreach (string entity in entityNameColumn)
                    {
                            EntityName.Add(reader[GetRowColumnToRead(entity)] as string);
                    }
                    EntityRecord entityValue = new EntityRecord
                    {

                        EntityId = EntityId,
                        EntityName = EntityName,
                        EntityType = entityName,
                        Values = new Decimal[measureColumns.Length]
                    };
                    for (int i = 0; i < measureColumns.Length; i++)
                    {
                        entityValue.Values[i] = Convert.ToDecimal(reader[measureColumns[i]]);
                    }
                    rslt.Add(entityValue);
                }
            });
            return rslt;
        }
       

        #region Private Methods
        protected string BuildQueryTopRowsParts(string columnBy, int count,  List<string> columnsNames)
        {
            StringBuilder columns=new StringBuilder();;
            foreach (string value in columnsNames){
                if(columns.Length==0)
                {
                    columns.Append(String.Format("{0}.CHILDREN",value));
                }
                else{
                    columns.Append(String.Format("* {0}.CHILDREN",value));
                } 

            }
            return String.Format(@"TopCount({0}, {1}, {2})", columns.ToString(), count, columnBy);
        }

        protected string BuildQueryTopRows(string columnBy, int count, List<String> queryFilter, List<string> entityIdColumn, List<string> entityNameColumn)
        {
            return String.Format(@"TopCount({0}, {1}, {2})", BuildQueryRows(queryFilter, entityIdColumn, entityNameColumn), count, columnBy);
        }
        protected string BuildQueryRows(List<String> queryFilter, List<string> entityIdColumn, List<string> entityNameColumn)
        {
            StringBuilder queryBuilder = null;
         
            queryBuilder = new StringBuilder();
            StringBuilder columns = new StringBuilder();
            foreach (string value in entityNameColumn)
            {
                if (columns.Length == 0)
                {
                    columns.AppendFormat("{0}.CHILDREN", value);
                }
                else
                {
                    columns.AppendFormat("* {0}.CHILDREN", value);
                }

            }
            StringBuilder ids = new StringBuilder();
            foreach (string id in entityIdColumn)
            {
                String FilterValue = BuildQueryRowsFilter(id, queryFilter);
                if (ids.Length == 0)
                {
                    ids.AppendFormat("Filter({0}.CHILDREN as FilterValue,{1} )", id, FilterValue);
                }
                else
                {
                    ids.AppendFormat("* Filter({0}.CHILDREN as FilterValue,{1} )", id, FilterValue);
                }

            }
            queryBuilder.AppendFormat("{0} * {1}", ids.ToString(), columns.ToString());
            return queryBuilder.ToString();
        }
        protected string BuildQueryRowsFilter(string entityIdColumn, List<String> queryFilter)
        {
            StringBuilder queryBuilder = null;
            foreach (var value in queryFilter)
            {
                if (queryBuilder == null)
                    queryBuilder = new StringBuilder();
                else
                    queryBuilder.Append(" or ");
                queryBuilder.AppendFormat("FilterValue.currentmember.member_caption =   {0}.&[{1}].member_caption ", entityIdColumn, value);
            }
            return queryBuilder.ToString();
        }


        private void GetEntityName(List<string> entitiesTypeName, out List<string> entitiesName)
        {
            entitiesName = new List<string>();
           foreach (BIConfiguration<BIConfigurationEntity> obj in _entityDefinitions)
            {
                foreach (string entityName in entitiesTypeName)
                    if (entityName == obj.Name)
                    {
                        entitiesName.Add(obj.Name);
                    }
            }
       }

       void GetEntityColumns(List<string> entitiesTypeName, out List<string> idsColumn, out List<string> namesColumn)
        {
            idsColumn = new List<string>();
            namesColumn = new List<string>();

            foreach (BIConfiguration<BIConfigurationEntity> obj in _entityDefinitions)
            {
                foreach (string entityTypeName in entitiesTypeName)
                {
                    if (entityTypeName == obj.Name)
                    {
                        idsColumn.Add(obj.Configuration.ColumnID);
                        namesColumn.Add(obj.Configuration.ColumnName);
                    }
                }
       
            }

        }

        private IEnumerable<TimeValuesRecord> GetTimeValuesRecord(TimeDimensionType timeDimensionType, DateTime fromDate, DateTime toDate, string[] additionalFilters, string[] measureTypesNames, List<String> supplierIds, List<String> customerIds, string customerColumnId)
        {
            List<TimeValuesRecord> rslt = new List<TimeValuesRecord>();
            string[] measureColumns;
            string expressionsPart;
            GetMeasuresColumns(measureTypesNames, out measureColumns, out expressionsPart);
            string columnsPart = BuildQueryColumnsPart(measureColumns);
            string rowsPart = BuildQueryDateRowColumns(timeDimensionType);
            string[] filters = new string[additionalFilters != null ? additionalFilters.Length + 1 : 1];
            filters[0] = GetDateFilter(fromDate, toDate);
            if (additionalFilters != null)
            {
                for (int i = 0; i < additionalFilters.Length; i++)
                {
                    filters[i + 1] = additionalFilters[i];
                }
            }
            string filtersPart = BuildQueryFiltersPart(filters);

            string query;
            if (supplierIds.Count != 0 || customerIds.Count != 0)
                query = BuildQuery(columnsPart, rowsPart, filtersPart, supplierIds, customerIds, customerColumnId,expressionsPart);
            else
                query = BuildQuery(columnsPart, rowsPart, filtersPart, expressionsPart);

            ExecuteReaderMDX(query, (reader) =>
            {
                while (reader.Read())
                {
                    TimeValuesRecord valuesRecord = new TimeValuesRecord
                    {
                        Values = new Decimal[measureColumns.Length]
                    };
                    for (int i = 0; i < measureColumns.Length; i++)
                    {
                        valuesRecord.Values[i] = Convert.ToDecimal(reader[measureColumns[i]]);
                    }
                    FillTimeCaptions(valuesRecord, reader, timeDimensionType);
                    rslt.Add(valuesRecord);
                }
            });
            return rslt.OrderBy(itm => itm.Time);
        }
        protected string BuildQuery(string columnsPart, string rowsPartValue, string filtersPart, List<String> customerIds, List<String> supplierIds,string customerColumnId, string expressionsPart = null)
        {
            StringBuilder customerfilter = new StringBuilder();
            StringBuilder supplierfilter = new StringBuilder();
            if (customerIds.Count != 0)
            {
                foreach (string customer in customerIds)
                {
                    if (customerfilter.Length == 0)
                    {
                        customerfilter.Append("* filter(");
                        customerfilter.Append(customerColumnId);
                        customerfilter.Append(".children as FilterValue,");
                        
                    } 
                    else
                    {
                        customerfilter.Append(" or ");
                    }
                    customerfilter.Append("FilterValue.CurrentMember.member_caption=");
                    customerfilter.Append(customerColumnId);
                    customerfilter.Append(".&[");
                    customerfilter.Append(customer);
                    customerfilter.Append("].MEMBER_CAPTION ");
                }
                customerfilter.Append(") ");
            }
            if (supplierIds.Count != 0)
            {
                foreach (string supplier in supplierIds)
                {
                    if (supplierfilter == null)
                        supplierfilter.Append("* filter([SupplierAccounts].[Carrier Account ID].children as FilterValue,");
                    else
                    {
                        supplierfilter.Append(" or ");
                    }
                    supplierfilter.Append("FilterValue.CurrentMember.member_caption=[SupplierAccounts].[Carrier Account ID].&[");
                    supplierfilter.Append(supplier);
                    supplierfilter.Append("].MEMBER_CAPTION ");
                }
                supplierfilter.Append(")");
            }
            string rowsPart = null;
            if (!String.IsNullOrEmpty(rowsPartValue))
                rowsPart = string.Format(@",{0} ON ROWS", rowsPartValue);
            string query = string.Format(@"select {{{0}}} ON COLUMNS                               
                                    {1} 
                                    FROM [{2}]
                                    WHERE {3} {4} {5}", columnsPart, rowsPart, CubeName, filtersPart, customerfilter, supplierfilter);

            if (!String.IsNullOrEmpty(expressionsPart))
                return String.Format(@"WITH {0}
                                        {1}", expressionsPart, query);
            else
                return query;
        }
        private void GetMeasuresColumns(string[] measureTypesNames, out string[] measureColumns, out string expressionsPart)
        {
            measureColumns = new string[measureTypesNames.Length];
            StringBuilder expressionPartsBuilder = null;
            for (int i = 0; i < measureTypesNames.Length; i++)
            {
                string measureTypeID = measureTypesNames[i];

                string expr;
                measureColumns[i] = GetMeasureColumn(measureTypeID, out expr);
                if (!String.IsNullOrEmpty(expr))
                {
                    if (expressionPartsBuilder == null)
                        expressionPartsBuilder = new StringBuilder();
                    expressionPartsBuilder.AppendLine(expr);
                }
            }
            expressionsPart = expressionPartsBuilder != null ? expressionPartsBuilder.ToString() : null;
        }
        string GetMeasureColumn(string measureTypeName, out string queryExpression)
        {
            queryExpression = null;

            foreach (BIConfiguration<BIConfigurationMeasure> obj in _measureDefinitions)
            {
                if (measureTypeName == obj.Name)
                {
                    if (obj.Configuration.Expression != null && obj.Configuration.Expression!="")
                    {
                        queryExpression = obj.Configuration.Expression;
                      return  obj.Configuration.ColumnName;}
                    else
                        return obj.Configuration.ColumnName;
                }
            }
            return null;
        }
        public Decimal[] GetMeasureValues(DateTime fromDate, DateTime toDate, params string[] measureTypeNames)
        {
            return GetRecords( fromDate, toDate, null, measureTypeNames);
        }
        private Decimal[] GetRecords(DateTime fromDate, DateTime toDate, string[] additionalFilters, string[] measureTypesNames)
        {
            Decimal[] rslt = new Decimal[measureTypesNames.Length]; ;
            string[] measureColumns;
            string expressionsPart;
            GetMeasuresColumns(measureTypesNames, out measureColumns, out expressionsPart);
            string columnsPart = BuildQueryColumnsPart(measureColumns);
            string[] filters = new string[additionalFilters != null ? additionalFilters.Length + 1 : 1];
            filters[0] = GetDateFilter(fromDate, toDate);
            if (additionalFilters != null)
            {
                for (int i = 0; i < additionalFilters.Length; i++)
                {
                    filters[i + 1] = additionalFilters[i];
                }
            }
            string filtersPart = BuildQueryFiltersPart(filters);
            string query = BuildQuery(columnsPart,null, filtersPart, expressionsPart);

            ExecuteReaderMDX(query, (reader) =>
            {
                while (reader.Read())
                {

                    rslt = new Decimal[measureColumns.Length];
                    for (int i = 0; i < measureColumns.Length; i++)
                    {
                        rslt[i] = Convert.ToDecimal(reader[measureColumns[i]]);
                    }
                }
            });
            return rslt;
        }
 
   
       
        
        #endregion

    }
}


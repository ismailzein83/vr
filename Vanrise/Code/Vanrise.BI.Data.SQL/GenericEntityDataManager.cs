﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BI.Entities;

namespace Vanrise.BI.Data.SQL
{
    public class GenericEntityDataManager : BaseDataManager, IGenericEntityDataManager
    {
  


        #region Public Methods

        public IEnumerable<TimeValuesRecord> GetMeasureValues(TimeDimensionType timeDimensionType, DateTime fromDate, DateTime toDate, List<object> customerIds, List<object> supplierIds, string customerColumnId, BIConfigurationTimeEntity configurationTimeEntity, params string[] measureTypeNames)
        {
            return GetTimeValuesRecord(timeDimensionType, fromDate, toDate, null, measureTypeNames, customerIds, supplierIds, customerColumnId, configurationTimeEntity);
        }
        public IEnumerable<TimeValuesRecord> GetEntityMeasuresValues(List<string> entityTypeName, string entityId, TimeDimensionType timeDimensionType, DateTime fromDate, DateTime toDate, List<object> customerIds, List<object> supplierIds, string customerColumnId, BIConfigurationTimeEntity configurationTimeEntity, params string[] measureTypesNames)
        {
            List<string> entityIdColumn;
            List<string> entityNameColumn;
            GetEntityColumns(entityTypeName, out entityIdColumn, out entityNameColumn);
            string[] additionalFilters = new string[] { BuildQueryColumnFilter(entityIdColumn, entityId) };
            return GetTimeValuesRecord(timeDimensionType, fromDate, toDate, additionalFilters, measureTypesNames, customerIds, supplierIds, customerColumnId, configurationTimeEntity);
        }
        public IEnumerable<EntityRecord> GetTopEntities(List<string> entityTypeName, string topByMeasureTypeName, DateTime fromDate, DateTime toDate, int topCount, BIConfigurationTimeEntity configurationTimeEntity, List<DimensionFilter> filter, params string[] measureTypesNames)
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
            string rowsPart = BuildQueryTopRows(topMeasureColumn, topCount, filter, entityIdColumn, entityNameColumn);

            string filtersPart = GetDateFilter(fromDate, toDate, configurationTimeEntity);
            string query = BuildQuery(columnsPart, rowsPart, filtersPart, expressionsPart);
            List<string> entityName;
            GetEntityName(entityTypeName, out entityName);
            List<string> filterCulomnsIds;
            GetFilterEntityColumns(filter, out filterCulomnsIds);
            ExecuteReaderMDX(query, (reader) =>
            {
                while (reader.Read())
                {
                    List<string> EntityId = new List<string>();
                    if (filter != null && filter.Count > 0)
                    {
                        foreach (string id in filterCulomnsIds)
                        {
                            EntityId.Add(reader[GetRowColumnToRead(id)] as string);
                        }
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
        public Decimal[] GetSummaryMeasureValues(DateTime fromDate, DateTime toDate, BIConfigurationTimeEntity configurationTimeEntity, params string[] measureTypeNames)
        {

            return GetRecords(fromDate, toDate, null, measureTypeNames, configurationTimeEntity);
        }

        #endregion

        #region Private Methods
      
        protected string BuildQueryTopRows(string columnBy, int count, List<DimensionFilter> queryFilter, List<string> entityIdColumn, List<string> entityNameColumn)
        {
            if (queryFilter != null && queryFilter.Count > 0)
            {
                return String.Format(@"TopCount(NONEMPTYCROSSJOIN({0}), {1}, {2})", BuildQueryRows(queryFilter, entityIdColumn, entityNameColumn), count, columnBy);
            }
            return BuildQueryTopRowsParts(columnBy, count, entityNameColumn);
        }
        protected string BuildQueryRows(List<DimensionFilter> queryFilter, List<string> entityIdColumn, List<string> entityNameColumn)
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
                foreach (var value in queryFilter)
                {
                    foreach (BIConfiguration<BIConfigurationEntity> obj in _entityDefinitions)
                    {
                        if (obj.Name == value.EntityName && obj.Configuration.ColumnID == id)
                        {
                            String FilterValue = BuildQueryRowsFilter(id, value.Values);
                            if (ids.Length == 0)
                            {
                                ids.AppendFormat("Filter({0}.CHILDREN as FilterValue,{1} )", id, FilterValue);
                            }
                            else
                            {
                                ids.AppendFormat("* Filter({0}.CHILDREN as FilterValue,{1} )", id, FilterValue);
                            }
                        }
                    }
                }



            }
            queryBuilder.AppendFormat("{0} * {1}", ids.ToString(), columns.ToString());
            return queryBuilder.ToString();
        }
        protected string BuildQueryTopRowsParts(string columnBy, int count, List<string> columnsNames)
        {
            StringBuilder columns = new StringBuilder(); ;
            foreach (string value in columnsNames)
            {
                if (columns.Length == 0)
                {
                    columns.Append(String.Format("NONEMPTYCROSSJOIN({0}.CHILDREN", value));
                }
                else
                {
                    columns.Append(String.Format(", {0}.CHILDREN", value));
                }

            }
            columns.Append(String.Format(")"));
            return String.Format(@"TopCount({0}, {1}, {2})", columns.ToString(), count, columnBy);
        }
        protected string BuildQueryRowsFilter(string entityIdColumn, List<Object> queryFilter)
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

            foreach (string entityTypeName in entitiesTypeName)
            {
                foreach (BIConfiguration<BIConfigurationEntity> obj in _entityDefinitions)
                {

                    if (entityTypeName == obj.Name)
                    {
                        idsColumn.Add(obj.Configuration.ColumnID);
                        namesColumn.Add(obj.Configuration.ColumnName);
                    }


                }
            }

        }
        void GetFilterEntityColumns(List<DimensionFilter> filter, out List<string> idsColumn)
        {
            idsColumn = new List<string>();
            if(filter != null)
            {
                foreach (var entityTypeName in filter)
                {
                    foreach (BIConfiguration<BIConfigurationEntity> obj in _entityDefinitions)
                    {

                        if (entityTypeName.EntityName == obj.Name)
                        {
                            idsColumn.Add(obj.Configuration.ColumnID);
                        }
                    }
                }

            }
        }
        private IEnumerable<TimeValuesRecord> GetTimeValuesRecord(TimeDimensionType timeDimensionType, DateTime fromDate, DateTime toDate, string[] additionalFilters, string[] measureTypesNames, List<object> supplierIds, List<object> customerIds, string customerColumnId, BIConfigurationTimeEntity configurationTimeEntity)
        {
            List<TimeValuesRecord> rslt = new List<TimeValuesRecord>();
            string[] measureColumns;
            string expressionsPart;
            GetMeasuresColumns(measureTypesNames, out measureColumns, out expressionsPart);
            string columnsPart = BuildQueryColumnsPart(measureColumns);


            string rowsPart = BuildQueryDateRowColumns(timeDimensionType, configurationTimeEntity);
            string[] filters = new string[additionalFilters != null ? additionalFilters.Length + 1 : 1];




            filters[0] = GetDateFilter(fromDate, toDate, configurationTimeEntity);
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
                query = BuildQuery(columnsPart, rowsPart, filtersPart, supplierIds, customerIds, customerColumnId, expressionsPart);
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
                    FillTimeCaptions(valuesRecord, reader, timeDimensionType, configurationTimeEntity);
                    rslt.Add(valuesRecord);
                }
            });
            return rslt.OrderBy(itm => itm.Time);
        }
        protected string BuildQuery(string columnsPart, string rowsPartValue, string filtersPart, List<object> customerIds, List<object> supplierIds, string customerColumnId, string expressionsPart = null)
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
                    if (obj.Configuration.Expression != null && obj.Configuration.Expression != "")
                    {
                        queryExpression = obj.Configuration.Expression;
                        return obj.Configuration.ColumnName;
                    }
                    else
                        return obj.Configuration.ColumnName;
                }
            }
            return null;
        }
        private Decimal[] GetRecords(DateTime fromDate, DateTime toDate, string[] additionalFilters, string[] measureTypesNames, BIConfigurationTimeEntity configurationTimeEntity)
        {
            Decimal[] rslt = new Decimal[measureTypesNames.Length]; ;
            string[] measureColumns;
            string expressionsPart;
            GetMeasuresColumns(measureTypesNames, out measureColumns, out expressionsPart);
            string columnsPart = BuildQueryColumnsPart(measureColumns);
            string[] filters = new string[additionalFilters != null ? additionalFilters.Length + 1 : 1];
            filters[0] = GetDateFilter(fromDate, toDate, configurationTimeEntity);
            if (additionalFilters != null)
            {
                for (int i = 0; i < additionalFilters.Length; i++)
                {
                    filters[i + 1] = additionalFilters[i];
                }
            }
            string filtersPart = BuildQueryFiltersPart(filters);
            string query = BuildQuery(columnsPart, null, filtersPart, expressionsPart);

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


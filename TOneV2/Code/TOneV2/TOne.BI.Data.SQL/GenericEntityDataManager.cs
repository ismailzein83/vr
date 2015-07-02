using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BI.Entities;

namespace TOne.BI.Data.SQL
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

        public IEnumerable<TimeValuesRecord> GetMeasureValues(TimeDimensionType timeDimensionType, DateTime fromDate, DateTime toDate, params MeasureType[] measureTypes)
        {
            return GetTimeValuesRecord(timeDimensionType, fromDate, toDate, null, measureTypes);
        }
        
        public IEnumerable<TimeValuesRecord> GetEntityMeasuresValues(EntityType entityType, string entityId, TimeDimensionType timeDimensionType, DateTime fromDate, DateTime toDate, params MeasureType[] measureTypes)
        {            
            string entityIdColumn;
            string entityNameColumn;
            GetEntityColumns(entityType, out entityIdColumn, out entityNameColumn);
            string[] additionalFilters = new string[] { BuildQueryColumnFilter(entityIdColumn, entityId) };
            return GetTimeValuesRecord(timeDimensionType, fromDate, toDate, additionalFilters, measureTypes);
        }

        public IEnumerable<GenericEntityRecord> GetTopEntities(EntityType entityType, MeasureType topByMeasureType, DateTime fromDate, DateTime toDate, int topCount, params MeasureType[] measureTypes)
        {
            List<GenericEntityRecord> rslt = new List<GenericEntityRecord>();
            string topMeasureColExp;
            string topMeasureColumn = GetMeasureColumn(topByMeasureType, out topMeasureColExp);

            string entityIdColumn;
            string entityNameColumn;
            GetEntityColumns(entityType, out entityIdColumn, out entityNameColumn);

            string[] measureColumns;
            string expressionsPart;
            GetMeasuresColumns(measureTypes, out measureColumns, out expressionsPart);

            if (!measureTypes.Contains(topByMeasureType))
            {
                if (expressionsPart == null)
                    expressionsPart = topMeasureColExp;
                else
                    expressionsPart = string.Format(@"{0} 
                                                        {1}", expressionsPart, topMeasureColExp);
            }

            string columnsPart = BuildQueryColumnsPart(measureColumns);
            string rowsPart = BuildQueryTopRowsPart(topMeasureColumn, topCount, entityIdColumn, entityNameColumn);
            string filtersPart = GetDateFilter(fromDate, toDate);
            string query = BuildQuery(columnsPart, rowsPart, filtersPart, expressionsPart);

            ExecuteReaderMDX(query, (reader) =>
            {
                while (reader.Read())
                {
                    GenericEntityRecord entityValue = new GenericEntityRecord
                    {
                        EntityId = reader[GetRowColumnToRead(entityIdColumn)] as string,
                        EntityName = reader[GetRowColumnToRead(entityNameColumn)] as string,
                        EntityType = entityType,
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

        private IEnumerable<TimeValuesRecord> GetTimeValuesRecord(TimeDimensionType timeDimensionType, DateTime fromDate, DateTime toDate, string[] additionalFilters, MeasureType[] measureTypes)
        {
            List<TimeValuesRecord> rslt = new List<TimeValuesRecord>();
            string[] measureColumns;
            string expressionsPart;

           // GetMeasuresColumns1(measureTypes, _measureDefinitions, out expressionsPart);

            GetMeasuresColumns(measureTypes, out measureColumns, out expressionsPart);
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
            string query = BuildQuery(columnsPart, rowsPart, filtersPart, expressionsPart);

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

        string GetMeasureColumn(MeasureType measureType, out string queryExpression)
        {
            queryExpression = null;
            switch(measureType)
            {
                case MeasureType.DurationInMinutes: return MeasureColumns.DURATION_IN_MINUTES;
                case MeasureType.Sale: return MeasureColumns.SALE;
                case MeasureType.Cost: return MeasureColumns.COST;
                case MeasureType.Profit:
                    queryExpression = string.Format(@"MEMBER {0} AS ({1} - {2})", MeasureColumns.PROFIT, MeasureColumns.SALE, MeasureColumns.COST);
                    return MeasureColumns.PROFIT;
                case MeasureType.SuccessfulAttempts:
                    return MeasureColumns.SUCCESSFUL_ATTEMPTS;
                case MeasureType.ACD:
                    return MeasureColumns.ACD;
                case MeasureType.PDD:
                    return MeasureColumns.PDD;
            }
            return null;
        }

        void GetEntityColumns(EntityType entityType, out string idColumn, out string nameColumn)
        {
            idColumn = null;
            nameColumn = null;
            switch(entityType)
            {
                case EntityType.SaleZone:
                    idColumn = SaleZoneColumns.ZONE_ID;
                    nameColumn = SaleZoneColumns.ZONE_NAME;
                    break;
                case EntityType.Customer:
                    idColumn = CustomerAccountColumns.CARRIER_ACCOUNT_ID;
                    nameColumn = CustomerAccountColumns.PROFILE_NAME;
                    break;
                case EntityType.Supplier:
                    idColumn = SupplierAccountColumns.CARRIER_ACCOUNT_ID;
                    nameColumn = SupplierAccountColumns.PROFILE_NAME;
                    break;
            }
        }

        private void GetMeasuresColumns(MeasureType[] measureTypes, out string[] measureColumns, out string expressionsPart)
        {
            measureColumns = new string[measureTypes.Length];
            StringBuilder expressionPartsBuilder = null;
            for (int i = 0; i < measureTypes.Length; i++)
            {
                MeasureType measureType = measureTypes[i];
                string expr;
                measureColumns[i] = GetMeasureColumn(measureType, out expr);
                if (!String.IsNullOrEmpty(expr))
                {
                    if (expressionPartsBuilder == null)
                        expressionPartsBuilder = new StringBuilder();
                    expressionPartsBuilder.AppendLine(expr);
                }
            }
            expressionsPart = expressionPartsBuilder != null ? expressionPartsBuilder.ToString() : null;
        }

        // Test Page Methods Use below
        //
        //
        //
        //
        //
        //
        //
        //
        //

        public IEnumerable<TimeValuesRecord> GetMeasureValues(TimeDimensionType timeDimensionType, DateTime fromDate, DateTime toDate, params string[] measureTypeNames)
        {
            return GetTimeValuesRecord(timeDimensionType, fromDate, toDate, null, measureTypeNames);
        }
        public IEnumerable<TimeValuesRecord> GetEntityMeasuresValues(string entityTypeName, string entityId, TimeDimensionType timeDimensionType, DateTime fromDate, DateTime toDate, params string[] measureTypesNames)
        {
            string entityIdColumn;
            string entityNameColumn;
            GetEntityColumns(entityTypeName, out entityIdColumn, out entityNameColumn);
           string[] additionalFilters = new string[] { BuildQueryColumnFilter(entityIdColumn, entityId) };
           return GetTimeValuesRecord(timeDimensionType, fromDate, toDate, additionalFilters, measureTypesNames);
        }
        public IEnumerable<EntityRecord> GetTopEntities(string entityTypeName, string topByMeasureTypeName, DateTime fromDate, DateTime toDate, int topCount, params string[] measureTypesNames)
        {
            List<EntityRecord> rslt = new List<EntityRecord>();
            string topMeasureColExp;
            string topMeasureColumn = GetMeasureColumn(topByMeasureTypeName, out topMeasureColExp);

            string entityIdColumn;
            string entityNameColumn;
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
            string rowsPart = BuildQueryTopRowsPart(topMeasureColumn, topCount, entityIdColumn, entityNameColumn);
            string filtersPart = GetDateFilter(fromDate, toDate);
            string query = BuildQuery(columnsPart, rowsPart, filtersPart, expressionsPart);
            string entityName=GetEntityName(entityTypeName, out entityName);
            ExecuteReaderMDX(query, (reader) =>
            {
                while (reader.Read())
                {
                    EntityRecord entityValue = new EntityRecord
                    {
                        EntityId = reader[GetRowColumnToRead(entityIdColumn)] as string,
                        EntityName = reader[GetRowColumnToRead(entityNameColumn)] as string,
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

       private string GetEntityName(string entityTypeName,out string entityName){
           entityName = null;
           foreach (BIConfiguration<BIConfigurationEntity> obj in _entityDefinitions)
            {
                if (entityTypeName == obj.Name)
                {
                 return  obj.Name;
                }
            }
           return null;
       }

        void GetEntityColumns(string entityTypeName, out string idColumn, out string nameColumn)
        {
            idColumn = null;
            nameColumn = null;

            foreach (BIConfiguration<BIConfigurationEntity> obj in _entityDefinitions)
            {
                if (entityTypeName == obj.Name)
                {
                    idColumn = obj.Configuration.ColumnID;
                    nameColumn = obj.Configuration.ColumnName;
                }
            }

        }

        private IEnumerable<TimeValuesRecord> GetTimeValuesRecord(TimeDimensionType timeDimensionType, DateTime fromDate, DateTime toDate, string[] additionalFilters, string[] measureTypesNames)
        {
            List<TimeValuesRecord> rslt = new List<TimeValuesRecord>();
            string[] measureColumns;
            string expressionsPart;

            // GetMeasuresColumns1(measureTypes, _measureDefinitions, out expressionsPart);

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
            string query = BuildQuery(columnsPart, rowsPart, filtersPart, expressionsPart);

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
            string query = BuildQuery1(columnsPart,null, filtersPart, expressionsPart);

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
        private string BuildQuery1(string columnsPart, string rowsPart, string filtersPart, string expressionsPart = null)
        {
           string rowPart1= BuildRowPart(rowsPart);
            string query = string.Format(@"select {{{0}}} ON COLUMNS                                
                                    {1}
                                    FROM [{2}]
                                    WHERE {3}", columnsPart, rowPart1, CubeName, filtersPart);

            if (!String.IsNullOrEmpty(expressionsPart))
                return String.Format(@"WITH {0}
                                        {1}", expressionsPart, query);
            else
                return query;
        }
        private string BuildRowPart(string rowsPart){
            if (rowsPart != null)
                return string.Format(@",{0} ON ROWS", rowsPart);
            else
                return null;
        }
       
        
        #endregion

    }
}


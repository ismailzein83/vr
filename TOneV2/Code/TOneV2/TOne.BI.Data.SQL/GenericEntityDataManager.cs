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
        public IEnumerable<GenericEntityRecord> GetTopEntities(EntityType entityType, MeasureType measureType, DateTime fromDate, DateTime toDate, int topCount)
        {
            List<GenericEntityRecord> rslt = new List<GenericEntityRecord>();
            string expressionsPart;
            string measureColumn = GetMeasureColumn(measureType, out expressionsPart);
            string entityIdColumn;
            string entityNameColumn;                
            GetEntityColumns(entityType, out entityIdColumn, out entityNameColumn);
            string columnsPart = BuildQueryColumnsPart(measureColumn);
            string rowsPart = BuildQueryTopRowsPart(measureColumn, topCount, entityIdColumn, entityNameColumn);
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
                        Value = Convert.ToDecimal(reader[measureColumn])
                    };
                    rslt.Add(entityValue);
                }
            });
            return rslt;
        }

        public IEnumerable<TimeDimensionValueRecord> GetEntityMeasureValues(EntityType entityType, string entityId, MeasureType measureType, TimeDimensionType timeDimensionType, DateTime fromDate, DateTime toDate)
        {
            List<TimeDimensionValueRecord> rslt = new List<TimeDimensionValueRecord>();
            string expressionsPart;
            string measureColumn = GetMeasureColumn(measureType, out expressionsPart);
            string entityIdColumn;
            string entityNameColumn;
            GetEntityColumns(entityType, out entityIdColumn, out entityNameColumn);
            string columnsPart = BuildQueryColumnsPart(measureColumn);
            string rowsPart = BuildQueryDateRowColumns(timeDimensionType);
            string filtersPart = BuildQueryFiltersPart(GetDateFilter(fromDate, toDate), BuildQueryColumnFilter(entityIdColumn, entityId));
            string query = BuildQuery(columnsPart, rowsPart, filtersPart, expressionsPart);

            ExecuteReaderMDX(query, (reader) =>
            {
                while (reader.Read())
                {
                    TimeDimensionValueRecord valueRecord = new TimeDimensionValueRecord
                    {
                        Value = Convert.ToDecimal(reader[measureColumn])
                    };
                    if (valueRecord.Value > 0)
                    {
                        FillTimeCaptions(valueRecord, reader, timeDimensionType);
                        rslt.Add(valueRecord);
                    }
                }
            });
            return rslt.OrderBy(itm => itm.Time);
        }

        public IEnumerable<TimeValuesRecord> GetEntityMeasuresValues(EntityType entityType, string entityId, TimeDimensionType timeDimensionType, DateTime fromDate, DateTime toDate, params MeasureType[] measureTypes)
        {
            List<TimeValuesRecord> rslt = new List<TimeValuesRecord>();
            string[] measureColumns = new string[measureTypes.Length];
            StringBuilder expressionPartsBuilder = null;
            for (int i=0; i<measureTypes.Length;i++)
            {
                MeasureType measureType = measureTypes[i];
                string expr;
                measureColumns[i] = GetMeasureColumn(measureType, out expr);
                if(!String.IsNullOrEmpty(expr))
                {
                    if (expressionPartsBuilder == null)
                        expressionPartsBuilder = new StringBuilder();
                    expressionPartsBuilder.AppendLine(expr);
                }
            }
            string entityIdColumn;
            string entityNameColumn;
            GetEntityColumns(entityType, out entityIdColumn, out entityNameColumn);
            string columnsPart = BuildQueryColumnsPart(measureColumns);
            string rowsPart = BuildQueryDateRowColumns(timeDimensionType);
            string filtersPart = BuildQueryFiltersPart(GetDateFilter(fromDate, toDate), BuildQueryColumnFilter(entityIdColumn, entityId));
            string query = BuildQuery(columnsPart, rowsPart, filtersPart, expressionPartsBuilder != null ? expressionPartsBuilder.ToString() : null);

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

        
    }
}

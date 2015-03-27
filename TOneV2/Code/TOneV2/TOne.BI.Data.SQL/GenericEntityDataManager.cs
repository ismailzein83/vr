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
        public IEnumerable<GenericEntityRecord> GetTopEntities(EntityType entityType, MeasureType measureType, DateTime fromDate, DateTime toDate, int topCount, params MeasureType[] moreMeasures)
        {
            List<GenericEntityRecord> rslt = new List<GenericEntityRecord>();
            StringBuilder expressionPartsBuilder = new StringBuilder();
            string measureColExp;
            string measureColumn = GetMeasureColumn(measureType, out measureColExp);
            if (measureColExp != null)
                expressionPartsBuilder.AppendLine(measureColExp);
            string entityIdColumn;
            string entityNameColumn;                
            GetEntityColumns(entityType, out entityIdColumn, out entityNameColumn);
            string[] moreColumnsNames = null;
            if(moreMeasures != null && moreMeasures.Length > 0)
            {
                moreColumnsNames = new string[moreMeasures.Length];
                for (int i = 0; i < moreMeasures.Length; i++)
                {
                    string exp;
                    moreColumnsNames[i] = GetMeasureColumn(moreMeasures[i], out exp);
                    expressionPartsBuilder.AppendLine(exp);
                }
            }
            string[] allColumnNames = new string[moreColumnsNames != null ? moreColumnsNames.Length + 1 : 1];
            allColumnNames[0] = measureColumn;
            if(moreColumnsNames != null)
            {
                for (int i = 0; i < moreColumnsNames.Length; i++)
                {
                    allColumnNames[i + 1] = moreColumnsNames[i];
                }
            }
            string columnsPart = BuildQueryColumnsPart(allColumnNames);
            string rowsPart = BuildQueryTopRowsPart(measureColumn, topCount, entityIdColumn, entityNameColumn);
            string filtersPart = GetDateFilter(fromDate, toDate);
            string query = BuildQuery(columnsPart, rowsPart, filtersPart, expressionPartsBuilder.ToString());

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
                    if(moreColumnsNames != null && moreColumnsNames.Length > 0)
                    {
                        entityValue.MoreValues = new Decimal[moreColumnsNames.Length];
                        for (int i = 0; i < moreColumnsNames.Length; i++)
                        {
                            entityValue.MoreValues[i] = Convert.ToDecimal(reader[moreColumnsNames[i]]);
                        }

                    }
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

        
    }
}

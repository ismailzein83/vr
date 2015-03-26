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
            string measureColumn = GetMeasureColumn(measureType);
            string entityIdColumn;
            string entityNameColumn;
            GetEntityColumns(entityType, out entityIdColumn, out entityNameColumn);
            string columnsPart = BuildQueryColumnsPart(measureColumn);
            string rowsPart = BuildQueryTopRowsPart(measureColumn, topCount, entityIdColumn, entityNameColumn);
            string filtersPart = GetDateFilter(fromDate, toDate);
            string query = BuildQuery(columnsPart, rowsPart, filtersPart);

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

        public IEnumerable<TimeDimensionValueRecord> GetEntityMeasureValues(EntityType entityType, string entityValue, MeasureType measureType, TimeDimensionType timeDimensionType, DateTime fromDate, DateTime toDate)
        {
            List<TimeDimensionValueRecord> rslt = new List<TimeDimensionValueRecord>();
            string measureColumn = GetMeasureColumn(measureType); 
            string entityIdColumn;
            string entityNameColumn;
            GetEntityColumns(entityType, out entityIdColumn, out entityNameColumn);
            string columnsPart = BuildQueryColumnsPart(measureColumn);
            string rowsPart = BuildQueryDateRowColumns(timeDimensionType);
            string filtersPart = BuildQueryFiltersPart(GetDateFilter(fromDate, toDate), BuildQueryColumnFilter(entityIdColumn, entityValue));
            string query = BuildQuery(columnsPart, rowsPart, filtersPart);

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

        string GetMeasureColumn(MeasureType measureType)
        {
            switch(measureType)
            {
                case MeasureType.DurationInMinutes: return MeasureColumns.DURATION_IN_MINUTES;
                case MeasureType.Sale: return MeasureColumns.SALE;
                case MeasureType.Cost: return MeasureColumns.COST;
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

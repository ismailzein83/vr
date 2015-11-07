﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities.RatePlanning;
using Vanrise.Data.SQL;

namespace TOne.WhS.Sales.Data.SQL
{
    public class RatePlanDataManager : BaseSQLDataManager, IRatePlanDataManager
    {
        public RatePlanDataManager() : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString")) { }

        public bool InsertSalePriceList(SalePriceList salePriceList, out int salePriceListId)
        {
            object insertedId;

            int recordsAffected = ExecuteNonQuerySP("TOneWhS_Sales.sp_SalePriceList_Insert", out insertedId, salePriceList.OwnerType, salePriceList.OwnerId, salePriceList.CurrencyId);

            salePriceListId = (int)insertedId;

            return recordsAffected > 0;
        }

        public bool CloseAndInsertSaleRates(int customerId, List<SaleRate> newSaleRates)
        {
            DataTable newSaleRatesTable = BuildNewSaleRatesTable(newSaleRates);

            int recordsAffected = ExecuteNonQuerySPCmd("TOneWhS_Sales.sp_SaleRate_CloseAndInsert", (cmd) =>
            {
                cmd.Parameters.Add(new SqlParameter("@CustomerId", customerId));

                var tableParameter = new SqlParameter("@NewSaleRatesTable", SqlDbType.Structured);
                tableParameter.Value = newSaleRatesTable;
                cmd.Parameters.Add(tableParameter);
            });

            return recordsAffected >= newSaleRates.Count;
        }

        private DataTable BuildNewSaleRatesTable(List<SaleRate> newSaleRates)
        {
            DataTable table = new DataTable();

            table.Columns.Add("ID", typeof(long));
            table.Columns["ID"].AllowDBNull = true;

            table.Columns.Add("PriceListID", typeof(int));
            table.Columns.Add("ZoneID", typeof(long));
            table.Columns.Add("Rate", typeof(decimal));
            table.Columns.Add("BED", typeof(DateTime));
            table.Columns.Add("EED", typeof(DateTime));

            table.BeginLoadData();

            foreach (var saleRate in newSaleRates)
            {
                DataRow row = table.NewRow();

                if (saleRate.SaleRateId != 0)
                    row["ID"] = saleRate.SaleRateId;

                row["PriceListID"] = saleRate.PriceListId;
                row["ZoneID"] = saleRate.ZoneId;
                row["Rate"] = saleRate.NormalRate;
                row["BED"] = saleRate.BeginEffectiveDate;
                
                if (saleRate.EndEffectiveDate != null)
                    row["EED"] = saleRate.EndEffectiveDate;

                table.Rows.Add(row);
            }

            table.EndLoadData();

            return table;
        }

        public Changes GetChanges(RatePlanOwnerType ownerType, int ownerId, RatePlanStatus status)
        {
            return GetItemSP("TOneWhS_Sales.sp_RatePlan_GetChanges", ChangesMapper, ownerType, ownerId, status);
        }

        public bool InsertOrUpdateChanges(RatePlanOwnerType ownerType, int ownerId, Changes changes, RatePlanStatus status)
        {
            string serializedChanges = Vanrise.Common.Serializer.Serialize(changes);
            int affectedRows = ExecuteNonQuerySP("TOneWhS_Sales.sp_RatePlan_InsertOrUpdateChanges", ownerType, ownerId, serializedChanges, status);
            return affectedRows > 0;
        }

        private Changes ChangesMapper(IDataReader reader)
        {
            return Vanrise.Common.Serializer.Deserialize<Changes>(reader["Changes"] as string);
        }

        #region Junk Code
        /*
        public bool SetRatePlanStatusIfExists(RatePlanOwnerType ownerType, int ownerId, RatePlanStatus status)
        {
            ExecuteNonQuerySP("TOneWhS_Sales.sp_RatePlan_SetStatusIfExists", ownerType, ownerId, status);
            return true;
        }
        
        public RatePlan GetRatePlan(RatePlanOwnerType ownerType, int ownerId, RatePlanStatus status)
        {
            return GetItemSP("TOneWhS_Sales.sp_RatePlan_GetByStatus", RatePlanMapper, ownerType, ownerId, status);
        }

        public bool InsertOrUpdateRatePlan(RatePlan ratePlan)
        {
            string serializedRatePlanItems = Vanrise.Common.Serializer.Serialize(ratePlan.RatePlanItems);

            int recordsAffected = ExecuteNonQuerySP("TOneWhS_Sales.sp_RatePlan_InsertOrUpdate", ratePlan.OwnerType, ratePlan.OwnerId, serializedRatePlanItems, ratePlan.Status);

            return recordsAffected > 0;
        }

        private RatePlan RatePlanMapper(IDataReader reader)
        {
            RatePlan ratePlan = new RatePlan();

            ratePlan.RatePlanId = (int)reader["ID"];
            ratePlan.OwnerType = (RatePlanOwnerType)reader["OwnerType"];
            ratePlan.OwnerId = (int)reader["OwnerId"];
            ratePlan.RatePlanItems = Vanrise.Common.Serializer.Deserialize<List<RatePlanItem>>(reader["Details"] as string);
            ratePlan.Status = (RatePlanStatus)reader["Status"];

            return ratePlan;
        }
        */
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;
using Vanrise.Common;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class CustomerZoneDBSyncDataManager : BaseSQLDataManager, IDBSyncDataManager
    {
        string _TableName = Vanrise.Common.Utilities.GetEnumDescription(DBTableName.CustomerCountry);
        string _Schema = "TOneWhS_BE";
        bool _UseTempTables;
        int _SellingProductId;
        public CustomerZoneDBSyncDataManager(bool useTempTables, int sellingProductId) :
            base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

            _UseTempTables = useTempTables;
            _SellingProductId = sellingProductId;
        }

        public void ApplyCustomerZoneToTemp(List<CustomerCountry2> customerZones)
        {
            DataTable dt = new DataTable();
            dt.TableName = MigrationUtils.GetTableName(_Schema, _TableName, _UseTempTables);
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("CustomerID", typeof(int));
            dt.Columns.Add("CountryID", typeof(int));
            dt.Columns.Add("BED", typeof(DateTime));
            dt.Columns.Add(new DataColumn { AllowDBNull = true, ColumnName = "EED", DataType = typeof(DateTime) });
            dt.BeginLoadData();

            foreach (var item in customerZones)
            {
                DataRow row = dt.NewRow();
                row["ID"] = item.CustomerCountryId;
                row["CustomerID"] = item.CustomerId;
                row["CountryID"] = item.CountryId;
                row["BED"] = (DateTime)item.BED;
                row["EED"] = item.EED.HasValue ? item.EED : (object)DBNull.Value;
                dt.Rows.Add(row);
            }
            dt.EndLoadData();
            WriteDataTableToDB(dt, System.Data.SqlClient.SqlBulkCopyOptions.KeepNulls);
        }


        public void ApplyCustomerSellingProductToTemp(List<CustomerCountry2> customerZones, List<CarrierAccount> carrierAccounts)
        {
            DataTable customerSellingProductDT = new DataTable();
            customerSellingProductDT.TableName = MigrationUtils.GetTableName(_Schema, "CustomerSellingProduct", _UseTempTables);
            customerSellingProductDT.Columns.Add("CustomerID", typeof(int));
            customerSellingProductDT.Columns.Add("SellingProductID", typeof(int));
            customerSellingProductDT.Columns.Add("BED", typeof(DateTime));

            IEnumerable<CarrierAccount> customers = carrierAccounts.FindAllRecords(item => item.AccountType != CarrierAccountType.Supplier);
            customerSellingProductDT.BeginLoadData();

            foreach (var item in customers)
            {
                CustomerCountry2 customerZone = customerZones.FindRecord(itm => itm.CustomerId == item.CarrierAccountId);
                DataRow customerSellingProductDR = customerSellingProductDT.NewRow();
                customerSellingProductDR["CustomerID"] = item.CarrierAccountId;
                customerSellingProductDR["SellingProductID"] = _SellingProductId;
                customerSellingProductDR["BED"] = customerZone != null ? (DateTime)customerZone.BED : DateTime.Now.Date;
                customerSellingProductDT.Rows.Add(customerSellingProductDR);
            }
            customerSellingProductDT.EndLoadData();
            WriteDataTableToDB(customerSellingProductDT, System.Data.SqlClient.SqlBulkCopyOptions.KeepNulls);
        }


        public Dictionary<int, SourceCustomerZone> GetCustomerZones(bool useTempTables)
        {
            Dictionary<int, SourceCustomerZone> customerZones = new Dictionary<int, SourceCustomerZone>();
            SourceCustomerZone customerZone;
            ExecuteReaderText("SELECT DISTINCT SP.OwnerID, SZ.CountryID, SP.OwnerType, MIN(SR.BED) AS BED , MAX(SR.eed) as EED, SUM(Case when SR.EED is null then 1 else 0 end) as HasNullEED FROM "
                      + MigrationUtils.GetTableName(_Schema, "SalePriceList", useTempTables) + " AS SP INNER JOIN"
                      + MigrationUtils.GetTableName(_Schema, "SaleRate", useTempTables) + " AS SR ON SP.ID = SR.PriceListID INNER JOIN "
                      + MigrationUtils.GetTableName(_Schema, "SaleZone", useTempTables) + " AS SZ ON SR.ZoneID = SZ.ID"
                      + " WHERE (SP.OwnerType = 1) GROUP BY SP.OwnerID, SZ.CountryID, SP.OwnerType ORDER BY SP.OwnerID", (reader) =>
                      {
                          while (reader.Read())
                          {
                              int ownerId = (int)reader["OwnerID"];
                              DateTime bed = (DateTime)reader["BED"];
                              DateTime? eed = GetReaderValue<DateTime?>(reader, "EED");
                              int hasNullEED = (int)reader["HasNullEED"];

                              DateTime? modifiedEED = hasNullEED == 0 ? null : eed;

                              if (!customerZones.TryGetValue(ownerId, out customerZone))
                              {
                                  customerZones.Add(ownerId, new SourceCustomerZone()
                                  {
                                      Countries = new List<CustomerCountry>()
                                      { 
                                          new CustomerCountry(){ 
                                              CountryId = (int)reader["CountryID"], 
                                              StartEffectiveTime = bed, 
                                              EndEffectiveTime = modifiedEED
                                          }
                                      },
                                      CustomerId = ownerId,
                                      StartEffectiveTime = bed
                                  });
                              }
                              else
                              {
                                  customerZone.Countries.Add(new CustomerCountry() { CountryId = (int)reader["CountryID"], StartEffectiveTime = bed, EndEffectiveTime = modifiedEED });
                                  customerZone.StartEffectiveTime = Vanrise.Common.Utilities.Min(customerZone.StartEffectiveTime, bed);
                              }
                          }
                      }, null);

            return customerZones;
        }


        public string GetConnection()
        {
            return base.GetConnectionString();
        }

        public string GetTableName()
        {
            return _TableName;
        }

        public string GetSchema()
        {
            return _Schema;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class CustomerZoneDBSyncDataManager : BaseSQLDataManager, IDBSyncDataManager
    {
        string _TableName = Vanrise.Common.Utilities.GetEnumDescription(DBTableName.CustomerZone);
        string _Schema = "TOneWhS_BE";
        bool _UseTempTables;
        int _SellingProductId;
        public CustomerZoneDBSyncDataManager(bool useTempTables , int sellingProductId) :
            base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

            _UseTempTables = useTempTables;
            _SellingProductId = sellingProductId;
        }


        public void ApplyCustomerZoneToTemp(List<CustomerZones> customerZones)
        {
            DataTable dt = new DataTable();
            dt.TableName = MigrationUtils.GetTableName(_Schema, _TableName, _UseTempTables);
            dt.Columns.Add("CustomerID", typeof(int));
            dt.Columns.Add("Details", typeof(string));
            dt.Columns.Add("BED", typeof(DateTime));

            dt.BeginLoadData();


            DataTable customerSellingProductDT = new DataTable();
            customerSellingProductDT.TableName = MigrationUtils.GetTableName(_Schema, "CustomerSellingProduct", _UseTempTables);
            customerSellingProductDT.Columns.Add("CustomerID", typeof(int));
            customerSellingProductDT.Columns.Add("SellingProductID", typeof(int));
            customerSellingProductDT.Columns.Add("BED", typeof(DateTime));
            
            customerSellingProductDT.BeginLoadData();
            
            foreach (var item in customerZones)
            {
                DataRow row = dt.NewRow();
                string serializedSettings = item.Countries != null ? Vanrise.Common.Serializer.Serialize(item.Countries) : null;
                row["CustomerID"] = item.CustomerId;
                row["Details"] = serializedSettings;
                row["BED"] = (DateTime)item.StartEffectiveTime;
                dt.Rows.Add(row);

                DataRow customerSellingProductDR = customerSellingProductDT.NewRow();
                customerSellingProductDR["CustomerID"] = item.CustomerId;
                customerSellingProductDR["SellingProductID"] = _SellingProductId;
                customerSellingProductDR["BED"] = (DateTime)item.StartEffectiveTime;
                customerSellingProductDT.Rows.Add(customerSellingProductDR);
            }
            dt.EndLoadData();
            customerSellingProductDT.EndLoadData();
            WriteDataTableToDB(dt, System.Data.SqlClient.SqlBulkCopyOptions.KeepNulls);
            WriteDataTableToDB(customerSellingProductDT, System.Data.SqlClient.SqlBulkCopyOptions.KeepNulls);
        }

        public Dictionary<int, SourceCustomerZone> GetCustomerZones(bool useTempTables)
        {
            Dictionary<int, SourceCustomerZone> customerZones = new Dictionary<int, SourceCustomerZone>();
            SourceCustomerZone customerZone;
            ExecuteReaderText("SELECT DISTINCT SP.OwnerID, SZ.CountryID, SP.OwnerType, MIN(SR.BED) AS BED FROM "
                      + MigrationUtils.GetTableName(_Schema, "SalePriceList", useTempTables) + " AS SP INNER JOIN"
                      + MigrationUtils.GetTableName(_Schema, "SaleRate", useTempTables) + " AS SR ON SP.ID = SR.PriceListID INNER JOIN "
                      + MigrationUtils.GetTableName(_Schema, "SaleZone", useTempTables) + " AS SZ ON SR.ZoneID = SZ.ID"
                      +  " WHERE (SP.OwnerType = 1) GROUP BY SP.OwnerID, SZ.CountryID, SP.OwnerType ORDER BY SP.OwnerID", (reader) =>
                      {
                          while (reader.Read())
                          {
                              int ownerId = (int)reader["OwnerID"];
                              if (!customerZones.TryGetValue(ownerId, out customerZone))
                              {
                                  customerZones.Add(ownerId, new SourceCustomerZone()
                                  {
                                      Countries = new List<CustomerCountry>()
                                      { 
                                          new CustomerCountry(){ CountryId = (int)reader["CountryID"]}
                                      },
                                      CustomerId = ownerId,
                                      StartEffectiveTime = (DateTime)reader["BED"]
                                  });
                              }
                              else
                              {
                                  customerZones[ownerId].Countries.Add(new CustomerCountry() { CountryId = (int)reader["CountryID"] });
                                  customerZones[ownerId].StartEffectiveTime = Vanrise.Common.Utilities.Min(customerZones[ownerId].StartEffectiveTime, (DateTime)reader["BED"]);
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

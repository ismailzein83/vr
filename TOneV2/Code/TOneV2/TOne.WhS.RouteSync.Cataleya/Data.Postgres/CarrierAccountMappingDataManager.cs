using System;
using System.Collections.Generic;
using System.Data;
using TOne.WhS.RouteSync.Cataleya.Entities;
using Vanrise.Data.Postgres;
using System.Linq;

namespace TOne.WhS.RouteSync.Cataleya.Data.Postgres
{
    public class CarrierAccountMappingDataManager : BasePostgresDataManager
    {
        string schemaName;
        string connectionString;
        string tableName;
        string tempTableName;

        protected override string GetConnectionString()
        {
            return connectionString;
        }

        #region Public Methods

        public CarrierAccountMappingDataManager(string _schemaName, string _connectionString)
        {
            schemaName = _schemaName;
            connectionString = _connectionString;
            tableName = !string.IsNullOrEmpty(schemaName) ? string.Format(@"""{0}"".{1}", schemaName, "CarrierAccount") : "CarrierAccount";
            tempTableName = !string.IsNullOrEmpty(schemaName) ? string.Format(@"""{0}"".{1}", schemaName, "CarrierAccount_Temp") : "CarrierAccount_Temp";
        }

        public List<CarrierAccountMapping> GetCarrierAccountMappings(bool getFromTemp)
        {
            var table = getFromTemp ? tempTableName : tableName;
            var query = GetAllCarrierAccountMappings_Query.Replace("#TABLENAMEWITHSCHEMA#", table);

            return this.GetItemsText(query, CarrierAccountMappingMapper, null);
        }

        public void Initialize()
        {
            string[] queries = new string[2]
            {
                CreateCarrierAccountTable_Query.Replace("#TABLENAMEWITHSCHEMA#", tableName),
                DropIfExistsCreateTempCarrierAccountTable_Query.Replace("#TABLENAMEWITHSCHEMA#", tempTableName)
            };

            ExecuteNonQuery(queries);
        }

        public void FillTempCarrierAccountMappingTable(IEnumerable<CarrierAccountMapping> carrierAccountsMapping)
        {
            if (carrierAccountsMapping == null || !carrierAccountsMapping.Any())
                return;

            var itemsToInsert = new List<String>();
            foreach (var carrierAccountMapping in carrierAccountsMapping)
            {
                itemsToInsert.Add($"({carrierAccountMapping.Version},'{carrierAccountMapping.CarrierId}','{carrierAccountMapping.ZoneID}','{carrierAccountMapping.RouteTableName}')");//,'{(int)carrierAccountMapping.Status}')");
            }

            var addCarrierAccountsMappingToTempTableQuery = AddCarrierAccountsMappingToTempTable_Query.Replace("#TABLENAMEWITHSCHEMA#", tempTableName);
            addCarrierAccountsMappingToTempTableQuery = addCarrierAccountsMappingToTempTableQuery.Replace("#ITEMS#", string.Join(" ,", itemsToInsert));

            ExecuteNonQuery(new string[] { addCarrierAccountsMappingToTempTableQuery });
        }

        public CarrierAccountMapping GetCarrierAccountMapping(int carrierID)
        {
            var query = GetCarrierAccount_Query.Replace("#TABLENAMEWITHSCHEMA#", tempTableName);
            query = query.Replace("#WHERECONDITION#", $"where CAID = {carrierID}");

            return this.GetItemText(query, CarrierAccountMappingMapper, null);
        }
        public string GetAddCarrierAccountMappingQuery(CarrierAccountMapping carrierAccountMappingToAdd)
        {
            var query = AddCarrierAccountMapping_Query.Replace("#TABLENAMEWITHSCHEMA#", tableName);

            query = query.Replace("#VERSION#", carrierAccountMappingToAdd.Version.ToString());
            query = query.Replace("#CAID#", carrierAccountMappingToAdd.CarrierId.ToString());
            query = query.Replace("#ZONEID#", carrierAccountMappingToAdd.ZoneID.ToString());
            query = query.Replace("#ROUTETABLENAME#", carrierAccountMappingToAdd.RouteTableName);
            //query = query.Replace("#STATUS#", ((int)carrierAccountMappingToAdd.Status).ToString());
            return query;
        }

        public string GetUpdateCarrierAccountMappingQuery(CarrierAccountMapping carrierAccountMappingToUpdate)
        {
            var query = UpdateCarrierAccountMapping_Query.Replace("#TABLENAMEWITHSCHEMA#", tableName);

            query = query.Replace("#VERSION#", carrierAccountMappingToUpdate.Version.ToString());
            query = query.Replace("#CAID#", carrierAccountMappingToUpdate.CarrierId.ToString());
            query = query.Replace("#ROUTETABLENAME#", carrierAccountMappingToUpdate.RouteTableName);
            //query = query.Replace("#STATUS#", ((int)carrierAccountMappingToUpdate.Status).ToString());

            return query;
        }

        public string GetDeleteCarrierAccountMappingQuery(CarrierAccountMapping carrierAccountMappingToDelete)
        {
            var query = DeleteCarrierAccountMapping_Query.Replace("#TABLENAMEWITHSCHEMA#", tableName);
            query = query.Replace("#CAID#", carrierAccountMappingToDelete.CarrierId.ToString());

            return query;
        }

        public string GetDropTempCarrierAccountMappingTableQuery()
        {
            return DropTempCarrierAccountMappingTable_Query.Replace("#TABLENAMEWITHSCHEMA#", tempTableName);
        }

        //public bool UpdateCarrierAccountMappingStatus(String customerId, CarrierAccountStatus status)
        //{
        //    var query = UpdateCarrierAccountMappingStatus_Query.Replace("#TABLENAMEWITHSCHEMA#", tableName);
        //    query = query.Replace("#CAID#", customerId);
        //    query = query.Replace("#STATUS#", ((int)status).ToString());

        //    return ExecuteNonQueryText(query, null) > 0;
        //}

        #endregion

        #region Mappers

        CarrierAccountMapping CarrierAccountMappingMapper(IDataReader reader)
        {
            return new CarrierAccountMapping
            {
                Version = (int)reader["Version"],
                CarrierId = (int)reader["CAID"],
                ZoneID = (int)reader["ZoneID"],
                RouteTableName = reader["RouteTableName"] as string
                //Status = (CarrierAccountStatus)reader["Status"]
            };
        }

        #endregion

        #region Queries 
        const string GetAllCarrierAccountMappings_Query = @"select Version, CAID , ZoneID, RouteTableName  from #TABLENAMEWITHSCHEMA#;";

        const string CreateCarrierAccountTable_Query = @"CREATE TABLE IF NOT EXISTS #TABLENAMEWITHSCHEMA#
                                                          (
                                                           CAID int,
                                                           ZoneID int,
                                                           RouteTableName character varying(30),
                                                           Version int);";

        const string DropIfExistsCreateTempCarrierAccountTable_Query = @"DROP TABLE IF EXISTS #TABLENAMEWITHSCHEMA#;
                                                           CREATE TABLE #TABLENAMEWITHSCHEMA#
                                                          (CAID int,
                                                           ZoneID int,
                                                           RouteTableName character varying(30),
                                                           Version int);";

        const string AddCarrierAccountsMappingToTempTable_Query = @"INSERT INTO #TABLENAMEWITHSCHEMA#(
	                                                                 Version, CAID,ZoneID,RouteTableName)
	                                                                 VALUES #ITEMS#;";

        const string GetCarrierAccount_Query = @"select Version, CAID , ZoneID, RouteTableName from #TABLENAMEWITHSCHEMA# #WHERECONDITION#;";

        const string AddCarrierAccountMapping_Query = @"INSERT INTO #TABLENAMEWITHSCHEMA# (Version, CAID,ZoneID,RouteTableName) VALUES ('#VERSION#','#CAID#','#ZONEID#','#ROUTETABLENAME#');";

        const string UpdateCarrierAccountMapping_Query = @"UPDATE #TABLENAMEWITHSCHEMA# set Version = '#VERSION#' , RouteTableName = '#ROUTETABLENAME#' where CAID = '#CAID#';";

        const string DeleteCarrierAccountMapping_Query = @"Delete from #TABLENAMEWITHSCHEMA# where CAID = '#CAID#';";

        const string DropTempCarrierAccountMappingTable_Query = @"Drop Table #TABLENAMEWITHSCHEMA#;";

        //const string UpdateCarrierAccountMappingStatus_Query = @"UPDATE #TABLENAMEWITHSCHEMA# set Status = '#STATUS#' where CAID = '#CAID#';";

        #endregion
    }
}
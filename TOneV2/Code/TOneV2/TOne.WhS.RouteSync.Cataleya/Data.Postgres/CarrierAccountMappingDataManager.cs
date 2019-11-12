using System;
using System.Collections.Generic;
using System.Data;
using TOne.WhS.RouteSync.Cataleya.Entities;
using Vanrise.Data.Postgres;

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

        public void Initialize(List<CarrierAccountMapping> carrierAccountsMapping)
        {
            CreateCarrierAccountTableIfNotExist();
            DropIfExistsCreateTempCarrierAccountTable();
            AddCarrierAccountsMappingToTempTable(carrierAccountsMapping);
        }

        void CreateCarrierAccountTableIfNotExist()
        {
            var createCarrierAccountTableIfNotExistQuery = CreateCarrierAccountTable_Query.Replace("#TABLENAMEWITHSCHEMA#", tableName);
            ExecuteNonQuery(new string[] { createCarrierAccountTableIfNotExistQuery });
        }

        void DropIfExistsCreateTempCarrierAccountTable()
        {
            var dropIfExistsCreateTempCarrierAccountTableQuery = DropIfExistsCreateTempCarrierAccountTable_Query.Replace("#TABLENAMEWITHSCHEMA#", tempTableName);
            ExecuteNonQuery(new string[] { dropIfExistsCreateTempCarrierAccountTableQuery });
        }

        void AddCarrierAccountsMappingToTempTable(List<CarrierAccountMapping> carrierAccountsMapping)
        {
            var itemsToInsert = new List<String>();
            foreach (var carrierAccountMapping in carrierAccountsMapping)
            {
                itemsToInsert.Add($"({carrierAccountMapping.Version},'{carrierAccountMapping.CarrierId}','{carrierAccountMapping.ZoneID}','{carrierAccountMapping.RouteTableName}')");
            }

            var addCarrierAccountsMappingToTempTableQuery = AddCarrierAccountsMappingToTempTable_Query.Replace("#TABLENAMEWITHSCHEMA#", tempTableName);
            addCarrierAccountsMappingToTempTableQuery = addCarrierAccountsMappingToTempTableQuery.Replace("#ITEMS#", string.Join(" ,", itemsToInsert));

            ExecuteNonQuery(new string[] { addCarrierAccountsMappingToTempTableQuery });
        }

        public CarrierAccountMapping GetCarrierAccountMapping(int carrierID)
        {
            var query = GetCarrierAccount_Query.Replace("#TABLENAMEWITHSCHEMA#", tableName);
            query.Replace("#WHERECONDITION#", $"where CAID = {carrierID}");

            return this.GetItemText(query, CarrierAccountMappingMapper, null);
        }
        public string GetAddCarrierAccountMappingQuery(CarrierAccountMapping carrierAccountMappingToAdd)
        {
            var query = AddCarrierAccountMapping_Query.Replace("#TABLENAMEWITHSCHEMA#", tableName);

            query = query.Replace("#VERSION#", carrierAccountMappingToAdd.Version.ToString());
            query = query.Replace("#CAID#", carrierAccountMappingToAdd.CarrierId.ToString());
            query = query.Replace("#ZONEID#", carrierAccountMappingToAdd.ZoneID.ToString());
            query = query.Replace("#ROUTETABLENAME#", carrierAccountMappingToAdd.RouteTableName);

            return query;
        }

        public string GetUpdateCarrierAccountMappingQuery(CarrierAccountMapping carrierAccountMappingToUpdate)
        {
            var query = UpdateCarrierAccountMapping_Query.Replace("#TABLENAMEWITHSCHEMA#", tableName);

            query = query.Replace("#VERSION#", carrierAccountMappingToUpdate.Version.ToString());
            query = query.Replace("#CAID#", carrierAccountMappingToUpdate.CarrierId.ToString());
            query = query.Replace("#ROUTETABLENAME#", carrierAccountMappingToUpdate.RouteTableName);

            return query;
        }

        public string GetDropTempCarrierAccountMappingTableQuery()
        {
            return DropTempCarrierAccountMappingTable_Query.Replace("#TABLENAMEWITHSCHEMA#", tempTableName);
        }

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
            };
        }

        #endregion

        #region Queries 
        const string GetAllCarrierAccountMappings_Query = @"select Version, CAID , ZoneID, RouteTableName  from #TABLENAMEWITHSCHEMA#;";

        const string CreateCarrierAccountTable_Query = @"CREATE TABLE IF NOT EXISTS #TABLENAMEWITHSCHEMA#
                                                          (Version int,
                                                           CAID int,
                                                           ZoneID int,
                                                           RouteTableName character varying(30));";

        const string DropIfExistsCreateTempCarrierAccountTable_Query = @"DROP TABLE IF EXISTS #TABLENAMEWITHSCHEMA#;
                                                           CREATE TABLE #TABLENAMEWITHSCHEMA#
                                                          (Version int,
                                                           CAID int,
                                                           ZoneID int,
                                                           RouteTableName character varying(30));";

        const string AddCarrierAccountsMappingToTempTable_Query = @"INSERT INTO #TABLENAMEWITHSCHEMA#(
	                                                                 Version, CAID,ZoneID,RouteTableName)
	                                                                 VALUES #ITEMS#;";

        const string GetCarrierAccountsPreviousVersionNumbers_Query = @"select Version, CAID from #TABLENAMEWITHSCHEMA#
                                                                        #WHERECONDITION#;";

        const string GetCarrierAccount_Query = @"select Version, CAID , ZoneID, RouteTableName from #TABLENAMEWITHSCHEMA#
                                                                        #WHERECONDITION#;";

        const string AddCarrierAccountMapping_Query = @"INSERT INTO #TABLENAMEWITHSCHEMA# (Version, CAID,ZoneID,RouteTableName) VALUES ('#VERSION#','#CAID#','#ZONEID#','#ROUTETABLENAME#');";

        const string UpdateCarrierAccountMapping_Query = @"UPDATE #TABLENAMEWITHSCHEMA# set Version = '#VERSION#' , RouteTableName = '#ROUTETABLENAME#' where CAID = '#CAID#';";

        const string DropTempCarrierAccountMappingTable_Query = @"Drop Table #TABLENAMEWITHSCHEMA#;";

        #endregion
    }
}
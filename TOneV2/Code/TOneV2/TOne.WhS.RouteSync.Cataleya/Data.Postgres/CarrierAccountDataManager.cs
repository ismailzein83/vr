using System;
using System.Collections.Generic;
using System.Data;
using TOne.WhS.RouteSync.Cataleya.Entities;
using Vanrise.Data.Postgres;

namespace TOne.WhS.RouteSync.Cataleya.Data.Postgres
{
    public class CarrierAccountDataManager : BasePostgresDataManager
    {
        public string SchemaName { get; set; }
        public string ConnectionString { get; set; }
        string TableName { get { return "CarrierAccount"; } }
        string TempTableName { get { return "CarrierAccount_Temp"; } }

        protected override string GetConnectionString()
        {
            return ConnectionString;
        }

        #region Public Methods

        public CarrierAccountDataManager(string schemaName, string connectionString)
        {
            SchemaName = schemaName;
            ConnectionString = connectionString;
        }

        public List<CarrierAccountVersionInfo> GetCarrierAccountsPreviousVersionNumbers(List<int> carrierAccountIds)
        {
            if (carrierAccountIds == null || carrierAccountIds.Count == 0)
                return null;

            var query = GetCarrierAccountsPreviousVersionNumbers_Query.Replace("#TABLENAMEWITHSCHEMA#", $"{SchemaName}.{TableName}");
            query.Replace("#WHERECONDITION#", $"where CAID in ({string.Join(",", carrierAccountIds)})");

            return this.GetItemsText(query, CarrierAccountVersionInfoMapper, null);
        }

        public List<CarrierAccountMapping> GetAllCarrierAccountsMapping(bool getFromTemp)
        {
            var TableNameWithSchema = getFromTemp ? $"{SchemaName}.{TempTableName}" : $"{SchemaName}.{TableName}";
            var query = GetAllCarrierAccountMappings_Query.Replace("#TABLENAMEWITHSCHEMA#", TableNameWithSchema);

            return this.GetItemsText(query, CarrierAccountMappingMapper, null);
        }

        public void CreateCarrierAccountTableIfNotExist()
        {
            var createCarrierAccountTableIfNotExistQuery = CreateCarrierAccountTable_Query.Replace("#TABLENAMEWITHSCHEMA#", $"{SchemaName}.{TableName}");

            ExecuteNonQuery(new string[] { createCarrierAccountTableIfNotExistQuery });
        }

        public CarrierAccountMapping GetCarrierAccountMapping(int carrierID)
        {
            var query = GetCarrierAccount_Query.Replace("#TABLENAMEWITHSCHEMA#", $"{SchemaName}.{TableName}");
            query.Replace("#WHERECONDITION#", $"where CAID = {carrierID}");

            return this.GetItemText(query, CarrierAccountMappingMapper, null);
        }

        public void DropIfExistsCreateTempCarrierAccountTable()
        {
            var dropIfExistsCreateTempCarrierAccountTableQuery = DropIfExistsCreateTempCarrierAccountTable_Query.Replace("#TABLENAMEWITHSCHEMA#", $"{SchemaName}.{TempTableName}");

            ExecuteNonQuery(new string[] { dropIfExistsCreateTempCarrierAccountTableQuery });
        }

        public void AddCarrierAccountsMappingToTempTable(List<CarrierAccountMapping> carrierAccountsMapping)
        {
            var itemsToInsert = new List<String>();
            foreach (var carrierAccountMapping in carrierAccountsMapping)
            {
                itemsToInsert.Add($"({carrierAccountMapping.Version},'{carrierAccountMapping.CarrierId}','{carrierAccountMapping.ZoneID}','{carrierAccountMapping.RouteTableName}')");
            }

            var addCarrierAccountsMappingToTempTableQuery = AddCarrierAccountsMappingToTempTable_Query.Replace("#TABLENAMEWITHSCHEMA#", $"{SchemaName}.{TempTableName}");
            addCarrierAccountsMappingToTempTableQuery = addCarrierAccountsMappingToTempTableQuery.Replace("#ITEMS#", string.Join(" ,", itemsToInsert));

            ExecuteNonQuery(new string[] { addCarrierAccountsMappingToTempTableQuery });
        }

        public string GetAddCarrierAccountMappingQuery(CarrierAccountMapping carrierAccountMappingToAdd)
        {
            var query = AddCarrierAccountMapping_Query.Replace("#TABLENAMEWITHSCHEMA#", $"{SchemaName}.{TableName}");

            query = query.Replace("#VERSION#", carrierAccountMappingToAdd.Version.ToString());
            query = query.Replace("#CAID#", carrierAccountMappingToAdd.CarrierId.ToString());
            query = query.Replace("#ZONEID#", carrierAccountMappingToAdd.ZoneID.ToString());
            query = query.Replace("#ROUTETABLENAME#", carrierAccountMappingToAdd.RouteTableName);

            return query;
        }

        public string GetUpdateCarrierAccountMappingQuery(CarrierAccountMapping carrierAccountMappingToUpdate)
        {
            var query = UpdateCarrierAccountMapping_Query.Replace("#TABLENAMEWITHSCHEMA#", $"{SchemaName}.{TableName}");

            query = query.Replace("#VERSION#", carrierAccountMappingToUpdate.Version.ToString());
            query = query.Replace("#CAID#", carrierAccountMappingToUpdate.CarrierId.ToString());
            query = query.Replace("#ROUTETABLENAME#", carrierAccountMappingToUpdate.RouteTableName);

            return query;
        }

        public string GetDropTempCarrierAccountMappingTableQuery()
        {
            return DropTempCarrierAccountMappingTable_Query.Replace("#TABLENAMEWITHSCHEMA#", $"{SchemaName}.{TempTableName}");
        }

        #endregion

        #region Mappers

        CarrierAccountVersionInfo CarrierAccountVersionInfoMapper(IDataReader reader)
        {
            return new CarrierAccountVersionInfo
            {
                Version = (int)reader["Version"],
                CarrierId = (int)reader["CAID"]
            };
        }

        CarrierAccountMapping CarrierAccountMappingMapper(IDataReader reader)
        {
            return new CarrierAccountMapping
            {
                Version = (int)reader["Version"],
                CarrierId = (int)reader["CAID"],
                ZoneID = (int)reader["ZoneID"],
                RouteTableName = reader["RouteTableName"] as string,
            };
        }

        #endregion

        #region Queries 

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

        const string GetAllCarrierAccountMappings_Query = @"select Version, CAID , ZoneID, RouteTableName  from #TABLENAMEWITHSCHEMA#;";

        const string AddCarrierAccountMapping_Query = @"INSERT INTO #TABLENAMEWITHSCHEMA# (Version, CAID,ZoneID,RouteTableName) VALUES ('#VERSION#','#CAID#','#ZONEID#','#ROUTETABLENAME#');";

        const string UpdateCarrierAccountMapping_Query = @"UPDATE #TABLENAMEWITHSCHEMA# set Version = '#VERSION#' , RouteTableName = '#ROUTETABLENAME#' where CAID = '#CAID#';";

        const string DropTempCarrierAccountMappingTable_Query = @"Drop Table #TABLENAMEWITHSCHEMA#;";

        #endregion
    }
}
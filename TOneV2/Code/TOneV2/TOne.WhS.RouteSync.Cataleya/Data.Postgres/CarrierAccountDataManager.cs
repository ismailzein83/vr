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

        public CarrierAccountDataManager(string schemaName, string connectionString)
        {
            SchemaName = schemaName;
            ConnectionString = connectionString;
        }

        public List<CarrierAccountVersionInfo> GetCarrierAccountsPreviousVersionNumbers(List<int> carrierAccountIds)
        {
            return this.GetItemsText("select name from sys.schemas ", CarrierAccountVersionInfoMapper, null);

            throw new NotImplementedException();
        }



        public void CreateCarrierAccountTableIfNotExist()
        {
            var createCarrierAccountTableIfNotExistQuery = CreateCarrierAccountTable_Query.Replace("#TABLENAMEWITHSCHEMA#", $"{SchemaName}.{TableName}");

            ExecuteNonQuery(new string[] { createCarrierAccountTableIfNotExistQuery });
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

        #region Mappers

        CarrierAccountVersionInfo CarrierAccountVersionInfoMapper(IDataReader reader)
        {
            return new CarrierAccountVersionInfo
            {
                Version = GetReaderValue<Int32>(reader, "Version"),
                CarrierAccountId = GetReaderValue<Int32>(reader, "CAID")
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

        #endregion

    }
}
using System;
using System.Collections.Generic;
using TOne.WhS.RouteSync.Cataleya.Entities;
using Vanrise.Data.Postgres;

namespace TOne.WhS.RouteSync.Cataleya.Data.Postgres
{
    public class CustomerIdentificationDataManager : BasePostgresDataManager
    {
        public string SchemaName { get; set; }
        public string ConnectionString { get; set; }
        string TableName { get { return "CustomerIdentification"; } }
        string TempTableName { get { return "CustomerIdentification_Temp"; } }
        protected override string GetConnectionString()
        {
            return ConnectionString;
        }

        public CustomerIdentificationDataManager(string schemaName, string connectionString)
        {
            SchemaName = schemaName;
            ConnectionString = connectionString;
        }
        public void CreateCustomerIdentificationTableIfNotExist()
        {
            var createCustomerIdentificationTableIfNotExistQuery = CreateCustomerIdentificationTable_Query.Replace("#TABLENAMEWITHSCHEMA#", $"{SchemaName}.{TableName}");

            ExecuteNonQuery(new string[] { createCustomerIdentificationTableIfNotExistQuery });
        }

        public void DropIfExistsCreateTempCustomerIdentificationTable()
        {
            var dropIfExistsCreateTempCustomerIdentificationTableQuery = DropIfExistsCreateTempCustomerIdentificationTable_Query.Replace("#TABLENAMEWITHSCHEMA#", $"{SchemaName}.{TempTableName}");

            ExecuteNonQuery(new string[] { dropIfExistsCreateTempCustomerIdentificationTableQuery });
        }

        public void AddCustomerIdentificationsToTempTable(List<CustomerIdentification> customersIdentification)
        {
            var itemsToInsert = new List<String>();
            foreach (var customerIdentification in customersIdentification)
            {
                itemsToInsert.Add($"({customerIdentification.CarrierId},'{customerIdentification.Trunk}')");
            }

            var addCustomerIdentificationsToTempTableQuery = AddCustomerIdentificationsToTempTable_Query.Replace("#TABLENAMEWITHSCHEMA#", $"{SchemaName}.{TempTableName}");
            addCustomerIdentificationsToTempTableQuery = addCustomerIdentificationsToTempTableQuery.Replace("#ITEMS#", string.Join(" ,", itemsToInsert));

            ExecuteNonQuery(new string[] { addCustomerIdentificationsToTempTableQuery });
        }

        #region Queries 

        const string CreateCustomerIdentificationTable_Query = @"CREATE TABLE IF NOT EXISTS #TABLENAMEWITHSCHEMA#
                                                          (CAID int,
                                                           TRUNK character varying(30));";

        const string DropIfExistsCreateTempCustomerIdentificationTable_Query = @"DROP TABLE IF EXISTS #TABLENAMEWITHSCHEMA# ;
                                                           CREATE TABLE #TABLENAMEWITHSCHEMA#
                                                          (CAID bigint,
                                                           TRUNK character varying(30));";

        const string AddCustomerIdentificationsToTempTable_Query = @"INSERT INTO #TABLENAMEWITHSCHEMA#(
	                                                                 caid, trunk)
	                                                                 VALUES #ITEMS#;";
        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Data;
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

        #region Public Methods

        public CustomerIdentificationDataManager(string schemaName, string connectionString)
        {
            SchemaName = schemaName;
            ConnectionString = connectionString;
        }

        public List<CustomerIdentification> GetAllCustomerIdentifications(bool getFromTemp)
        {
            var TableNameWithSchema = getFromTemp ? $"{SchemaName}.{TempTableName}" : $"{SchemaName}.{TableName}";
            var query = GetAllCustomerIdentifications_Query.Replace("#TABLENAMEWITHSCHEMA#", TableNameWithSchema);

            return this.GetItemsText(query, CustomerIdentificationMapper, null);
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

        public string GeAddCustomerIdentificationQuery(CustomerIdentification customerIdentificationToAdd)
        {
            var query = AddCustomerIdentification_Query.Replace("#TABLENAMEWITHSCHEMA#", $"{SchemaName}.{TableName}");

            query = query.Replace("#CAID#", customerIdentificationToAdd.CarrierId.ToString());
            query = query.Replace("#TRUNK#", customerIdentificationToAdd.Trunk);
            return query;
        }

        public string GetDeleteCustomerIdentificationQuery(CustomerIdentification customerIdentificationToDelete)
        {
            var query = DeleteCustomerIdentification_Query.Replace("#TABLENAMEWITHSCHEMA#", $"{SchemaName}.{TableName}");

            query = query.Replace("#CAID#", customerIdentificationToDelete.CarrierId.ToString());
            query = query.Replace("#TRUNK#", customerIdentificationToDelete.Trunk);
            return query;
        }

        public string GetDropTempCustomerIdentificationTableQuery()
        {
            return DropTempCustomerIdentificationTable_Query.Replace("#TABLENAMEWITHSCHEMA#", $"{SchemaName}.{TempTableName}");
        }

        #endregion

        #region Mappers

        CustomerIdentification CustomerIdentificationMapper(IDataReader reader)
        {
            return new CustomerIdentification
            {
                CarrierId = (int)reader["CAID"],
                Trunk = reader["TRUNK"] as string
            };
        }

        #endregion

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

        const string GetAllCustomerIdentifications_Query = @"select  CAID , Trunk  from #TABLENAMEWITHSCHEMA#;";

        const string AddCustomerIdentification_Query = @"INSERT INTO #TABLENAMEWITHSCHEMA# (CAID,Trunk) VALUES ('#CAID#','#TRUNK#');";

        const string DeleteCustomerIdentification_Query = @"Delete From #TABLENAMEWITHSCHEMA# where CAID = '#CAID#' and Trunk = '#TRUNK#';";

        const string DropTempCustomerIdentificationTable_Query = @"Drop Table #TABLENAMEWITHSCHEMA#;";
        #endregion
    }
}
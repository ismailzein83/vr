using System;
using System.Collections.Generic;
using System.Data;
using TOne.WhS.RouteSync.Cataleya.Entities;
using Vanrise.Data.Postgres;

namespace TOne.WhS.RouteSync.Cataleya.Data.Postgres
{
    public class CustomerIdentificationDataManager : BasePostgresDataManager
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

        public CustomerIdentificationDataManager(string _schemaName, string _connectionString)
        {
            schemaName = _schemaName;
            connectionString = _connectionString;
            tableName = !string.IsNullOrEmpty(schemaName) ? string.Format(@"""{0}"".{1}", schemaName, "CustomerIdentification") : "CustomerIdentification";
            tempTableName = !string.IsNullOrEmpty(schemaName) ? string.Format(@"""{0}"".{1}", schemaName, "CustomerIdentification_Temp") : "CustomerIdentification_Temp";
        }

        public void Initialize(List<CustomerIdentification> customersIdentification)
        {
            string[] queries = new string[3]
            {
                CreateCustomerIdentificationTableIfNotExist(),
                DropIfExistsCreateTempCustomerIdentificationTable(),
                AddCustomerIdentificationsToTempTable(customersIdentification)
            };

            ExecuteNonQuery(queries);
        }

        string CreateCustomerIdentificationTableIfNotExist()
        {
            return CreateCustomerIdentificationTable_Query.Replace("#TABLENAMEWITHSCHEMA#", tableName);
        }

        string DropIfExistsCreateTempCustomerIdentificationTable()
        {
            return DropIfExistsCreateTempCustomerIdentificationTable_Query.Replace("#TABLENAMEWITHSCHEMA#", tempTableName);
        }

        string AddCustomerIdentificationsToTempTable(List<CustomerIdentification> customersIdentification)
        {
            var itemsToInsert = new List<String>();
            foreach (var customerIdentification in customersIdentification)
            {
                itemsToInsert.Add($"({customerIdentification.CarrierId},'{customerIdentification.Trunk}')");
            }

            var addCustomerIdentificationsToTempTableQuery = AddCustomerIdentificationsToTempTable_Query.Replace("#TABLENAMEWITHSCHEMA#", tempTableName);
            addCustomerIdentificationsToTempTableQuery = addCustomerIdentificationsToTempTableQuery.Replace("#ITEMS#", string.Join(" ,", itemsToInsert));

            return addCustomerIdentificationsToTempTableQuery;
        }

        public List<CustomerIdentification> GetAllCustomerIdentifications(bool getFromTemp)
        {
            var table = getFromTemp ? tempTableName : tableName;
            var query = GetAllCustomerIdentifications_Query.Replace("#TABLENAMEWITHSCHEMA#", table);

            return this.GetItemsText(query, CustomerIdentificationMapper, null);
        }

        public string GeAddCustomerIdentificationQuery(CustomerIdentification customerIdentificationToAdd)
        {
            var query = AddCustomerIdentification_Query.Replace("#TABLENAMEWITHSCHEMA#", tableName);

            query = query.Replace("#CAID#", customerIdentificationToAdd.CarrierId.ToString());
            query = query.Replace("#TRUNK#", customerIdentificationToAdd.Trunk);
            return query;
        }

        public string GetDeleteCustomerIdentificationQuery(CustomerIdentification customerIdentificationToDelete)
        {
            var query = DeleteCustomerIdentification_Query.Replace("#TABLENAMEWITHSCHEMA#", tableName);

            query = query.Replace("#CAID#", customerIdentificationToDelete.CarrierId.ToString());
            query = query.Replace("#TRUNK#", customerIdentificationToDelete.Trunk);
            return query;
        }

        public string GetDropTempCustomerIdentificationTableQuery()
        {
            return DropTempCustomerIdentificationTable_Query.Replace("#TABLENAMEWITHSCHEMA#", tempTableName);
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
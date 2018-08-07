using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.Data.RDB
{
    public class InvoiceSettingDataManager : IInvoiceSettingDataManager
    {
        static string TABLE_NAME = "VR_Invoice_InvoiceSetting";

        static InvoiceSettingDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add("ID", new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add("InvoiceTypeID", new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add("Name", new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });
            columns.Add("IsDefault", new RDBTableColumnDefinition { DataType = RDBDataType.Boolean });
            columns.Add("Details", new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add("IsDeleted", new RDBTableColumnDefinition { DataType = RDBDataType.Boolean });
            columns.Add("CreatedTime", new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "VR_Invoice",
                DBTableName = "InvoiceSetting",
                Columns = columns,
                IdColumnName = "ID",
                CreatedTimeColumnName = "CreatedTime"
            });
        }

        #region Private Methods

        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("VR_Invoice", "InvoiceDBConnStringKey", "InvoiceDBConnString");
        }

        public InvoiceSetting InvoiceSettingMapper(IRDBDataReader reader)
        {
            InvoiceSetting invoiceSetting = new InvoiceSetting
            {
                InvoiceSettingId = reader.GetGuid("ID"),
                Name = reader.GetString("Name"),
                InvoiceTypeId = reader.GetGuid("InvoiceTypeID"),
                Details = Vanrise.Common.Serializer.Deserialize<InvoiceSettingDetails>(reader.GetString("Details")),
                IsDefault = reader.GetBooleanWithNullHandling("IsDefault")
            };
            return invoiceSetting;
        }

        #endregion

        #region IInvoiceSettingDataManager

        public List<InvoiceSetting> GetInvoiceSettings()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "invSett", null, true);
            selectQuery.SelectColumns().AllTableColumns("invSett");

            selectQuery.Where().ConditionIfColumnNotNull("IsDeleted").EqualsCondition("IsDeleted").Value(false);

            selectQuery.Sort().ByColumn("Name", RDBSortDirection.ASC);

            return queryContext.GetItems(InvoiceSettingMapper);
        }

        public bool AreInvoiceSettingsUpdated(ref object updateHandle)
        {
            throw new NotImplementedException();
        }

        public bool InsertInvoiceSetting(Entities.InvoiceSetting invoiceSetting)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);

            var notExistsCondition = insertQuery.IfNotExists("invSett");
            notExistsCondition.EqualsCondition("InvoiceTypeID").Value(invoiceSetting.InvoiceTypeId);
            notExistsCondition.EqualsCondition("Name").Value(invoiceSetting.Name);
            notExistsCondition.ConditionIfColumnNotNull("IsDeleted").EqualsCondition("IsDeleted").Value(false);

            insertQuery.Column("ID").Value(invoiceSetting.InvoiceSettingId);
            insertQuery.Column("InvoiceTypeID").Value(invoiceSetting.InvoiceTypeId);
            insertQuery.Column("Name").Value(invoiceSetting.Name);
            insertQuery.Column("IsDefault").Value(invoiceSetting.IsDefault);
            if (invoiceSetting.Details != null)
                insertQuery.Column("Details").Value(Vanrise.Common.Serializer.Serialize(invoiceSetting.Details));
            
            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool UpdateInvoiceSetting(Entities.InvoiceSetting invoiceSetting)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            var notExistsCondition = updateQuery.IfNotExists("invSett");
            notExistsCondition.NotEqualsCondition("ID").Value(invoiceSetting.InvoiceSettingId);
            notExistsCondition.EqualsCondition("InvoiceTypeID").Value(invoiceSetting.InvoiceTypeId);
            notExistsCondition.EqualsCondition("Name").Value(invoiceSetting.Name);
            notExistsCondition.ConditionIfColumnNotNull("IsDeleted").EqualsCondition("IsDeleted").Value(false);

            updateQuery.Column("InvoiceTypeID").Value(invoiceSetting.InvoiceTypeId);
            updateQuery.Column("Name").Value(invoiceSetting.Name);
            updateQuery.Column("IsDefault").Value(invoiceSetting.IsDefault);
            updateQuery.Column("Details").Value(Vanrise.Common.Serializer.Serialize(invoiceSetting.Details));

            updateQuery.Where().EqualsCondition("ID").Value(invoiceSetting.InvoiceSettingId);

            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool SetInvoiceSettingDefault(Guid invoiceSettingId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            queryContext.DeclareParameters().AddParameter("InvoiceTypeID", RDBDataType.UniqueIdentifier);
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "invSett", 1);
            selectQuery.SelectColumns().ColumnToParameter("InvoiceTypeID", "InvoiceTypeID");

            selectQuery.Where().EqualsCondition("ID").Value(invoiceSettingId);
            
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            var caseContext = updateQuery.Column("IsDefault").CaseExpression();
            
            var caseWhenContext = caseContext.AddCase();
            caseWhenContext.When().EqualsCondition("ID").Value(invoiceSettingId);
            caseWhenContext.Then().Value(true);

            caseContext.Else().Value(false);

            var updateWhere = updateQuery.Where();
            updateWhere.EqualsCondition("InvoiceTypeID").Parameter("InvoiceTypeID");
            var orCondition = updateWhere.ChildConditionGroup(RDBConditionGroupOperator.OR);
            orCondition.EqualsCondition("ID").Value(invoiceSettingId);
            orCondition.EqualsCondition("IsDefault").Value(true);

            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool DeleteInvoiceSetting(Guid invoiceSettingId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            updateQuery.Column("IsDeleted").Value(1);

            updateQuery.Where().EqualsCondition("ID").Value(invoiceSettingId);

            return queryContext.ExecuteNonQuery() > 0;
        }

        #endregion
    }
}

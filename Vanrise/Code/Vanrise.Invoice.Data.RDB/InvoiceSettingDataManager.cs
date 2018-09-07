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

        const string COL_ID = "ID";
        const string COL_InvoiceTypeId = "InvoiceTypeId";
        const string COL_Name = "Name";
        const string COL_IsDefault = "IsDefault";
        const string COL_Details = "Details";
        const string COL_IsDeleted = "IsDeleted";
        const string COL_CreatedTime = "CreatedTime";

        static InvoiceSettingDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_InvoiceTypeId, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });
            columns.Add(COL_IsDefault, new RDBTableColumnDefinition { DataType = RDBDataType.Boolean });
            columns.Add(COL_Details, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_IsDeleted, new RDBTableColumnDefinition { DataType = RDBDataType.Boolean });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "VR_Invoice",
                DBTableName = "InvoiceSetting",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime
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
                InvoiceSettingId = reader.GetGuid(COL_ID),
                Name = reader.GetString(COL_Name),
                InvoiceTypeId = reader.GetGuid(COL_InvoiceTypeId),
                Details = Vanrise.Common.Serializer.Deserialize<InvoiceSettingDetails>(reader.GetString(COL_Details)),
                IsDefault = reader.GetBooleanWithNullHandling(COL_IsDefault)
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

            selectQuery.Where().ConditionIfColumnNotNull(COL_IsDeleted).EqualsCondition(COL_IsDeleted).Value(false);

            selectQuery.Sort().ByColumn(COL_Name, RDBSortDirection.ASC);

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
            notExistsCondition.EqualsCondition(COL_InvoiceTypeId).Value(invoiceSetting.InvoiceTypeId);
            notExistsCondition.EqualsCondition(COL_Name).Value(invoiceSetting.Name);
            notExistsCondition.ConditionIfColumnNotNull(COL_IsDeleted).EqualsCondition(COL_IsDeleted).Value(false);

            insertQuery.Column(COL_ID).Value(invoiceSetting.InvoiceSettingId);
            insertQuery.Column(COL_InvoiceTypeId).Value(invoiceSetting.InvoiceTypeId);
            insertQuery.Column(COL_Name).Value(invoiceSetting.Name);
            insertQuery.Column(COL_IsDefault).Value(invoiceSetting.IsDefault);
            if (invoiceSetting.Details != null)
                insertQuery.Column(COL_Details).Value(Vanrise.Common.Serializer.Serialize(invoiceSetting.Details));
            
            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool UpdateInvoiceSetting(Entities.InvoiceSetting invoiceSetting)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            var notExistsCondition = updateQuery.IfNotExists("invSett");
            notExistsCondition.NotEqualsCondition(COL_ID).Value(invoiceSetting.InvoiceSettingId);
            notExistsCondition.EqualsCondition(COL_InvoiceTypeId).Value(invoiceSetting.InvoiceTypeId);
            notExistsCondition.EqualsCondition(COL_Name).Value(invoiceSetting.Name);
            notExistsCondition.ConditionIfColumnNotNull(COL_IsDeleted).EqualsCondition(COL_IsDeleted).Value(false);

            updateQuery.Column(COL_InvoiceTypeId).Value(invoiceSetting.InvoiceTypeId);
            updateQuery.Column(COL_Name).Value(invoiceSetting.Name);
            updateQuery.Column(COL_IsDefault).Value(invoiceSetting.IsDefault);
            updateQuery.Column(COL_Details).Value(Vanrise.Common.Serializer.Serialize(invoiceSetting.Details));

            updateQuery.Where().EqualsCondition(COL_ID).Value(invoiceSetting.InvoiceSettingId);

            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool SetInvoiceSettingDefault(Guid invoiceSettingId)
        {
            Guid invoiceTypeId = GetInvoiceTypeId(invoiceSettingId);

            var queryContext = new RDBQueryContext(GetDataProvider());

            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            var caseContext = updateQuery.Column(COL_IsDefault).CaseExpression();
            
            var caseWhenContext = caseContext.AddCase();
            caseWhenContext.When().EqualsCondition(COL_ID).Value(invoiceSettingId);
            caseWhenContext.Then().Value(true);

            caseContext.Else().Value(false);

            var updateWhere = updateQuery.Where();
            updateWhere.EqualsCondition(COL_InvoiceTypeId).Value(invoiceTypeId);
            var orCondition = updateWhere.ChildConditionGroup(RDBConditionGroupOperator.OR);
            orCondition.EqualsCondition(COL_ID).Value(invoiceSettingId);
            orCondition.EqualsCondition(COL_IsDefault).Value(true);

            return queryContext.ExecuteNonQuery() > 0;
        }

        private Guid GetInvoiceTypeId(Guid invoiceSettingId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "invSett", 1);
            selectQuery.SelectColumns().Column(COL_InvoiceTypeId);

            selectQuery.Where().EqualsCondition(COL_ID).Value(invoiceSettingId);

            return queryContext.ExecuteScalar().GuidValue;
        }

        public bool DeleteInvoiceSetting(Guid invoiceSettingId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            updateQuery.Column(COL_IsDeleted).Value(1);

            updateQuery.Where().EqualsCondition(COL_ID).Value(invoiceSettingId);

            return queryContext.ExecuteNonQuery() > 0;
        }

        #endregion
    }
}

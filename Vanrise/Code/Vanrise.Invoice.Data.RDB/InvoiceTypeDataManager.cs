using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.Data.RDB
{
    public class InvoiceTypeDataManager : IInvoiceTypeDataManager
    {
        static string TABLE_NAME = "VR_Invoice_InvoiceType";

        static InvoiceTypeDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add("ID", new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add("Name", new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add("Settings", new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add("CreatedTime", new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "VR_Invoice",
                DBTableName = "InvoiceType",
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

        public InvoiceType InvoiceTypeMapper(IRDBDataReader reader)
        {
            InvoiceType invoiceType = new InvoiceType
            {
                InvoiceTypeId = reader.GetGuid("ID"),
                Name = reader.GetString("Name"),
                Settings = Vanrise.Common.Serializer.Deserialize<InvoiceTypeSettings>(reader.GetString("Settings"))
            };
            return invoiceType;
        }

        #endregion

        #region IInvoiceTypeDataManager

        public List<Entities.InvoiceType> GetInvoiceTypes()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "invType", null, true);
            selectQuery.SelectColumns().AllTableColumns("invType");
            selectQuery.Sort().ByColumn("Name", RDBSortDirection.ASC);

            return queryContext.GetItems(InvoiceTypeMapper);
        }

        public bool AreInvoiceTypesUpdated(ref object updateHandle)
        {
            throw new NotImplementedException();
        }

        public bool InsertInvoiceType(Entities.InvoiceType invoiceType)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.IfNotExists("invType").EqualsCondition("Name").Value(invoiceType.Name);

            insertQuery.Column("ID").Value(invoiceType.InvoiceTypeId);
            insertQuery.Column("Name").Value(invoiceType.Name);
            if (invoiceType.Settings != null)
                insertQuery.Column("Settings").Value(Vanrise.Common.Serializer.Serialize(invoiceType.Settings));

            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool UpdateInvoiceType(Entities.InvoiceType invoiceType)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            var notExistsCondition = updateQuery.IfNotExists("invType");
            notExistsCondition.NotEqualsCondition("ID").Value(invoiceType.InvoiceTypeId);
            notExistsCondition.EqualsCondition("Name").Value(invoiceType.Name);

            updateQuery.Column("Name").Value(invoiceType.Name);
            updateQuery.Column("Settings").Value(Vanrise.Common.Serializer.Serialize(invoiceType.Settings));

            updateQuery.Where().EqualsCondition("ID").Value(invoiceType.InvoiceTypeId);

            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool ApproveInvoice(long invoiceId, DateTime? ApprovedDate, int? ApprovedBy)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

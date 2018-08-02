using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.Data.RDB
{
    public class InvoiceItemDataManager : IInvoiceItemDataManager
    {
        static string TABLE_NAME = "VR_Invoice_InvoiceItem";

        static InvoiceItemDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add("ID", new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add("InvoiceID", new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add("ItemSetName", new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add("Name", new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 900 });
            columns.Add("Details", new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add("CreatedTime", new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "VR_Invoice",
                DBTableName = "InvoiceItem",
                Columns = columns,
                IdColumnName = "ID",
                CreatedTimeColumnName = "CreatedTime"
            });
        }

        string _storageConnectionStringKey;
        public string StorageConnectionStringKey
        {
            set { this._storageConnectionStringKey = value; }
        }

        #region Private Methods

        BaseRDBDataProvider GetDataProvider()
        {
            
            if (!String.IsNullOrWhiteSpace(this._storageConnectionStringKey))
            {
                var connectionStringKey = ConfigurationManager.AppSettings[this._storageConnectionStringKey];
                if (!string.IsNullOrEmpty(connectionStringKey))
                {
                    var connectionString = ConfigurationManager.ConnectionStrings[connectionStringKey];
                    if (connectionString == null)
                        throw new NullReferenceException(String.Format("connectionString '{0}'", connectionStringKey));
                    return RDBDataProviderFactory.CreateProvider("VR_Invoice", connectionString.ConnectionString);
                }
            }
            return RDBDataProviderFactory.CreateProvider("VR_Invoice", "InvoiceDBConnStringKey", "InvoiceDBConnString");
        }

        private RDBCompareConditionOperator MapCompareOperator(CompareOperator compareOperator)
        {
            RDBCompareConditionOperator rdbCompareOperator;
            switch (compareOperator)
            {
                case CompareOperator.Equal: rdbCompareOperator = RDBCompareConditionOperator.Eq; break;
                case CompareOperator.Contains: rdbCompareOperator = RDBCompareConditionOperator.Contains; break;
                case CompareOperator.StartWith: rdbCompareOperator = RDBCompareConditionOperator.StartWith; break;
                case CompareOperator.EndWith: rdbCompareOperator = RDBCompareConditionOperator.EndWith; break;
                default: throw new NotSupportedException(string.Format("compareOperator '{0}'", compareOperator.ToString()));
            }
            return rdbCompareOperator;
        }

        public Entities.InvoiceItem InvoiceItemMapper(IRDBDataReader reader)
        {
            Entities.InvoiceItem invoiceItem = new Entities.InvoiceItem
            {
                Details = Vanrise.Common.Serializer.Deserialize(reader.GetString("Details")),
                InvoiceId = reader.GetLong("InvoiceID"),
                ItemSetName = reader.GetString("ItemSetName"),
                Name = reader.GetString("Name"),
                InvoiceItemId = reader.GetLong("ID"),
            };
            return invoiceItem;
        }

        #endregion

        #region IInvoiceItemDataManager

        public IEnumerable<InvoiceItem> GetFilteredInvoiceItems(long invoiceId, string itemSetName, CompareOperator compareOperator)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "invItm");
            selectQuery.SelectColumns().AllTableColumns("invItm");

            var where = selectQuery.Where();
            where.EqualsCondition("InvoiceID").Value(invoiceId);
            where.CompareCondition("ItemSetName", MapCompareOperator(compareOperator)).Value(itemSetName);

            return queryContext.GetItems(InvoiceItemMapper);
        }

        public IEnumerable<InvoiceItem> GetInvoiceItemsByItemSetNames(List<long> invoiceIds, IEnumerable<string> itemSetNames, CompareOperator compareOperator)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var tempTableQuery = queryContext.CreateTempTable();
            tempTableQuery.AddColumn("ItemSetName", new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });

            if(itemSetNames != null)
            {
                foreach(var itemSetName in itemSetNames)
                {
                    var insertQuery = queryContext.AddInsertQuery();
                    insertQuery.IntoTable(tempTableQuery);
                    insertQuery.Column("ItemSetName").Value(itemSetName);
                }
            }

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "invItm");
            selectQuery.SelectColumns().AllTableColumns("invItm");

            var joinCondition = selectQuery.Join().Join(tempTableQuery, "setNames").On();
            joinCondition.CompareCondition("ItemSetName", MapCompareOperator(compareOperator)).Column("setNames", "ItemSetName");

            selectQuery.Where().ListCondition("InvoiceID", RDBListConditionOperator.IN, invoiceIds);

            return queryContext.GetItems(InvoiceItemMapper);
        }

        #endregion

        public void SaveInvoiceItems(long invoiceId, IEnumerable<GeneratedInvoiceItemSet> invoiceItemSets)
        {
            if (invoiceItemSets != null && invoiceItemSets.Count() > 0)
            {
                var queryContext = new RDBQueryContext(GetDataProvider());

                foreach (var itemSet in invoiceItemSets)
                {
                    if (itemSet.Items != null && itemSet.Items.Count > 0)
                    {
                        foreach (var item in itemSet.Items)
                        {
                            var insertQuery = queryContext.AddInsertQuery();
                            insertQuery.IntoTable(TABLE_NAME);
                            insertQuery.Column("InvoiceID").Value(invoiceId);
                            insertQuery.Column("ItemSetName").Value(itemSet.SetName);
                            insertQuery.Column("Name").Value(item.Name);
                            string serializedDetails = Vanrise.Common.Serializer.Serialize(item.Details);
                            insertQuery.Column("Details").Value(serializedDetails);
                        }
                    }
                }

                queryContext.ExecuteNonQuery();
            }
        }
    }
}

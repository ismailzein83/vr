using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;

namespace Vanrise.Invoice.Data.RDB
{
    public class InvoiceSequenceDataManager : IInvoiceSequenceDataManager
    {
        static string TABLE_NAME = "VR_Invoice_InvoiceSequence";

        static InvoiceSequenceDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add("SequenceGroup", new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 255 });
            columns.Add("InvoiceTypeID", new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add("SequenceKey", new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add("InitialValue", new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add("LastValue", new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add("CreatedTime", new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "VR_Invoice",
                DBTableName = "InvoiceSequence",
                Columns = columns,
                CreatedTimeColumnName = "CreatedTime"
            });
        }

        #region Private Methods

        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("VR_Invoice", "InvoiceDBConnStringKey", "InvoiceDBConnString");
        }

        #endregion

        #region IInvoiceSequenceDataManager

        public long GetNextSequenceValue(string sequenceGroup, Guid invoiceTypeId, string sequenceKey, long initialValue)
        {            
            long nextSequence;
            if(!TryUpdateAndGetNextSequence(sequenceGroup, invoiceTypeId, sequenceKey, out nextSequence))
            {
                if (!TryInsertSequenceRecord(sequenceGroup, invoiceTypeId, sequenceKey, initialValue, out nextSequence))
                    if (!TryUpdateAndGetNextSequence(sequenceGroup, invoiceTypeId, sequenceKey, out nextSequence))
                        throw new Exception(string.Format("Cannot update sequence for invoiceTypeId '{0}', SequenceKey '{1}', sequenceGroup '{2}', initialValue '{3}'", invoiceTypeId, sequenceKey, sequenceGroup, initialValue));
            }
            return nextSequence;
        }

        private bool TryUpdateAndGetNextSequence(string sequenceGroup, Guid invoiceTypeId, string sequenceKey, out long nextSequence)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            queryContext.DeclareParameters().AddParameter("NextSequence", RDBDataType.BigInt);

            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            var sumContext = updateQuery.Parameter("NextSequence").ArithmeticExpression(RDBArithmeticExpressionOperator.Add);
            sumContext.Expression1().Column("LastValue");
            sumContext.Expression2().Value(1);

            sumContext = updateQuery.Column("LastValue").ArithmeticExpression(RDBArithmeticExpressionOperator.Add);
            sumContext.Expression1().Column("LastValue");
            sumContext.Expression2().Value(1);

            var where = updateQuery.Where();
            where.EqualsCondition("InvoiceTypeID").Value(invoiceTypeId);
            where.EqualsCondition("SequenceKey").Value(sequenceKey);
            where.EqualsCondition("SequenceGroup").Value(sequenceGroup);


            queryContext.AddSelectQuery().SelectColumns().Expression("NextSequence").Parameter("NextSequence");

            long? nextSequenceNullable = queryContext.ExecuteScalar().NullableLongValue;
            if (nextSequenceNullable.HasValue)
            {
                nextSequence = nextSequenceNullable.Value;
                return true;
            }
            else
            {
                nextSequence = 0;
                return false;
            }
        }

        private bool TryInsertSequenceRecord(string sequenceGroup, Guid invoiceTypeId, string sequenceKey, long initialValue, out long nextSequence)
        {
            //check if exists sequence for same sequence group
            var queryContext = new RDBQueryContext(GetDataProvider());

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "seq", 1, true);
            selectQuery.SelectColumns().Column("InvoiceTypeID");

            var where = selectQuery.Where();
            where.EqualsCondition("InvoiceTypeID").Value(invoiceTypeId);
            where.EqualsCondition("SequenceGroup").Value(sequenceGroup);
            long effectiveInitialValue;
            if (queryContext.ExecuteScalar().NullableGuidValue.HasValue)
                effectiveInitialValue = 1;
            else
                effectiveInitialValue = initialValue;

            try
            {
                queryContext = new RDBQueryContext(GetDataProvider());
                var insertQuery = queryContext.AddInsertQuery();
                insertQuery.IntoTable(TABLE_NAME);

                insertQuery.Column("SequenceGroup").Value(sequenceGroup);
                insertQuery.Column("InvoiceTypeID").Value(invoiceTypeId);
                insertQuery.Column("SequenceKey").Value(sequenceKey);
                insertQuery.Column("InitialValue").Value(effectiveInitialValue);
                insertQuery.Column("LastValue").Value(effectiveInitialValue);

                queryContext.ExecuteNonQuery();
                nextSequence = effectiveInitialValue;
                return true;
            }
            catch//if insert conflicted with other call
            {
                nextSequence = 0;
                return false;
            }
        }

        #endregion
    }
}

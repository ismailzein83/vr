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

        const string COL_SequenceGroup = "SequenceGroup";
        const string COL_InvoiceTypeID = "InvoiceTypeID";
        const string COL_SequenceKey = "SequenceKey";
        const string COL_InitialValue = "InitialValue";
        const string COL_LastValue = "LastValue";
        const string COL_CreatedTime = "CreatedTime";

        static InvoiceSequenceDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_SequenceGroup, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 255 });
            columns.Add(COL_InvoiceTypeID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_SequenceKey, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_InitialValue, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_LastValue, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "VR_Invoice",
                DBTableName = "InvoiceSequence",
                Columns = columns,
                CreatedTimeColumnName = COL_CreatedTime
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

            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            var sumContext = updateQuery.Column(COL_LastValue).ArithmeticExpression(RDBArithmeticExpressionOperator.Add);
            sumContext.Expression1().Column(COL_LastValue);
            sumContext.Expression2().Value(1);

            var where = updateQuery.Where();
            where.EqualsCondition(COL_InvoiceTypeID).Value(invoiceTypeId);

            if (sequenceKey == string.Empty)
                where.EqualsCondition(COL_SequenceKey).EmptyString();
            else
                where.EqualsCondition(COL_SequenceKey).Value(sequenceKey);
            where.EqualsCondition(COL_SequenceGroup).Value(sequenceGroup);

            updateQuery.AddSelectColumn(COL_LastValue);

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
            selectQuery.SelectColumns().Column(COL_InvoiceTypeID);

            var where = selectQuery.Where();
            where.EqualsCondition(COL_InvoiceTypeID).Value(invoiceTypeId);
            where.EqualsCondition(COL_SequenceGroup).Value(sequenceGroup);
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

                insertQuery.Column(COL_SequenceGroup).Value(sequenceGroup);
                insertQuery.Column(COL_InvoiceTypeID).Value(invoiceTypeId);

                if (sequenceKey == string.Empty)
                    insertQuery.Column(COL_SequenceKey).EmptyString();
                else
                    insertQuery.Column(COL_SequenceKey).Value(sequenceKey);

                insertQuery.Column(COL_InitialValue).Value(effectiveInitialValue);
                insertQuery.Column(COL_LastValue).Value(effectiveInitialValue);

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

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vanrise.Invoice.Data;
using Vanrise.RDBTests.Common;

namespace Vanrise.RDB.Tests.Invoice
{
    [TestClass]
    public class InvoiceSequenceDataManagerTests
    {
        const string DBTABLE_NAME_INVOICESEQUENCE = "InvoiceSequence";

        IInvoiceSequenceDataManager _rdbDataManager = RDBDataManagerFactory.GetDataManager<IInvoiceSequenceDataManager>();
        IInvoiceSequenceDataManager _sqlDataManager = SQLDataManagerFactory.GetDataManager<IInvoiceSequenceDataManager>();

        [TestMethod]
        public void GetNextSequenceValue()
        {
            UTUtilities.TruncateTable(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_INVOICE, DBTABLE_NAME_INVOICESEQUENCE);
            GetNextSequenceValue("Group1", "Key1", 1);
            GetNextSequenceValue("Group1", "Key1", 5);
            GetNextSequenceValue("", "Key1", 1);
            GetNextSequenceValue("", "Key1", 5);
            GetNextSequenceValue("Group1", "", 1);
            GetNextSequenceValue("Group1", "", 5);
            GetNextSequenceValue("", "", 1);
            GetNextSequenceValue("", "", 5);
        }

        void GetNextSequenceValue(string sequenceGroup, string sequenceKey, int initialValue)
        {
            Guid invoiceTypeId = Guid.NewGuid();
            UTUtilities.AssertValuesAreEqual(_sqlDataManager.GetNextSequenceValue(sequenceGroup, invoiceTypeId, sequenceKey, initialValue), _rdbDataManager.GetNextSequenceValue(sequenceGroup, invoiceTypeId, sequenceKey, initialValue));
            UTUtilities.AssertDBTablesAreSimilar(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_INVOICE, DBTABLE_NAME_INVOICESEQUENCE);
            UTUtilities.AssertValuesAreEqual(_sqlDataManager.GetNextSequenceValue(sequenceGroup, invoiceTypeId, sequenceKey, initialValue), _rdbDataManager.GetNextSequenceValue(sequenceGroup, invoiceTypeId, sequenceKey, initialValue));
            UTUtilities.AssertDBTablesAreSimilar(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_INVOICE, DBTABLE_NAME_INVOICESEQUENCE);
            UTUtilities.AssertValuesAreEqual(_sqlDataManager.GetNextSequenceValue(sequenceGroup, invoiceTypeId, sequenceKey, initialValue), _rdbDataManager.GetNextSequenceValue(sequenceGroup, invoiceTypeId, sequenceKey, initialValue));
            UTUtilities.AssertValuesAreEqual(_sqlDataManager.GetNextSequenceValue(sequenceGroup, invoiceTypeId, sequenceKey, initialValue), _rdbDataManager.GetNextSequenceValue(sequenceGroup, invoiceTypeId, sequenceKey, initialValue));
            UTUtilities.AssertValuesAreEqual(_sqlDataManager.GetNextSequenceValue(sequenceGroup, invoiceTypeId, sequenceKey, initialValue), _rdbDataManager.GetNextSequenceValue(sequenceGroup, invoiceTypeId, sequenceKey, initialValue));
            UTUtilities.AssertValuesAreEqual(_sqlDataManager.GetNextSequenceValue(sequenceGroup, invoiceTypeId, sequenceKey, initialValue), _rdbDataManager.GetNextSequenceValue(sequenceGroup, invoiceTypeId, sequenceKey, initialValue));
            UTUtilities.AssertDBTablesAreSimilar(Constants.CONNSTRING_NAME_CONFIG, Constants.DBSCHEMA_NAME_INVOICE, DBTABLE_NAME_INVOICESEQUENCE);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Billing.Entities;
using TOne.Data.SQL;

namespace TOne.Billing.Data.SQL
{
    public class SupplierInvoiceDataManager : BaseTOneDataManager, ISupplierInvoiceDataManager
    {
        public Vanrise.Entities.BigResult<SupplierInvoice> GetFilteredSupplierInvoices(Vanrise.Entities.DataRetrievalInput<SupplierInvoiceQuery> input)
        {
            Dictionary<string, string> mapper = new Dictionary<string, string>();

            mapper.Add("InvoiceID", "InvoiceID");

            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQueryText(CreateTempTableIfNotExists(tempTableName), (cmd) =>
                {
                    cmd.Parameters.Add(new SqlParameter("@SelectedSupplierID", input.Query.selectedSupplierID));
                    cmd.Parameters.Add(new SqlParameter("@From", input.Query.from));
                    cmd.Parameters.Add(new SqlParameter("@to", input.Query.to));
                });
            };

            return RetrieveData(input, createTempTableAction, SupplierInvoiceMapper, mapper);
        }

        private string CreateTempTableIfNotExists(string tempTableName)
        {
            StringBuilder query = new StringBuilder();
            return query.ToString();
        }

        private SupplierInvoice SupplierInvoiceMapper(IDataReader reader)
        {
            SupplierInvoice supplierInvoice = new SupplierInvoice
            {
                InvoiceID = (int)reader["InvoiceID"]
            };

            return supplierInvoice;
        }
    }
}

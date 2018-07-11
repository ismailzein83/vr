using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace Demo.Module.Data.SQL
{
    public class PaymentDataManager : BaseSQLDataManager, IPaymentDataManager
    {

        #region Constructors
        public PaymentDataManager() :
            base(GetConnectionStringName("DemoProjectCallCenter_DBConnStringKey", "DemoProjectCallCenter_DBConnStringKey"))
        {
        }
        #endregion

        #region Public Methods
        public List<Payment> GetPayments()
        {
            return GetItemsSP("[CcEntities].[sp_Payment_GetPayments]", PaymentMapper);
        }
        #endregion  


        #region Mappers
        Payment PaymentMapper(IDataReader reader)
        {
            return new Payment
            {
                PaymentId = GetReaderValue<long>(reader, "ID"),
                Notes = GetReaderValue<string>(reader, "Notes"),
                ReferenceNumber = GetReaderValue<string>(reader, "ReferenceNumber"),
                Amount = GetReaderValue<decimal>(reader, "Amount"),
                Currency = GetReaderValue<string>(reader, "Currency"),
                PaymentDate = GetReaderValue<DateTime>(reader, "PaymentDate")
            };
        }
        #endregion
    }
}

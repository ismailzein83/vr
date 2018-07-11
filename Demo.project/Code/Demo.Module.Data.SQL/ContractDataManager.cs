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
    public class ContractDataManager : BaseSQLDataManager, IContractDataManager
    {

        #region Constructors
        public ContractDataManager() :
            base(GetConnectionStringName("DemoProjectCallCenter_DBConnStringKey", "DemoProjectCallCenter_DBConnStringKey"))
        {
        }
        #endregion

        #region Public Methods
        public Contract GetContract()
        {
            return GetItemSP("[CcEntities].[sp_Contract_GetContract]", ContractMapper);
        }
        #endregion  


        #region Mappers
        Contract ContractMapper(IDataReader reader)
        {
            return new Contract
            {
                ContractId = GetReaderValue<long>(reader, "ID"),
                MobileNumber = GetReaderValue<string>(reader, "MobileNumber"),
                MSISDN = GetReaderValue<string>(reader, "MSISDN"),
                RatePlan = GetReaderValue<string>(reader, "RatePlan")
            };
        }
        #endregion
    }
}

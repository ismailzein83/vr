using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Entities;

namespace Demo.Module.Data.SQL
{
    public class CDRDataManager : BaseSQLDataManager, ICDRDataManager
    {

        #region Constructors
        public CDRDataManager() :
            base(GetConnectionStringName("DemoProjectCallCenter_DBConnStringKey", "DemoProjectCallCenter_DBConnStringKey"))
        {
        }
        #endregion

        #region Public Methods
        public List<CDR> GetCDR(DataRetrievalInput<CDRQuery> input)
        {
            string types=null;
            if (input.Query.Type != null && input.Query.Type.Count() > 0)
                 types = string.Join<CDRType>(",", input.Query.Type);

            return GetItemsSP("[CcEntities].[sp_CDR_GetCDR]", CDRMapper, input.Query.From, input.Query.To, types);
           
        }
        #endregion  


        #region Mappers
        CDR CDRMapper(IDataReader reader)
        {
            return new CDR
            {
                CDRId = GetReaderValue<long>(reader, "ID"),
                Time = GetReaderValue<DateTime>(reader, "Time"),
                Direction = GetReaderValue<string>(reader, "Direction"),
                PhoneNumber = GetReaderValue<string>(reader, "PhoneNumber"),
                Duration = GetReaderValue<decimal>(reader, "Duration"),
                Type = GetReaderValue<string>(reader, "Type")
            };
        }
        #endregion
    }
}

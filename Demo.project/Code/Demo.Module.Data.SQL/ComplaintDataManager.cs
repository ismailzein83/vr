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
    public class ComplaintDataManager : BaseSQLDataManager, IComplaintDataManager
    {

        #region Constructors
        public ComplaintDataManager() :
            base(GetConnectionStringName("DemoProjectCallCenter_DBConnStringKey", "DemoProjectCallCenter_DBConnStringKey"))
        {
        }
        #endregion

        #region Public Methods
        public List<Complaint> GetComplaints()
        {
            return GetItemsSP("[CcEntities].[sp_Complaints_GetComplaints]", ComplaintMapper);
        }
        #endregion  


        #region Mappers
        Complaint ComplaintMapper(IDataReader reader)
        {
            return new Complaint
            {
                ComplaintId = GetReaderValue<long>(reader, "ID"),
                Time = GetReaderValue<DateTime>(reader, "Time"),
                Category = GetReaderValue<string>(reader, "Category"),
                Status = GetReaderValue<string>(reader, "Status"),
                Agent = GetReaderValue<string>(reader, "Agent"),
                Notes = GetReaderValue<string>(reader, "Notes")
            };
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Entities;

namespace Vanrise.Common.Data.SQL
{
    public class VRActionAuditLKUPDataManager : BaseSQLDataManager, IVRActionAuditLKUPDataManager
    { 
        public VRActionAuditLKUPDataManager()
            : base(GetConnectionStringName("LoggingDBConnStringKey", "LogDBConnString"))
        {

        }
        #region public Methods
        public int AddLKUPIfNotExists(Entities.VRActionAuditLKUPType lkupType, string name)
        {
            return (int)ExecuteScalarSP("logging.sp_ActionAuditLKUP_InsertIfNotExists", (int)lkupType, name);
        }

        public List<Entities.VRActionAuditLKUP> GetAll()
        {
            return GetItemsSP("logging.sp_ActionAuditLKUP_GetAll", (reader) =>
                {

                    return new VRActionAuditLKUP
                    {
                        VRActionAuditLKUPId = (int)reader["ID"],
                        Type = (VRActionAuditLKUPType)reader["Type"],
                        Name = reader["Name"] as string
                    };
                });
        }

        public bool AreVRActionAuditLKUPUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[logging].[ActionAuditLKUP]", ref updateHandle);
        }
        #endregion
    }
}

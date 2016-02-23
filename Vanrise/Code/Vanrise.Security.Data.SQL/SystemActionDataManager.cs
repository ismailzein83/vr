using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Data.SQL
{
    public class SystemActionDataManager : BaseSQLDataManager, ISystemActionDataManager
    {
        #region Constructors/Fields

        public SystemActionDataManager()
            : base(GetConnectionStringName("SecurityDBConnStringKey", "SecurityDBConnString"))
        {

        }
        
        #endregion

        #region Public Methods

        public IEnumerable<SystemAction> GetSystemActions()
        {
            return GetItemsSP("sec.sp_SystemAction_GetAll", SystemActionMapper);
        }

        public bool AreSystemActionsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("sec.SystemAction", ref updateHandle);
        }
        
        #endregion

        #region Mappers

        SystemAction SystemActionMapper(IDataReader reader)
        {
            return new SystemAction()
            {
                SystemActionId = (int)reader["ID"],
                Name = (string)reader["Name"],
                RequiredPermissions = (string)reader["RequiredPermissions"]
            };
        }
        
        #endregion
    }
}

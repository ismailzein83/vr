using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Data.SQL
{
    public class RequiredPermissionSetDataManager : BaseSQLDataManager, IRequiredPermissionSetDataManager
    {
        #region ctor
        public RequiredPermissionSetDataManager()
            : base(GetConnectionStringName("SecurityDBConnStringKey", "SecurityDBConnString"))
        {

        }

        #endregion

        public int AddIfNotExists(string module, string requiredPermissionString)
        {
            return (int)ExecuteScalarSP("[sec].[sp_RequiredPermissionSet_InsertIfNotExists]", module, requiredPermissionString);
        }

        public List<RequiredPermissionSet> GetAll()
        {
            return GetItemsSP("[sec].[sp_RequiredPermissionSet_GetAll]",
                (reader) =>
                {
                    return new RequiredPermissionSet
                    {
                        RequiredPermissionSetId = (int)reader["ID"],
                        Module = reader["Module"] as string,
                        RequiredPermissionString = reader["RequiredPermissionString"] as string
                    };
                });
        }


        public bool AreRequiredPermissionSetsUpdated(ref object updateHandle)
        {
            return IsDataUpdated("[sec].[RequiredPermissionSet]", ref updateHandle);
        }
    }
}

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
    public class UserGroupDataManager : BaseSQLDataManager, IUserGroupDataManager
    {
         #region ctor
        public UserGroupDataManager()
            : base(GetConnectionStringName("SecurityDBConnStringKey", "SecurityDBConnString"))
        {

        }

        #endregion

        #region Public Methods

        public IEnumerable<UserGroup> GetAllUserGroupEntities()
        {
            return GetItemsSP("sec.sp_UserGroup_GetAll", UserGroupMapper);
        }

        public bool AreUserGroupUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("sec.[UserGroup]", ref updateHandle);
        }

        #endregion

        #region Mappers

        private UserGroup UserGroupMapper(IDataReader reader)
        {
            return new Entities.UserGroup
            {
                UserId = Convert.ToInt32(reader["UserId"]),
                GroupId = Convert.ToInt32(reader["GroupId"])
            };
        }

        #endregion
    }
}

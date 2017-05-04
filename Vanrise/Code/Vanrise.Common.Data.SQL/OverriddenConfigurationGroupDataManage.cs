using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Entities;

namespace Vanrise.Common.Data.SQL
{
    public class OverriddenConfigurationGroupDataManager : BaseSQLDataManager, IOverriddenConfigurationGroupDataManager
    {
        #region ctor/Local Variables
        public OverriddenConfigurationGroupDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnString", "ConfigurationDBConnString"))
        {

        }

        #endregion

        #region Public Methods
       public List<OverriddenConfigurationGroup> GetOverriddenConfigurationGroup()
        {
            return GetItemsSP("Common.sp_OverriddenConfigurationGroup_GetAll", OverriddenConfigurationGroupMapper);
        }
        public bool AreOverriddenConfigurationGroupUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("Common.OverriddenConfigurationGroup", ref updateHandle);
        }
        public bool Insert(OverriddenConfigurationGroup overriddenConfigurationGroup)
        {
            int affectedRecords = ExecuteNonQuerySP("Common.sp_OverriddenConfigurationGroup_Insert", overriddenConfigurationGroup.OverriddenConfigurationGroupId, overriddenConfigurationGroup.Name);
            return (affectedRecords > 0);

        }
       
        #endregion

        #region Mappers
        OverriddenConfigurationGroup OverriddenConfigurationGroupMapper(IDataReader reader)
        {
            OverriddenConfigurationGroup overriddenConfigurationGroup = new OverriddenConfigurationGroup
            {
                OverriddenConfigurationGroupId = (Guid)reader["ID"],
                Name = reader["Name"] as string,
            };
            return overriddenConfigurationGroup;
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demo.Module.Entities;
using Vanrise.Data.SQL;

namespace Demo.Module.Data.SQL
{
    public class DestinationGroupDataManager : BaseSQLDataManager, IDestinationGroupDataManager
    {

        #region ctor/Local Variables
        public DestinationGroupDataManager()
            : base(GetConnectionStringName("DemoProject_DBConnStringKey", "DemoDBConnectionString"))
        {

        }
        #endregion

        #region Public Methods
        public bool Insert(DestinationGroup group, out int insertedId)
        {
            object infoId;

            int recordsEffected = ExecuteNonQuerySP("dbo.sp_DestinationGroup_Insert", out infoId, group.DestinationType, Vanrise.Common.Serializer.Serialize(group.GroupSettings), group.Name);
            bool insertedSuccesfully = (recordsEffected > 0);
            if (insertedSuccesfully)
                insertedId = (int)infoId;
            else
                insertedId = 0;
            return insertedSuccesfully;
        }
        public bool Update(DestinationGroup group)
        {
            int recordsEffected = ExecuteNonQuerySP("dbo.sp_DestinationGroup_Update", group.DestinationGroupId, group.DestinationType, Vanrise.Common.Serializer.Serialize(group.GroupSettings), group.Name);
            return (recordsEffected > 0);
        }
        public bool AreDestinationGroupsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("dbo.DestinationGroup", ref updateHandle);
        }
        public List<DestinationGroup> GetDestinationGroups()
        {
            return GetItemsSP("dbo.sp_DestinationGroup_GetAll", DestinationGroupMapper);
        }
        #endregion

        #region Private Methods

        #endregion

        #region  Mappers
        private DestinationGroup DestinationGroupMapper(IDataReader reader)
        {
            string GroupSettingsString = reader["GroupSettings"] as string;

            DestinationGroup group = new DestinationGroup
            {
                DestinationGroupId = (int)reader["ID"],
                DestinationType = (int)reader["DestinationType"],
                Name = reader["Name"] as string,
                GroupSettings = (GroupSettingsString != null ? Vanrise.Common.Serializer.Deserialize<GroupType>(GroupSettingsString) : null)
            };
            return group;
        }

        #endregion

    }
}

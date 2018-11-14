using Demo.BestPractices.Entities;
using System.Collections.Generic;
using System.Data;
using Vanrise.Data.SQL;

namespace Demo.BestPractices.Data.SQL
{
    public class ChildDataManager : BaseSQLDataManager, IChildDataManager
    {
        #region Properties/Ctor

        public ChildDataManager() :
            base(GetConnectionStringName("DemoProject_DBConnStringKey", "DemoProject_DBConnStringKey"))
        {
        }

        #endregion

        #region Public Methods

        public bool Insert(Child child, out long insertedId)
        {
            string serializedChildSettings = child.Settings != null ? Vanrise.Common.Serializer.Serialize(child.Settings) : null;

            object id;
            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_Child_Insert]", out id, child.Name, child.ParentId, serializedChildSettings);

            bool result = nbOfRecordsAffected > 0;
            if (result)
                insertedId = (long)id;
            else
                insertedId = 0;

            return result;
        }

        public bool Update(Child child)
        {
            string serializedChildSettings = null;
            if (child.Settings != null)
                serializedChildSettings = Vanrise.Common.Serializer.Serialize(child.Settings);

            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_Child_Update]", child.ChildId, child.Name, child.ParentId, serializedChildSettings);
            return (nbOfRecordsAffected > 0);
        }

        public bool AreChildsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[dbo].[Child]", ref updateHandle);
        }

        public List<Entities.Child> GetChilds()
        {
            return GetItemsSP("[dbo].[sp_Child_GetAll]", ChildMapper);
        }

        #endregion

        #region Mappers

        private Child ChildMapper(IDataReader reader)
        {
            return new Child
            {
                ChildId = GetReaderValue<long>(reader, "ID"),
                Name = GetReaderValue<string>(reader, "Name"),
                ParentId = GetReaderValue<long>(reader, "ParentId"),
                Settings = Vanrise.Common.Serializer.Deserialize<ChildSettings>(reader["Settings"] as string)
            };
        }

        #endregion
    }
}
using Demo.Module.Entities;
using System.Collections.Generic;
using System.Data;
using Vanrise.Data.SQL;

namespace Demo.Module.Data.SQL
{
    class ZooDataManager : BaseSQLDataManager, IZooDataManager
    {
        #region Properties/Ctor

        public ZooDataManager() :
            base(GetConnectionStringName("DemoProject_DBConnStringKey", "DemoProject_DBConnStringKey"))
        {

        }

        #endregion

        #region Public Methods

        public List<Entities.Zoo> GetZoos()
        {
            return GetItemsSP("[dbo].[sp_Zoo_GetAll]", ZooMapper);
        }

        public bool Insert(Zoo zoo, out long insertedId)
        {
            object id;
            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_Zoo_Insert]", out id, zoo.Name, zoo.City, zoo.Size);

            bool result = nbOfRecordsAffected > 0;

            if (result)
                insertedId = (long)id;
            else
                insertedId = 0;

            return result;
        }

        public bool Update(Zoo zoo)
        {
            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_Zoo_Update]", zoo.ZooId, zoo.Name, zoo.City, zoo.Size);
            return nbOfRecordsAffected > 0;
        }

        #endregion

        #region Mappers

        public Zoo ZooMapper(IDataReader reader)
        {
            return new Zoo()
            {
                ZooId = GetReaderValue<long>(reader, "ID"),
                Name = GetReaderValue<string>(reader, "Name"),
                City = GetReaderValue<string>(reader, "City"),
                Size = GetReaderValue<ZooSizeEnum>(reader, "Size")
            };
        }

        #endregion
    }
}

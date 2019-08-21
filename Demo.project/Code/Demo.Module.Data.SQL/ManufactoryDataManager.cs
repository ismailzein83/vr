using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using Vanrise.Data.SQL;

namespace Demo.Module.Data.SQL
{
    public class ManufactoryDataManager : BaseSQLDataManager, IManufactoryDataManager
    {
        public ManufactoryDataManager() :
            base(GetConnectionStringName("DemoProject_DBConnStringKey", "DemoProject_DBConnStringKey"))
        {
        }

        #region Public Methods

        public List<Manufactory> GetManufactories()
        {
            return GetItemsSP("[dbo].[sp_Manufactory_GetAll]", ManufactoryMapper);
        }

        public bool InsertManufactory(Manufactory manufactory, out int insertedId)
        {
            object id;
            int numberOfAffectedRows = ExecuteNonQuerySP("[dbo].[sp_Manufactory_Insert]", out id, manufactory.Name, manufactory.CountryOfOrigin);

            bool result = numberOfAffectedRows > 0;
            if (result)
                insertedId = Convert.ToInt32(id);
            else
                insertedId = -1;

            return result;
        }

        public bool UpdateManufactory(Manufactory manufactory)
        {
            int numberOfAffectedRows = ExecuteNonQuerySP("[dbo].[sp_Manufactory_Update]", manufactory.Id, manufactory.Name, manufactory.CountryOfOrigin);
            return numberOfAffectedRows > 0;
        }

        public bool AreManufactoriesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[dbo].[Manufactory]", ref updateHandle);
        }

        #endregion

        #region Private Methods

        private Manufactory ManufactoryMapper(IDataReader dataReader)
        {
            return new Manufactory()
            {
                Id = GetReaderValue<int>(dataReader, "Id"),
                Name = GetReaderValue<string>(dataReader, "Name"),
                CountryOfOrigin = GetReaderValue<string>(dataReader, "CountryOfOrigin")
            };
        }

        #endregion
    }
}

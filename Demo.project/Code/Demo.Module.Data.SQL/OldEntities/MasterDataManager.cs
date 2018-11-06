using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using System.Data;
using Demo.Module.Entities;
using Newtonsoft.Json;

namespace Demo.Module.Data.SQL
{
    public class MasterDataManager : BaseSQLDataManager, IMasterDataManager
    {

        #region Constructors
        public MasterDataManager() :
            base(GetConnectionStringName("DemoProject_DBConnStringKey", "DemoProject_DBConnStringKey"))
        {
        }
        #endregion

        #region Public Methods
        public bool AreMastersUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[dbo].[Master]", ref updateHandle);
        }
        public List<Master> GetMasters()
        {
            return GetItemsSP("[dbo].[sp_Master_GetAll]", MasterMapper);
        }
        public bool Insert(Master master, out long insertedId)
        {
            object id;
            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_Master_Insert]", out id, master.Name);
            bool result = (nbOfRecordsAffected > 0);
            if (result)
                insertedId = (long)id;
            else
                insertedId = 0;
            return result;
        }
        public bool Update(Master master)
        {

            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_Master_Update]", master.MasterId, master.Name);
            return (nbOfRecordsAffected > 0);
        }

        #endregion

        #region Mappers
        Master MasterMapper(IDataReader reader)
        {
            return new Master
            {
                MasterId = GetReaderValue<long>(reader, "ID"),
                Name = GetReaderValue<string>(reader, "Name")
            };
        }
        #endregion
    }
}
using Demo.BestPractices.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace Demo.BestPractices.Data.SQL
{
    public class ChildDataManager : BaseSQLDataManager, IChildDataManager
    {
        
        #region Constructors
        public ChildDataManager() :
            base(GetConnectionStringName("DemoProject_DBConnStringKey", "DemoProject_DBConnStringKey"))
        {
        }
        #endregion

        #region Public Methods
        public bool AreChildsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[dbo].[Child]", ref updateHandle);
        }
        public List<Entities.Child> GetChilds()
        {
            return GetItemsSP("[dbo].[sp_Child_GetAll]", ChildMapper);
        }
        public bool Insert(Child child, out long insertedId)
        {
            object id;
            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_Child_Insert]", out id, child.Name, child.ParentId);
            bool result = (nbOfRecordsAffected > 0);
            if (result)
                insertedId = (long)id;
            else
                insertedId = 0;
            return result;
        }
        public bool Update(Child child)
        {

            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_Child_Update]", child.ChildId, child.Name, child.ParentId);
            return (nbOfRecordsAffected > 0);
        }

        #endregion

        #region Mappers
        Child ChildMapper(IDataReader reader)
        {
            return new Child
            {
                ChildId = GetReaderValue<long>(reader, "ID"),
                Name = GetReaderValue<string>(reader, "Name"),
                ParentId = GetReaderValue<long>(reader, "ParentId"),
            };
        }
        #endregion
    }
}

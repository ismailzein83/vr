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
    public class ParentDataManager: BaseSQLDataManager, IParentDataManager
    {

        #region Constructors
        public ParentDataManager() :
            base(GetConnectionStringName("DemoProject_DBConnStringKey", "DemoProject_DBConnStringKey"))
        {
        }
        #endregion

        #region Public Methods
        public bool AreCompaniesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[dbo].[Parent]", ref updateHandle);
        }
        public List<Entities.Parent> GetParents()
        {
            return GetItemsSP("[dbo].[sp_Parent_GetAll]", ParentMapper);
        }
        public bool Insert(Parent parent, out long insertedId)
        {
            object id;
            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_Parent_Insert]", out id, parent.Name);
            insertedId = Convert.ToInt64(id);
            return (nbOfRecordsAffected > 0);
        }
        public bool Update(Parent parent)
        {

            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_Parent_Update]", parent.ParentId, parent.Name);
            return (nbOfRecordsAffected > 0);
        }

        #endregion

        #region Mappers
        Parent ParentMapper(IDataReader reader)
        {
            return new Parent
            {
                ParentId = GetReaderValue<long>(reader, "ID"),
                Name = GetReaderValue<string>(reader, "Name")
            };
        }
        #endregion
    }
}

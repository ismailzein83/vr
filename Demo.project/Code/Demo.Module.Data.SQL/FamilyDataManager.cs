using Demo.Module.Entities;

using System;

using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace Demo.Module.Data.SQL
{
    public class FamilyDataManager : BaseSQLDataManager, IFamilyDataManager
    {

        #region Constructors
        public FamilyDataManager() :
            base(GetConnectionStringName("DemoProject_DBConnStringKey", "DemoProject_DBConnStringKey"))
        {
        }
        #endregion

        #region Public Methods
        public bool AreCompaniesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[dbo].[Family1]", ref updateHandle);
        }
        public List<Family> GetFamilies()
        {
            return GetItemsSP("[dbo].[sp_Family1_GetAll]", FamilyMapper);
        }
        public bool Insert(Family family, out long insertedId)
        {
            object id;
            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_Family_Insert]", out id, family.Name);
            bool result = (nbOfRecordsAffected > 0);
            if (result)
                insertedId = (long)id;
            else
                insertedId = 0;
            return result;
        }
        public bool Update(Family family)
        {

            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_Family_Update]", family.FamilyId, family.Name);
            return (nbOfRecordsAffected > 0);
        }

        #endregion

        #region Mappers
        Family FamilyMapper(IDataReader reader)
        {
            return new Family
            {
                FamilyId = GetReaderValue<long>(reader, "ID"),
                Name = GetReaderValue<string>(reader, "Name")
            };
        }
        #endregion
    }
}

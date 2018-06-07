using Demo.Module.Entities.Member;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace Demo.Module.Data.SQL
{
    public class MemberDataManager : BaseSQLDataManager,IMemberDataManager
    {

 
        #region Constructors
        public MemberDataManager() :
            base(GetConnectionStringName("DemoProject_DBConnStringKey", "DemoProject_DBConnStringKey"))
        {
        }
        #endregion

        #region Public Methods
        public bool AreMembersUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[dbo].[Member]", ref updateHandle);
        }
        public List<Member> GetMembers()
        {
            return GetItemsSP("[dbo].[sp_Member_GetAll]", MemberMapper);
        }
        public bool Insert(Member member, out long insertedId)
        {
            object id;
            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_Member_Insert]", out id, member.Name, member.FamilyId);
            bool result = (nbOfRecordsAffected > 0);
            if (result)
                insertedId = (long)id;
            else
                insertedId = 0;
            return result;
        }
        public bool Update(Member member)
        {

            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_Member_Update]", member.MemberId, member.Name, member.FamilyId);
            return (nbOfRecordsAffected > 0);
        }

        #endregion

        #region Mappers
        Member MemberMapper(IDataReader reader)
        {
            return new Member
            {
                MemberId = GetReaderValue<long>(reader, "ID"),
                Name = GetReaderValue<string>(reader, "Name"),
                FamilyId = GetReaderValue<long>(reader, "FamilyID"),
            };
        }
        #endregion
    }
}

using Demo.Module.Entities.Member;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Data
{
  public interface IMemberDataManager:IDataManager
    {
        bool AreMembersUpdated(ref object updateHandle);
        List<Member> GetMembers();
        bool Insert(Member member, out long insertedId);
        bool Update(Member member);
    }
}

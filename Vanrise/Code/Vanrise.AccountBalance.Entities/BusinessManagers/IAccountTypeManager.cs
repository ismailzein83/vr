using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountBalance.Entities
{
    public interface IAccountTypeManager : IBusinessManager
    {
       bool DoesUserHaveViewAccess(int UserId, List<Guid> AccountTypeIds);
    }
}

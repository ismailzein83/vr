using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public interface IUserManager : IBEManager
    {
        string GetUserName(int userId);

        int? GetSystemUserId();
    }
}

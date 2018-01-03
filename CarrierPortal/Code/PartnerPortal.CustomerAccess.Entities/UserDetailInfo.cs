using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace PartnerPortal.CustomerAccess.Entities
{
   public class UserDetailInfo
    {
       public int UserId { get; set; }
       public UserStatus UserStatus { get; set; }
    }
}

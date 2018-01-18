using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
   public class UserProfile
    {
       public int UserId { get; set; }
       public string Name { get; set; }
       public long? PhotoFileId { get; set; }
    }
}

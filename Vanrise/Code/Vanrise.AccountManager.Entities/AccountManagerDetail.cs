using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountManager.Entities
{
  public  class AccountManagerDetail
    {
      public Guid AccountManagerDefinitionId { get; set; }
      public long AccountManagerId { get; set; }
      public string UserName { get; set; }
      public int UserId { get; set; }
    }
}

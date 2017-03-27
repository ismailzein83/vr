using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Notification.Entities
{
  public  class VRAlertLevelQuery
    {
        public string Name { get; set; }
        public Guid? BusinessEntityDefinitionId { get; set; }
    }
}

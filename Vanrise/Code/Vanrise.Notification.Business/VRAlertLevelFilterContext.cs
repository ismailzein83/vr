using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Notification.Entities;

namespace Vanrise.Notification.Business
{
  public  class VRAlertLevelFilterContext : IVRALertLevelFilterContext
    {
      public VRAlertLevel VRAlertLevel { get; set; }
    }
}

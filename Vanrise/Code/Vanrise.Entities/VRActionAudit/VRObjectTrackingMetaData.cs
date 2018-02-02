using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
namespace Vanrise.Entities
{
    public class VRObjectTrackingMetaData
    {
      public long VRObjectTrackingId { get; set; }
      public DateTime Time { get; set; }
      public int UserId { get; set; }
      public int ActionId { get; set; }
      public bool HasDetail { get; set; }
      public bool HasChangeInfo { get; set; }
      public string ActionDescription { get; set; }
 
    }
}

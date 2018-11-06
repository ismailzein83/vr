using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities
{
   public class Interaction
    {
        public long InteractionId { get; set; }
        public int SenderType { get; set; }
        public string SenderName { get; set; }
        public string Message { get; set; }
        public DateTime Time { get; set; }
    }
}

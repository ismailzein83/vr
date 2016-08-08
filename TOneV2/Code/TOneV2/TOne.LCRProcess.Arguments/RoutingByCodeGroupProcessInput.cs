using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOne.LCRProcess.Arguments
{
    public class RoutingByCodeGroupProcessInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public int RoutingDatabaseId { get; set; }

        public string CodePrefix { get; set; }

        public DateTime EffectiveTime { get; set; }

        public bool IsFuture { get; set; }

        public bool IsLcrOnly { get; set; }

        public override string GetTitle()
        {
            return String.Format("#BPDefinitionTitle# for Codes starts by {0}", CodePrefix);
        }
    }
}

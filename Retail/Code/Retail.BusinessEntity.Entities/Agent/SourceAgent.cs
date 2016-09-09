using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BEBridge.Entities;

namespace Retail.BusinessEntity.Entities
{
    public class SourceAgent : ITargetBE
    {
        public Agent Agent { get; set; }

        public object SourceBEId
        {
            get { return Agent.SourceId; }
        }

        public object TargetBEId
        {
            get { return Agent.Id; }
        }
    }
}

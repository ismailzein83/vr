﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.CodePreparation.Entities.Processing
{
    public class ZoneToProcess : IRuleTarget
    {
        public ZoneToProcess()
        {
            CodesToAdd = new List<CodeToAdd>();
            CodesToMove = new List<CodeToMove>();
            CodesToClose = new List<CodeToClose>();
        }
        public string ZoneName { get; set; }

        public List<CodeToAdd> CodesToAdd { get; set; }

        public List<CodeToMove> CodesToMove { get; set; }

        public List<CodeToClose> CodesToClose { get; set; }

        public IEnumerable<AddedZone> AddedZones { get; set; }

        public IEnumerable<ExistingZone> NewAndExistingZones { get; set; }

        public object Key
        {
            get { return this.ZoneName; }
        }

        public string TargetType
        {
            get { return "Zone"; }
        }
    }
}

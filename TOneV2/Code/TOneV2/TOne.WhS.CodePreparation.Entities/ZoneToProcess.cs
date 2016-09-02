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
        public string ZoneName { get; set; }

        private List<CodeToAdd> _codesToAdd = new List<CodeToAdd>();
        public List<CodeToAdd> CodesToAdd
        {
            get
            {
                return this._codesToAdd;
            }
        }


        private List<CodeToMove> _codesToMove = new List<CodeToMove>();

        public List<CodeToMove> CodesToMove
        {
            get
            {
                return this._codesToMove;
            }
        }


        private List<CodeToClose> _codesToClose = new List<CodeToClose>();

        public List<CodeToClose> CodesToClose
        {
            get
            {
                return this._codesToClose;
            }
        }

        private List<AddedZone> _addedZones = new List<AddedZone>();

        public List<AddedZone> AddedZones
        {
            get
            {
                return this._addedZones;
            }
        }


        private List<ExistingZone> _existingZones = new List<ExistingZone>();
        public List<ExistingZone> ExistingZones 
        { 
            get
            {
                return this._existingZones;
            }
        }


        private List<RateToAdd> _ratesToAdd = new List<RateToAdd>();

        public List<RateToAdd> RatesToAdd
        {
            get
            {
                return this._ratesToAdd;
            }
        }

        public ExistingRate SystemRate { get; set; }
        public string RecentZoneName { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }
        public ZoneChangeType ChangeType { get; set; }

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

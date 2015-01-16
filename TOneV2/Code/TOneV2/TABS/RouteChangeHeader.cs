using System;
using System.Collections.Generic;

namespace TABS
{
    [Serializable]
    public class RouteChangeHeader : Components.BaseEntity
    {
        public override string Identifier { get { return "RouteChangeHeader:" + (ID == 0 ? "#" : ID.ToString()); } }

        public virtual int ID { get; set; }
        public virtual string Reason { get; set; }
        public virtual DateTime Created { get; set; }
        protected string _IsEnded;

        public virtual bool IsEnded
        {
            get { return "Y".Equals(_IsEnded); }
            set { _IsEnded = value ? "Y" : "N"; }
        }

        public IList<SpecialRequest> GetSpecialRequests()
        {
            return ObjectAssembler.GetSpecialRequests(this);
        }

        public IList<RouteBlock> GetRouteBlocks()
        {
            return ObjectAssembler.GetRouteBlocks(this);
        }
    }
}

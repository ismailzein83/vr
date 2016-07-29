using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.RouteSync.Entities;

namespace TOne.WhS.RouteSync.BP.Activities
{

    public sealed class GetRouteReadRanges : CodeActivity
    {
        [RequiredArgument]
        public InArgument<RouteReader> RouteReader { get; set; }

        [RequiredArgument]
        public OutArgument<RouteRangeType?> RangeType { get; set; }

        [RequiredArgument]
        public OutArgument<List<RouteRangeInfo>> Ranges { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var tryGetReadRangesContext = new RouteReaderGetReadRangesContext();
            if(this.RouteReader.Get(context).TryGetReadRanges(tryGetReadRangesContext))
            {
                this.RangeType.Set(context, tryGetReadRangesContext.RangeType);
                this.Ranges.Set(context, tryGetReadRangesContext.Ranges);
            }
        }

        #region Private Classes

        private class RouteReaderGetReadRangesContext : IRouteReaderGetReadRangesContext
        {
            public RouteRangeType RangeType
            {
                set;
                get;
            }

            public List<RouteRangeInfo> Ranges
            {
                set;
                get;
            }
        }


        #endregion
    }
}

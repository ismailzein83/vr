using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.LCR.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;

namespace TOne.LCRProcess.Activities
{
    public class UpdateRouteDetails : CodeActivity
    {
        [RequiredArgument]
        public InArgument<BaseQueue<RouteDetailBatch>> InputQueue { get; set; }

        [RequiredArgument]
        public InArgument<int> RoutingDatabaseId { get; set; }

        protected override void Execute(CodeActivityContext context)
        {

        }
    }
}

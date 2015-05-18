using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.Threading.Tasks;

namespace TOne.CDRProcess.Activities
{
    public class InitializeObjects : CodeActivity
    {
        protected override void Execute(CodeActivityContext context)
        {
            //Task t = new Task(() =>
            //    {
            //        CDRManager manager = new CDRManager();
            //        manager.ReserveRePricingMainCDRIDs(0);
            //    });
            //t.Start();
            //t = new Task(() =>
            //{
            //    CDRManager manager = new CDRManager();
            //    manager.ReserveRePricingInvalidCDRIDs(0);
            //});
            //t.Start();
        }
    }
}

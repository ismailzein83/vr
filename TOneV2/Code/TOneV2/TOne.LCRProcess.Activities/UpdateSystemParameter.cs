using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;

namespace TOne.LCRProcess.Activities
{

    public sealed class UpdateSystemParameter : CodeActivity
    {
        [RequiredArgument]
        public InArgument<TABS.SystemParameter> Parameter { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            using (NHibernate.ISession session = TABS.DataConfiguration.OpenSession())
            {
                session.Update(this.Parameter.Get(context));
                session.Flush();
            }
        }
    }
}

using Mediation.Generic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mediation.Generic.MainExtensions.MediationOutputHandlers
{
    public class ExecuteOtherMediationOutputHandler : MediationOutputHandler
    {
        public override Guid ConfigId
        {
            get { return new Guid("F1D57186-49CE-4BF9-B4B6-46DDCE93E9EC"); }
        }

        public int MediationDefinitionId { get; set; }

        public override void Execute(IMediationOutputHandlerContext context)
        {
            throw new NotImplementedException();
        }
    }
}

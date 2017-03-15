using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BEBridge.Entities;

namespace Vanrise.BEBridge.Business
{
    public class BEReceiveDefinitionStartInstanceFilter : IBEReceiveDefinitionFilter
    {
        public bool IsMatched(IBEReceiveDefinitionFilterContext context)
        {
            return new BEReceiveDefinitionManager().DoesUserHaveStartInstanceAccess(context.BEReceiveDefinitionId);
        }
    }
}

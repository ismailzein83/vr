using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BEBridge.Entities;
using Vanrise.BusinessProcess.Entities;
namespace Vanrise.BEBridge.BP.Arguments
{
    public class SourceBESyncProcessInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public override string GetTitle()
        {
            return GetReceiveDefinitionsTitle();
        }

        public override string GetDefinitionTitle()
        {
            return GetReceiveDefinitionsTitle();
        }

        public List<Guid> BEReceiveDefinitionIds { get; set; }

        string GetReceiveDefinitionsTitle()
        {
            var manager = Vanrise.BEBridge.Entities.BusinessManagerFactory.GetManager<IBEReceiveDefinitionManager>();
            List<string> bpDefinitionNames = new List<string>();
            if (this.BEReceiveDefinitionIds != null)
            {
                foreach (var id in this.BEReceiveDefinitionIds)
                {
                    string name = manager.GetReceiveDefinitionName(id);
                    if (name != null)
                        bpDefinitionNames.Add(name);
                }
            }
            return String.Join(",", bpDefinitionNames);
        }

        public override bool DosesUserHaveViewAccess()
        {
            return false;// base.DosesUserHaveViewAccess();
        }
    }
}

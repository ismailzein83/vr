using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;
using Vanrise.Common;
using Vanrise.BusinessProcess;

namespace TOne.WhS.Routing.BP.Activities
{
    public class MakeCodePrefixesAvailableForSubProcesses : CodeActivity
    {
        [RequiredArgument]
        public InArgument<IEnumerable<CodePrefix>> CodePrefixes { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            CodePrefixRequest.LoadProcessCodePrefixes(context.GetSharedInstanceData().InstanceInfo.ProcessInstanceID, this.CodePrefixes.Get(context).ToList());
        }
    }

    public class MakeCodePrefixesUnavailable : CodeActivity
    {
        protected override void Execute(CodeActivityContext context)
        {
            CodePrefixRequest.UnLoadProcessCodePrefixes(context.GetSharedInstanceData().InstanceInfo.ProcessInstanceID);
        }
    }

    public class CodePrefixRequest : Vanrise.Runtime.Entities.InterRuntimeServiceRequest<List<CodePrefix>>
    {
        public long ProcessInstanceId { get; set; }

        public string Prefix { get; set; }

        static Dictionary<long, List<CodePrefix>> s_codePrefixesByProcessInstanceId = new Dictionary<long, List<CodePrefix>>();

        public override List<CodePrefix> Execute()
        {
            return s_codePrefixesByProcessInstanceId.GetRecord(this.ProcessInstanceId).Where(itm => itm.Code.StartsWith(this.Prefix)).ToList();
        }

        internal static void LoadProcessCodePrefixes(long processInstanceId, List<CodePrefix> codePrefixes)
        {
            s_codePrefixesByProcessInstanceId.Add(processInstanceId, codePrefixes);
        }

        internal static void UnLoadProcessCodePrefixes(long processInstanceId)
        {
            s_codePrefixesByProcessInstanceId.Remove(processInstanceId);
        }
    }
}

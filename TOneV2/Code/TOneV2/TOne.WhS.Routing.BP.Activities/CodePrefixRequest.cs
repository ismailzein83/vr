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
        public InArgument<IEnumerable<IEnumerable<CodePrefix>>> CodePrefixes { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            CodePrefixRequest.LoadProcessCodePrefixes(context.GetSharedInstanceData().InstanceInfo.ProcessInstanceID, this.CodePrefixes.Get(context));
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

        static Dictionary<long, IEnumerable<IEnumerable<CodePrefix>>> s_codePrefixesByProcessInstanceId = new Dictionary<long, IEnumerable<IEnumerable<CodePrefix>>>();

        public override List<CodePrefix> Execute()
        {
            List<CodePrefix> result = new List<CodePrefix>();
            IEnumerable<IEnumerable<CodePrefix>> codePrefixes = s_codePrefixesByProcessInstanceId.GetRecord(this.ProcessInstanceId);
            foreach (IEnumerable<CodePrefix> codePrefixList in codePrefixes)
            {
                IEnumerable<CodePrefix> data = codePrefixList.Where(itm => itm.Code.StartsWith(this.Prefix));
                if (data != null)
                    result.AddRange(data.ToList());
            }
            return result;
        }

        internal static void LoadProcessCodePrefixes(long processInstanceId, IEnumerable<IEnumerable<CodePrefix>> codePrefixes)
        {
            s_codePrefixesByProcessInstanceId.Add(processInstanceId, codePrefixes);
        }

        internal static void UnLoadProcessCodePrefixes(long processInstanceId)
        {
            s_codePrefixesByProcessInstanceId.Remove(processInstanceId);
        }
    }
}

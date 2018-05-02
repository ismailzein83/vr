using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Common;

namespace TOne.WhS.Routing.BP.Activities
{
    public class LoadSupplierCodesInput
    {
        public DateTime? EffectiveOn { get; set; }

        public bool IsFuture { get; set; }

        public IEnumerable<RoutingSupplierInfo> SupplierInfo { get; set; }

        public int ParentWFRuntimeProcessId { get; set; }
        public string Prefix { get; set; }

    }

    public class LoadSupplierCodesOutput
    {

    }

    public class LoadSupplierCodes : BaseAsyncActivity<LoadSupplierCodesInput, LoadSupplierCodesOutput>
    {
        [RequiredArgument]
        public InArgument<DateTime?> EffectiveOn { get; set; }

        [RequiredArgument]
        public InArgument<bool> IsFuture { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<RoutingSupplierInfo>> SupplierInfo { get; set; }

        public InArgument<int> ParentWFRuntimeProcessId { get; set; }

        public InArgument<string> Prefix { get; set; }

        protected override LoadSupplierCodesOutput DoWorkWithResult(LoadSupplierCodesInput inputArgument, AsyncActivityHandle handle)
        {
            SupplierCodeRequest.LoadSupplierCodes(inputArgument.EffectiveOn, inputArgument.IsFuture, inputArgument.SupplierInfo, inputArgument.Prefix, inputArgument.ParentWFRuntimeProcessId, 
                handle.SharedInstanceData.InstanceInfo.ParentProcessID.Value, () => ShouldStop(handle));
            handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Loading Supplier Codes is done");
            return null;
        }

        protected override LoadSupplierCodesInput GetInputArgument(System.Activities.AsyncCodeActivityContext context)
        {
            return new LoadSupplierCodesInput
            {
                SupplierInfo = this.SupplierInfo.Get(context),
                ParentWFRuntimeProcessId = this.ParentWFRuntimeProcessId.Get(context),
                Prefix = this.Prefix.Get(context),
                EffectiveOn = this.EffectiveOn.Get(context),
                IsFuture = this.IsFuture.Get(context)
            };
        }

        protected override void OnWorkComplete(System.Activities.AsyncCodeActivityContext context, LoadSupplierCodesOutput result)
        {
            
        }
    }

    public class UnLoadSupplierCodesFromSubProcesses : CodeActivity
    {
        public InArgument<List<int>> SubProcessesRuntimeProcessIds { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            long processInstanceId = context.GetSharedInstanceData().InstanceInfo.ProcessInstanceID;
            foreach(var runtimeProcessId in this.SubProcessesRuntimeProcessIds.Get(context))
            {
                new Vanrise.Runtime.InterRuntimeServiceManager().SendRequest(runtimeProcessId, new UnloadSupplierCodesRequest { ParentProcessInstanceId = processInstanceId });
            }
        }

        private class UnloadSupplierCodesRequest : Vanrise.Runtime.Entities.InterRuntimeServiceRequest<object>
        {
            public long ParentProcessInstanceId { get; set; }

            public override object Execute()
            {
                SupplierCodeRequest.UnloadSupplierCodes(this.ParentProcessInstanceId);
                return null;
            }
        }
    }

    public class SupplierCodeRequest : Vanrise.Runtime.Entities.InterRuntimeServiceRequest<List<SupplierCode>>
    {
        public string CodePrefix { get; set; }

        public long ParentProcessInstanceId { get; set; }


        static Dictionary<long, Dictionary<string, HashSet<SupplierCode>>> s_codesByPrefixByProcessInstanceId = new Dictionary<long,Dictionary<string,HashSet<SupplierCode>>>();

        internal static void LoadSupplierCodes(DateTime? effectiveOn, bool isFuture, IEnumerable<RoutingSupplierInfo> activeSupplierInfo, string prefixOfCodesToLoad, int parentWFRuntimeProcessId, 
            long parentProcessInstanceId, Func<bool> shouldStop)
        {
            var codePrefixRequest = new CodePrefixRequest { ProcessInstanceId = parentProcessInstanceId, Prefix = prefixOfCodesToLoad };
            List<CodePrefix> codePrefixes = new Vanrise.Runtime.InterRuntimeServiceManager().SendRequest(parentWFRuntimeProcessId, codePrefixRequest);
            var codesByPrefix = s_codesByPrefixByProcessInstanceId.GetOrCreateItem(parentProcessInstanceId);
            CodeIterator<CodePrefix> codePrefixesNeedsChildrenIterator = new CodeIterator<CodePrefix>(codePrefixes.Where(itm => !itm.IsCodeDivided));
            int minLength = int.MaxValue;
            int maxLength = 0;
            Dictionary<string, List<SupplierCode>> codesByCodeValue = new Dictionary<string, List<SupplierCode>>();

            new SupplierCodeManager().LoadSupplierCodes(activeSupplierInfo, prefixOfCodesToLoad, effectiveOn, isFuture, shouldStop, (supplierCode) =>
            {
                var longestMatchCodePrefix = codePrefixesNeedsChildrenIterator.GetLongestMatch(supplierCode.Code);
                if (longestMatchCodePrefix != null)
                    codesByPrefix.GetOrCreateItem(longestMatchCodePrefix.Code).Add(supplierCode);
                codesByCodeValue.GetOrCreateItem(supplierCode.Code).Add(supplierCode);

                int codeLength = supplierCode.Code.Length;
                if (codeLength < minLength)
                    minLength = codeLength;
                if (codeLength > maxLength)
                    maxLength = codeLength;
            });
            
            foreach(var codePrefix in codePrefixes)
            {
                if (shouldStop != null && shouldStop())
                    break;

                var codePrefixCodes = codesByPrefix.GetOrCreateItem(codePrefix.Code);

                string prefix = codePrefix.Code.Substring(0, Math.Min(maxLength, codePrefix.Code.Length));
                while (prefix.Length >= minLength)
                {
                    List<SupplierCode> matchCodes;
                    if (codesByCodeValue.TryGetValue(prefix, out matchCodes))
                    {
                        foreach (var code in matchCodes)
                        {
                            codePrefixCodes.Add(code);
                        }
                    }
                    prefix = prefix.Substring(0, prefix.Length - 1);
                }
            }
        }

        internal static void UnloadSupplierCodes(long processInstanceId)
        {
            s_codesByPrefixByProcessInstanceId.Remove(processInstanceId);
        }

        public override List<SupplierCode> Execute()
        {
            var codesByPrefix = s_codesByPrefixByProcessInstanceId[this.ParentProcessInstanceId];
            var codes = codesByPrefix.GetRecord(this.CodePrefix).ToList();
            lock (codesByPrefix)
            {
                codesByPrefix.Remove(this.CodePrefix);
            }
            return codes;
        }
    }
}

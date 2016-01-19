using System;
using System.Activities;
using System.Collections.Generic;
using Vanrise.BusinessProcess;
using Vanrise.Common;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{

    public class GetPrefixes : CodeActivity
    {
        #region Arguments

        [RequiredArgument]
        public InArgument<int> PrefixLength { get; set; }

        public OutArgument<List<string>> Prefixes { get; set; }


        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            int prefixLength = this.PrefixLength.Get(context);
            ContextExtensions.WriteTrackingMessage(context, LogEntryType.Verbose, "Started creating prefix list with length {0}", prefixLength);

            List<string> prefixes = new List<string>();
            if (prefixLength == 0)
                prefixes.Add("");
            else
            {
                string prefixMax = "";

                for (int i = 0; i < prefixLength; i++)
                    prefixMax = prefixMax + "9";

                int maxValue = 0;
                int.TryParse(prefixMax, out maxValue);

                for (int i = 0; i <= maxValue; i++)
                    prefixes.Add(i.ToString("D" + prefixLength as string));
            }

            ContextExtensions.WriteTrackingMessage(context, LogEntryType.Verbose, "Finished creating prefix list ");

            context.SetValue(Prefixes, prefixes);
        }
    }
}

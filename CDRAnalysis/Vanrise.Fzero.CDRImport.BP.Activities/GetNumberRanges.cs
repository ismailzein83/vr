using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Common;

namespace Vanrise.Fzero.CDRImport.BP.Activities
{

    public sealed class GetNumberRanges : CodeActivity
    {
        public InArgument<IEnumerable<string>> FixedPrefixes { get; set; }

        [RequiredArgument]
        public InArgument<int> PrefixLength { get; set; }

        public OutArgument<List<NumberRangeDefinition>> NumberRanges { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            int prefixLength = this.PrefixLength.Get(context);
            context.WriteTrackingMessage(LogEntryType.Verbose, "Started creating prefix list with length {0}", prefixLength);

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

            List<NumberRangeDefinition> numberRanges = new List<NumberRangeDefinition>();
            IEnumerable<string> fixedPrefixes = this.FixedPrefixes.Get(context);
            foreach(var prefix in prefixes)
            {
                NumberRangeDefinition numberRange = new NumberRangeDefinition { Prefixes = new List<string>() };
                numberRanges.Add(numberRange);
                if (fixedPrefixes != null && fixedPrefixes.Count() > 0)
                {
                    foreach (var fixedPrefix in fixedPrefixes)
                    {
                        numberRange.Prefixes.Add(String.Format("{0}{1}", fixedPrefix, prefix));
                    }
                }
                else
                    numberRange.Prefixes.Add(prefix);
            }

            context.WriteTrackingMessage(LogEntryType.Verbose, "Finished creating prefix list ");

            context.SetValue(this.NumberRanges, numberRanges);
        }
    }
}

using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestRuntime;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;

namespace TOne.WhS.Runtime.Tasks
{
    public class AliAtouiTask: ITask
    {
        public void Execute()
        {
            Console.WriteLine("Ali Atoui");


            int prefixLenght = 1;
            DateTime? effectiveOn = DateTime.Now;
            bool isFuture = false;

            HashSet<string> prefixesHashSet = new HashSet<string>();
            List<CodePrefixInfo> codePrefixes = new List<CodePrefixInfo>();
            long validNumberPrefix;

            SupplierCodeManager supplierCodeManager = new SupplierCodeManager();
            IEnumerable<CodePrefixInfo> supplierCodePrefixes = supplierCodeManager.GetDistinctCodeByPrefixes(prefixLenght, effectiveOn, isFuture);

            if (supplierCodePrefixes != null)
            {
                foreach (CodePrefixInfo item in supplierCodePrefixes)
                    if (long.TryParse(item.CodePrefix, out validNumberPrefix))
                    {
                        if (!prefixesHashSet.Contains(item.CodePrefix))
                        {
                            codePrefixes.Add(item);
                            prefixesHashSet.Add(item.CodePrefix);
                        }
                    }
                    //else
                    //    context.WriteTrackingMessage(LogEntryType.Warning, "Invalid Supplier Code Prefix: {0}", item.CodePrefix);
            }


            SaleCodeManager saleCodeManager = new SaleCodeManager();
            IEnumerable<CodePrefixInfo> saleCodePrefixes = saleCodeManager.GetDistinctCodeByPrefixes(prefixLenght, effectiveOn, isFuture);

            if (saleCodePrefixes != null)
            {
                foreach (CodePrefixInfo item in saleCodePrefixes)
                    if (long.TryParse(item.CodePrefix, out validNumberPrefix))
                    {
                        if (!prefixesHashSet.Contains(item.CodePrefix))
                        {
                            codePrefixes.Add(item);
                            prefixesHashSet.Add(item.CodePrefix);
                        }
                    }
                    //else
                    //    context.WriteTrackingMessage(LogEntryType.Warning, "Invalid Sale Code Prefix: {0}", item.CodePrefix);
            }

            IEnumerable<string> result = codePrefixes.OrderBy(itm => itm.Count).Select(itm => itm.CodePrefix);

            foreach (CodePrefixInfo codePrefix in codePrefixes)
            {
                Console.WriteLine(codePrefix.CodePrefix + "  " + codePrefix.Count);
            }

            //this.DistinctCodePrefixes.Set(context, result);
            //context.GetSharedInstanceData().WriteTrackingMessage(LogEntryType.Information, "Preparing Code Prefixes is done", null);

            Console.ReadLine();
        }
    }
}

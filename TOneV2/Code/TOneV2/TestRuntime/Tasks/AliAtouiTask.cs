using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestRuntime;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Entities;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.Runtime.Tasks
{
    public class AliAtouiTask : ITask
    {
        #region Variables
        //Dictionaries
        Dictionary<string, CodePrefixInfo> codePrefixes = new Dictionary<string, CodePrefixInfo>();
        Dictionary<string, CodePrefixInfo> pendingCodePrefixes = new Dictionary<string, CodePrefixInfo>();

        //Settings
        int prefixLength;
        int maxPrefixLength;
        int threshold;
        DateTime? effectiveOn;
        bool isFuture;

        //Temporary Variables
        long validNumberPrefix;
        CodePrefixInfo temp_CodePrefixInfo;
        Dictionary<string, CodePrefixInfo> temp_PendingCodePrefixes;
        IEnumerable<string> temp_IEnumerableString;
        #endregion


        #region Public Method
        public void Execute()
        {
            Console.WriteLine("Ali Atoui");

            //Initializint Settings
            SettingManager settingManager = new SettingManager();
            RouteSettingsData settings = settingManager.GetSetting<RouteSettingsData>(Routing.Business.Constants.RouteSettings);
            prefixLength = 1;
            maxPrefixLength = settings.PrepareCodePrefixes.MaxPrefixLength;
            threshold = settings.PrepareCodePrefixes.Threshold;
            effectiveOn = DateTime.Now;
            isFuture = false;


            SupplierCodeManager supplierCodeManager = new SupplierCodeManager();
            IEnumerable<CodePrefixInfo> supplierCodePrefixes = supplierCodeManager.GetDistinctCodeByPrefixes_ByAA(prefixLength, effectiveOn, isFuture);
            AddSupplierCodePrefixes(supplierCodePrefixes);

            SaleCodeManager saleCodeManager = new SaleCodeManager();
            IEnumerable<CodePrefixInfo> saleCodePrefixes = saleCodeManager.GetDistinctCodeByPrefixes(prefixLength, effectiveOn, isFuture);
            AddSaleCodePrefixes(saleCodePrefixes);

            DisplayDictionary(pendingCodePrefixes);

            temp_PendingCodePrefixes = new Dictionary<string, CodePrefixInfo>(pendingCodePrefixes);
            CheckThreshold(temp_PendingCodePrefixes);

            while (pendingCodePrefixes.Count > 0 && prefixLength < maxPrefixLength)
            {
                prefixLength++;

                temp_IEnumerableString = pendingCodePrefixes.Keys;
                pendingCodePrefixes = new Dictionary<string, CodePrefixInfo>();

                supplierCodePrefixes = supplierCodeManager.GetSpecificCodeByPrefixes_ByAA(prefixLength, temp_IEnumerableString, effectiveOn, isFuture);
                AddSupplierCodePrefixes(supplierCodePrefixes);

                saleCodePrefixes = saleCodeManager.GetSpecificCodeByPrefixes_ByAA(prefixLength, temp_IEnumerableString, effectiveOn, isFuture);
                AddSaleCodePrefixes(saleCodePrefixes);

                temp_PendingCodePrefixes = new Dictionary<string, CodePrefixInfo>(pendingCodePrefixes);
                CheckThreshold(temp_PendingCodePrefixes);
            }

            DisplayDictionary(codePrefixes);

            Console.ReadLine();
        }
        #endregion


        #region Private Methods
        void AddSupplierCodePrefixes(IEnumerable<CodePrefixInfo> supplierCodePrefixes)
        {
            if (supplierCodePrefixes != null)
            {
                foreach (CodePrefixInfo item in supplierCodePrefixes)
                    if (long.TryParse(item.CodePrefix, out validNumberPrefix))
                    {
                        pendingCodePrefixes.Add(item.CodePrefix, item);
                    }
                //else
                //    context.WriteTrackingMessage(LogEntryType.Warning, "Invalid Supplier Code Prefix: {0}", item.CodePrefix);
            }
        }

        void AddSaleCodePrefixes(IEnumerable<CodePrefixInfo> saleCodePrefixes)
        {
            if (saleCodePrefixes != null)
            {
                foreach (CodePrefixInfo item in saleCodePrefixes)
                    if (long.TryParse(item.CodePrefix, out validNumberPrefix))
                    {
                        if (pendingCodePrefixes.TryGetValue(item.CodePrefix, out temp_CodePrefixInfo))
                        {
                            temp_CodePrefixInfo.Count += item.Count;
                        }
                        else
                        {
                            pendingCodePrefixes.Add(item.CodePrefix, item);
                        }
                    }
                //else
                //    context.WriteTrackingMessage(LogEntryType.Warning, "Invalid Sale Code Prefix: {0}", item.CodePrefix);
            }
        }

        void CheckThreshold(Dictionary<string, CodePrefixInfo> temp_PendingCodePrefixes)
        {
            foreach (KeyValuePair<string, CodePrefixInfo> item in temp_PendingCodePrefixes)
                if (item.Value.Count <= threshold)
                {
                    codePrefixes.Add(item.Key, item.Value);
                    pendingCodePrefixes.Remove(item.Key);
                }
        }

        void DisplayDictionary(Dictionary<string, CodePrefixInfo> codePrefixes)
        {
            IEnumerable<CodePrefixInfo> _list = codePrefixes.Values.OrderBy(x => x.CodePrefix);

            foreach (CodePrefixInfo item in _list)
                Console.WriteLine(item.CodePrefix + "   " + item.Count);

            Console.WriteLine("\n");
        }
        #endregion
    }
}

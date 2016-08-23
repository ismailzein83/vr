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
using Vanrise.Common;

namespace TOne.WhS.Runtime.Tasks 
{
    public class AliAtouiTask : ITask
    {   
        #region Public Methods 
        public void Execute()
        {
            //PrepareCodePrefixesTask pcp = new PrepareCodePrefixesTask();
            //IEnumerable<CodePrefixInfo> codePrefixesResult = pcp.PrepareCodePrefixes_Main();
            //DisplayList(codePrefixesResult);

            Console.WriteLine("Ali Atoui: VRMailMessageTemplate");

            //VRMailMessageTemplateManager vrMailMessageTemplateManager = new VRMailMessageTemplateManager();
            //VRMailMessageTemplate vrMailMessageTemplate = vrMailMessageTemplateManager.GetMailMessageTemplate(guid);
            //VRMailMessageTypeManager vrMailMessageTypeManager = new VRMailMessageTypeManager();
            //VRMailMessageType vrMailMessageType = vrMailMessageTypeManager.GetMailMessageType(vrMailMessageTemplate.VRMailMessageTypeId);

            //Dictionary<string, dynamic> objects = new Dictionary<string, dynamic>();
            //foreach (var objectVaribale in vrMailMessageType.Settings.Objects)
            //    objects.Add(objectVaribale.Key, objectVaribale.Value);

            Guid guid = new Guid("E487B8FB-398B-4CEC-AB12-0229AAF01A90");
            Carrier carrier = new Carrier() { Id = 100, CustomerId = 101 };

            Dictionary<string, dynamic> objects = new Dictionary<string, dynamic>();
            objects.Add("Carrier-ON", carrier);

            VRMailManager vrMailManager = new VRMailManager();
            vrMailManager.SendMail(guid, objects);

            Console.ReadLine();
        }   
        #endregion

        #region Private Methods
        void DisplayList(IEnumerable<CodePrefixInfo> codePrefixes)
        {
            foreach (CodePrefixInfo item in codePrefixes)
                Console.WriteLine(item.CodePrefix + "   " + item.Count);

            Console.WriteLine("\n");
        }
        #endregion
    }


    public class Carrier
    {
        public int Id {get; set;}

        public int CustomerId { get; set; }
    }



    public class VRMailMessageTemplateTask
    {
        #region Public Method
        public void VRMailMessageTemplate_Main()
        {

        }
        #endregion


        #region Private Methods

        #endregion
    }

    public class PrepareCodePrefixesTask
    {
        #region Public Method
        public IEnumerable<CodePrefixInfo> PrepareCodePrefixes_Main()
        {
            Console.WriteLine("Ali Atoui: PrepareCodePrefixes");

            //Dictionaries
            Dictionary<string, CodePrefixInfo> codePrefixes = new Dictionary<string, CodePrefixInfo>();
            Dictionary<string, CodePrefixInfo> pendingCodePrefixes = new Dictionary<string, CodePrefixInfo>();

            //Initializint Settings
            SettingManager settingManager = new SettingManager();
            RouteSettingsData settings = settingManager.GetSetting<RouteSettingsData>(Routing.Business.Constants.RouteSettings);
            int threshold = settings.SubProcessSettings.CodeRangeCountThreshold;
            int maxPrefixLength = settings.SubProcessSettings.MaxCodePrefixLength;
            int prefixLength = 1;
            DateTime? effectiveOn = DateTime.Now;
            bool isFuture = false;


            SupplierCodeManager supplierCodeManager = new SupplierCodeManager();
            IEnumerable<CodePrefixInfo> supplierCodePrefixes = supplierCodeManager.GetDistinctCodeByPrefixes(prefixLength, effectiveOn, isFuture);
            AddCodePrefixes(supplierCodePrefixes, pendingCodePrefixes);

            SaleCodeManager saleCodeManager = new SaleCodeManager();
            IEnumerable<CodePrefixInfo> saleCodePrefixes = saleCodeManager.GetDistinctCodeByPrefixes(prefixLength, effectiveOn, isFuture);
            AddCodePrefixes(saleCodePrefixes, pendingCodePrefixes);

            DisplayDictionary(pendingCodePrefixes);

            if (maxPrefixLength == 1)
                return pendingCodePrefixes.Values.OrderByDescending(x => x.Count);

            CheckThreshold(pendingCodePrefixes, codePrefixes, threshold);

            while (pendingCodePrefixes.Count > 0 && prefixLength < maxPrefixLength)
            {
                prefixLength++;

                IEnumerable<string> _pendingCodePrefixes = pendingCodePrefixes.Keys;
                pendingCodePrefixes = new Dictionary<string, CodePrefixInfo>();

                supplierCodePrefixes = supplierCodeManager.GetSpecificCodeByPrefixes(prefixLength, _pendingCodePrefixes, effectiveOn, isFuture);
                AddCodePrefixes(supplierCodePrefixes, pendingCodePrefixes);

                saleCodePrefixes = saleCodeManager.GetSpecificCodeByPrefixes(prefixLength, _pendingCodePrefixes, effectiveOn, isFuture);
                AddCodePrefixes(saleCodePrefixes, pendingCodePrefixes);

                CheckThreshold(pendingCodePrefixes, codePrefixes, threshold);
            }

            if (pendingCodePrefixes.Count > 0)
                foreach (KeyValuePair<string, CodePrefixInfo> item in pendingCodePrefixes)
                    codePrefixes.Add(item.Key, item.Value);

            DisplayDictionary(codePrefixes);

            return codePrefixes.Values.OrderByDescending(x => x.Count);
        }
        #endregion


        #region Private Methods

        void AddCodePrefixes(IEnumerable<CodePrefixInfo> codePrefixes, Dictionary<string, CodePrefixInfo> pendingCodePrefixes)
        {
            long _validNumberPrefix;
            CodePrefixInfo _codePrefixInfo;

            if (codePrefixes != null)
            {
                foreach (CodePrefixInfo item in codePrefixes)
                    if (long.TryParse(item.CodePrefix, out _validNumberPrefix))
                    {
                        if (pendingCodePrefixes.TryGetValue(item.CodePrefix, out _codePrefixInfo))
                        {
                            _codePrefixInfo.Count += item.Count;
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

        void CheckThreshold(Dictionary<string, CodePrefixInfo> pendingCodePrefixes, Dictionary<string, CodePrefixInfo> codePrefixes, int threshold)
        {
            Dictionary<string, CodePrefixInfo> _pendingCodePrefixes = new Dictionary<string, CodePrefixInfo>(pendingCodePrefixes);
            foreach (KeyValuePair<string, CodePrefixInfo> item in _pendingCodePrefixes)
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

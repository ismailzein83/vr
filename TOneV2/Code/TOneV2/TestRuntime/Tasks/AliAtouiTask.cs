using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TestRuntime;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.RouteSync.Entities;
using TOne.WhS.RouteSync.TelesIdb;
using TOne.WhS.Routing.Data.SQL;
using TOne.WhS.Routing.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Caching.Runtime;
using Vanrise.Common.Business;
using Vanrise.Integration.Entities;
using Vanrise.Integration.Mappers;
using Vanrise.Queueing;
using Vanrise.Rules.Normalization;
using Vanrise.Runtime;
using Vanrise.Runtime.Entities;

namespace TOne.WhS.Runtime.Tasks
{
    public class AliAtouiTask : ITask
    {
        #region Public Methods

        public void Execute()
        {
            #region NormalizationRule RemoveAction
            //NormalizationRuleRemoveActionTask normalizationRuleRemoveActionTask = new NormalizationRuleRemoveActionTask();
            //normalizationRuleRemoveActionTask.NormalizationRuleRemoveActionTask_Main();
            #endregion

            #region TelesIdbSWSyncTask
            //TelesIdbSWSyncTask telesIdbSWSyncTask = new TelesIdbSWSyncTask();
            //telesIdbSWSyncTask.TelesIdbSWSync_Main();
            #endregion

            #region DeserializeTask
            DeserializeTask deserializeTask = new DeserializeTask();
            deserializeTask.DeserializeTask_Main();
            #endregion

            #region VRMailMessageTemplateTask
            //VRMailMessageTemplateTask vrMailMessageTemplateTask = new VRMailMessageTemplateTask();
            //vrMailMessageTemplateTask.VRMailMessageTemplate_Main();
            #endregion

            #region PrepareCodePrefixesTask
            //PrepareCodePrefixesTask prepareCodePrefixesTask = new PrepareCodePrefixesTask();
            //IEnumerable<CodePrefixInfo> codePrefixesResult = prepareCodePrefixesTask.PrepareCodePrefixes_Main();
            //DisplayList(codePrefixesResult);
            #endregion

            #region Runtime
            //ExecuteRuntime executeRuntime = new ExecuteRuntime();
            //executeRuntime.Runtime_Main();
            #endregion
        }

        public static MappingOutput DataSourceMapData(IImportedData data, MappedBatchItemsToEnqueue mappedBatches)
        {
            var cdrs = new List<dynamic>();
            var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();
            Type cdrRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType("CDR");

            long startingId;
            int batchSize = 50000;
            var dataRecordVanriseType = new Vanrise.GenericData.Entities.DataRecordVanriseType("CDR");

            Vanrise.Common.Business.IDManager.Instance.ReserveIDRange(dataRecordVanriseType, batchSize, out startingId);

            var importedData = ((Vanrise.Integration.Entities.DBReaderImportedData)(data));

            IDataReader reader = importedData.Reader;

            long currentCDRId = startingId;
            int rowCount = 0;
            while (reader.Read())
            {
                dynamic cdr = Activator.CreateInstance(cdrRuntimeType) as dynamic;
                cdr.Id = currentCDRId;
                cdr.SwitchId = 83;
                cdr.IDonSwitch = Utils.GetReaderValue<long>(reader, "IDonSwitch");
                cdr.Tag = reader["Tag"] as string;
                cdr.AttemptDateTime = (DateTime)reader["AttemptDateTime"];
                cdr.AlertDateTime = Utils.GetReaderValue<DateTime>(reader, "AlertDateTime");
                cdr.ConnectDateTime = Utils.GetReaderValue<DateTime>(reader, "ConnectDateTime");
                cdr.DisconnectDateTime = Utils.GetReaderValue<DateTime>(reader, "DisconnectDateTime");
                cdr.DurationInSeconds = Utils.GetReaderValue<Decimal>(reader, "DurationInSeconds");
                cdr.InTrunk = reader["IN_TRUNK"] as string;
                cdr.InCircuit = reader["IN_CIRCUIT"] != DBNull.Value ? Convert.ToInt64(reader["IN_CIRCUIT"]) : default(Int64);
                cdr.InCarrier = reader["IN_CARRIER"] as string;
                cdr.InIP = reader["IN_IP"] as string;
                cdr.OutTrunk = reader["OUT_TRUNK"] as string;
                cdr.OutCircuit = reader["OUT_CIRCUIT"] != DBNull.Value ? Convert.ToInt64(reader["OUT_CIRCUIT"]) : default(Int64);
                cdr.OutCarrier = reader["OUT_CARRIER"] as string;
                cdr.OutIP = reader["OUT_IP"] as string;

                cdr.CGPN = reader["CGPN"] as string;
                cdr.CDPN = reader["CDPN"] as string;
                cdr.CauseFromReleaseCode = reader["CAUSE_FROM_RELEASE_CODE"] as string;
                cdr.CauseFrom = reader["CAUSE_FROM"] as string;
                cdr.CauseToReleaseCode = reader["CAUSE_TO_RELEASE_CODE"] as string;
                cdr.CauseTo = reader["CAUSE_TO"] as string;
                cdr.IsRerouted = reader["IsRerouted"] != DBNull.Value ? ((reader["IsRerouted"] as string) == "Y") : false;
                cdr.CDPNOut = reader["CDPNOut"] as string;
                cdr.CDPNIn = reader["CDPNIn"] as string;
                cdr.SIP = reader["SIP"] as string;

                cdrs.Add(cdr);
                importedData.LastImportedId = reader["CDRID"];

                currentCDRId++;
                rowCount++;
                if (rowCount == batchSize)
                    break;

            }
            if (cdrs.Count > 0)
            {
                var batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(cdrs, "#RECORDSCOUNT# of Raw CDRs", "CDR");
                mappedBatches.Add("Distribute Raw CDRs Stage", batch);
            }
            else
                importedData.IsEmpty = true;

            Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();
            result.Result = Vanrise.Integration.Entities.MappingResult.Valid;

            return result;
        }

        #endregion

        #region Private Methods

        private void DisplayList(IEnumerable<CodePrefixInfo> codePrefixes)
        {
            foreach (CodePrefixInfo item in codePrefixes)
                Console.WriteLine(item.CodePrefix + "   " + item.Count);

            Console.WriteLine("\n");
        }

        private static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        #endregion
    }

    public class NormalizationRuleRemoveActionTask
    {
        public void NormalizationRuleRemoveActionTask_Main()
        {
            NormalizeNumberTarget target = new NormalizeNumberTarget() { PhoneNumber = "abc123acc145abc" }; //"abc123abc145"

            var removeActionSettings = new Vanrise.Rules.Normalization.MainExtensions.RemoveActionSettings();
            removeActionSettings.TextToRemove = "abc";
            removeActionSettings.IncludingText = false;
            removeActionSettings.TextOccurrence = Vanrise.Rules.Normalization.MainExtensions.TextOccurrence.FirstOccurrence;
            removeActionSettings.RemoveDirection = Vanrise.Rules.Normalization.MainExtensions.RemoveDirection.Before;
            removeActionSettings.Execute(null, target);

            string normalizedPhoneNB = target.PhoneNumber;
        }
    }

    public class TelesIdbSWSyncTask
    {
        public void TelesIdbSWSync_Main()
        {
            CarrierMapping carrierMapping1 = new CarrierMapping() { CarrierId = 1, SupplierMapping = new List<string>() { "C001", "C002" } };
            CarrierMapping carrierMapping2 = new CarrierMapping() { CarrierId = 1, SupplierMapping = new List<string>() { "C003" } };
            CarrierMapping carrierMapping3 = new CarrierMapping() { CarrierId = 1, SupplierMapping = new List<string>() { "C005", "C006", "C007" } };
            CarrierMapping carrierMapping4 = new CarrierMapping() { CarrierId = 1, SupplierMapping = new List<string>() { "C008" } };
            CarrierMapping carrierMapping5 = new CarrierMapping() { CarrierId = 1, SupplierMapping = new List<string>() { "C009" } };

            Dictionary<string, CarrierMapping> carrierMappings = new Dictionary<string, CarrierMapping>();
            carrierMappings.Add("1", carrierMapping1);
            carrierMappings.Add("2", carrierMapping2);
            carrierMappings.Add("3", carrierMapping3); carrierMappings.Add("4", carrierMapping4); carrierMappings.Add("5", carrierMapping5);

            var routeOption1 = new RouteSync.Entities.RouteOption() { SupplierId = "1", SupplierRate = 2, IsBlocked = false, NumberOfTries = 2, Percentage = 30 };
            var routeOption2 = new RouteSync.Entities.RouteOption() { SupplierId = "2", SupplierRate = 2, IsBlocked = false, NumberOfTries = 2, Percentage = null };
            var routeOption3 = new RouteSync.Entities.RouteOption() { SupplierId = "3", SupplierRate = 2, IsBlocked = false, NumberOfTries = 2, Percentage = 50 };
            var routeOption4 = new RouteSync.Entities.RouteOption() { SupplierId = "4", SupplierRate = 2, IsBlocked = false, NumberOfTries = 2, Percentage = 10 };
            var routeOption5 = new RouteSync.Entities.RouteOption() { SupplierId = "5", SupplierRate = 2, IsBlocked = false, NumberOfTries = 2, Percentage = 10 };

            Route route = new Route() { CustomerId = "1", SaleZoneId = 2, Code = "3", SaleRate = 4, Options = new List<RouteSync.Entities.RouteOption>() };
            route.Options.Add(routeOption1); route.Options.Add(routeOption2); route.Options.Add(routeOption3); route.Options.Add(routeOption4); route.Options.Add(routeOption5);

            TelesIdbSWSync telesIdbSWSync = new TelesIdbSWSync();
            telesIdbSWSync.NumberOfMappings = 2;
            telesIdbSWSync.NumberOfOptions = 5;
            telesIdbSWSync.SupplierOptionsSeparator = "|";
            telesIdbSWSync.CarrierMappings = carrierMappings;

            //string optionsAsString = telesIdbSWSync.BuildOptions(route, null, telesIdbSWSync.SupplierOptionsSeparator);
        }
    }

    public class DeserializeTask
    {
        CustomerRouteDataManager customerRouteDataManager = new CustomerRouteDataManager();
        RPRouteDataManager rpRouteDataManager = new RPRouteDataManager();
        CodeMatchesDataManager codeMatchesDataManager = new CodeMatchesDataManager();

        #region Public Methods

        public void DeserializeTask_Main()
        {
            //CustomerRoutes
            //string serializedCustomerRouteOptions = "88011~~~9664~~|8801~~~11457~~|88011~~~13620~~|88011~~~15414~~|88011~~~21776~~";
            //string customerRouteOptionsAsJSON = this.DeserializeCustomerRouteOptions(serializedCustomerRouteOptions);

            //RoutingProductRoutes
            //string serializedRPOptionsDetailsBySupplier = "240~315094$0.01540000$1$$False$2515165#315095$0.01540000$1$$False$2515166#315096$0.01540000$1$$False$2515167#315091$0.01540000$1$$False$2515162#315092$0.01540000$1$$False$2515163#315090$0.01540000$1$$False$2515161~0~6~~1|564~322849$0.02250000$1$$False$2522986#322852$0.02250000$1$$False$2522989~0~2~~1|57~2712$0.03390000$1@2@5$$False$29107#2717$0.03390000$1@2@5$$False$29112#2718$0.03390000$1@2@5$$False$29113#2713$0.03390000$1@2@5$$False$29108#2714$0.03390000$1@2@5$$False$29109#2715$0.03390000$1@2@5$$False$29110~0~6~~20|66~9660$0.02600000$7@4$$False$115805#9664$0.03000000$7@4$$False$115803#9665$0.03000000$7@4$$False$115804~0~3~~79|559~315422$0.02250000$1$$False$2515559#315425$0.02250000$1$$False$2515562~0~2~~1|88~30370$0.02700000$7@4$$False$395295~0~1~~79|527~329529$0.01970000$4$$False$2529781#329528$0.02240000$4$$False$2529780~0~2~~15|60~7255$0.03390000$7@4$$False$86520#7261$0.03390000$7@4$$False$86526#7256$0.03390000$7@4$$False$86521#7260$0.03390000$7@4$$False$86525#7258$0.03390000$7@4$$False$86523#7257$0.03390000$7@4$$False$86522~0~6~~79|65~9536$0.38350000$7@4$$False$114463#9531$0.38350000$7@4$$False$114458#9533$0.38350000$7@4$$False$114460#9534$0.38350000$7@4$$False$114461#9535$0.38350000$7@4$$False$114462#9530$0.38350000$7@4$$False$114457~0~6~~79|70~15411$0.02700000$7@4$$False$187067#15414$0.04130000$7@4$$False$187074~0~2~~79|561~319138$0.02250000$1$$False$2519275#319141$0.02250000$1$$False$2519278~0~2~~1|69~13620$0.03390000$7@4$$False$164003#13622$0.03390000$7@4$$False$164005#13617$0.03390000$7@4$$False$164000#13623$0.03390000$7@4$$False$164006#13618$0.03390000$7@4$$False$164001#13619$0.03390000$7@4$$False$164002~0~6~~79|563~321615$0.02250000$1$$False$2521752#321612$0.02250000$1$$False$2521749~0~2~~1|145~331901$0.01680000$7@4$$False$2532162~0~1~~79|562~320378$0.02250000$1$$False$2520515#320375$0.02250000$1$$False$2520512~0~2~~1|558~316664$0.02250000$1$$False$2516801#316667$0.02250000$1$$False$2516804~0~2~~1|75~21776$0.01950000$1@2$$False$280981#21775$0.02150000$1@2$$False$280982~0~2~~4|560~317904$0.02250000$1$$False$2518041#317901$0.02250000$1$$False$2518038~0~2~~1|67~11457$0.03000000$7@4$$False$137632#11458$0.03000000$7@4$$False$137633#11459$0.03000000$7@4$$False$137634~0~3~~79";
            //string rpOptionsDetailsBySupplierAsJSON = this.DeserializeOptionsDetailsBySupplier(serializedRPOptionsDetailsBySupplier);

            //string serializedRPOptionsByPolicy = "6d584c11-ce52-4385-a871-3b59505d0f57~57$0.03390000$$111.00000000$10813$0$20$False#65$0.38350000$$111.00000000$10813$0$79$False#66$0.0286666666666666666666666667$$111.00000000$10813$0$79$False#67$0.03000000$$111.00000000$10813$0$79$False#69$0.03390000$$111.00000000$10813$0$79$False#70$0.03415000$$111.00000000$10813$0$79$False#75$0.02050000$$111.00000000$10813$0$4$False#88$0.02700000$$111.00000000$10813$0$79$False#145$0.01680000$$111.00000000$10813$0$79$False#240$0.01540000$$111.00000000$10813$0$1$False#527$0.02105000$$111.00000000$10813$0$15$False#558$0.02250000$$111.00000000$10813$0$1$False#559$0.02250000$$111.00000000$10813$0$1$False#560$0.02250000$$111.00000000$10813$0$1$False#562$0.02250000$$111.00000000$10813$0$1$False#563$0.02250000$$111.00000000$10813$0$1$False#564$0.02250000$$111.00000000$10813$0$1$False#60$0.03390000$$45.00000000$10813$0$79$False#561$0.02250000$$6.00000000$10813$0$1$False|cb8cc5ed-afda-4ed7-882d-1377666c141e~57$0.03390000$$111.00000000$10813$0$20$False#65$0.38350000$$111.00000000$10813$0$79$False#66$0.03000000$$111.00000000$10813$0$79$False#67$0.03000000$$111.00000000$10813$0$79$False#69$0.03390000$$111.00000000$10813$0$79$False#70$0.04130000$$111.00000000$10813$0$79$False#75$0.02150000$$111.00000000$10813$0$4$False#88$0.02700000$$111.00000000$10813$0$79$False#145$0.01680000$$111.00000000$10813$0$79$False#240$0.01540000$$111.00000000$10813$0$1$False#527$0.02240000$$111.00000000$10813$0$15$False#558$0.02250000$$111.00000000$10813$0$1$False#559$0.02250000$$111.00000000$10813$0$1$False#560$0.02250000$$111.00000000$10813$0$1$False#562$0.02250000$$111.00000000$10813$0$1$False#563$0.02250000$$111.00000000$10813$0$1$False#564$0.02250000$$111.00000000$10813$0$1$False#60$0.03390000$$45.00000000$10813$0$79$False#561$0.02250000$$6.00000000$10813$0$1$False|e85f9e2f-1ce6-4cc3-9df9-b664e63826f5~57$0.03390000$$111.00000000$10813$0$20$False#65$0.38350000$$111.00000000$10813$0$79$False#66$0.02600000$$111.00000000$10813$0$79$False#67$0.03000000$$111.00000000$10813$0$79$False#69$0.03390000$$111.00000000$10813$0$79$False#70$0.02700000$$111.00000000$10813$0$79$False#75$0.01950000$$111.00000000$10813$0$4$False#88$0.02700000$$111.00000000$10813$0$79$False#145$0.01680000$$111.00000000$10813$0$79$False#240$0.01540000$$111.00000000$10813$0$1$False#527$0.01970000$$111.00000000$10813$0$15$False#558$0.02250000$$111.00000000$10813$0$1$False#559$0.02250000$$111.00000000$10813$0$1$False#560$0.02250000$$111.00000000$10813$0$1$False#562$0.02250000$$111.00000000$10813$0$1$False#563$0.02250000$$111.00000000$10813$0$1$False#564$0.02250000$$111.00000000$10813$0$1$False#60$0.03390000$$45.00000000$10813$0$79$False#561$0.02250000$$6.00000000$10813$0$1$False";
            //string rpOptionsByPolicyAsJSON = this.DeserializeOptionsByPolicy(serializedRPOptionsByPolicy);

            //string serializedSupplierCodeMatchesWithRate = "88$31089$48$~0.01690000~7#4#8#3~7#4~79~396015~|66$10765$48$~0.01230000~7#4#8#3~7#4~79~130519~|68$12438$48$~0.01230000~7#4#8#3~7#4~79~151274~|57$5292$48$~0.01050000~1#6#7#5#4#8#3#2#9~1#2#5~20~63520~|60$9110$48$~0.01050000~7#4#8#3~7#4~79~110510~|69$14923$48$~0.01050000~7#4#8#3~7#4~79~182340~|70$16037$48$~0.00980000~7#4#8#3~7#4~79~204973~|75$22505$48$~0.00950000~1#6#7#5#4#8#3#2#9~1#2~4~299772~";
            //string supplierodCodeMatchesWithRateAsJSON = this.DeserializeSupplierZodeMatchesWithRate(serializedSupplierCodeMatchesWithRate); 
        }

        #endregion

        #region Private Methods

        //private string DeserializeCustomerRouteOptions(string serializedOptions)
        //{
        //    List<TOne.WhS.Routing.Entities.RouteOption> routeOptions = customerRouteDataManager.DeserializeOptions(serializedOptions);
        //    return Vanrise.Common.Serializer.Serialize(routeOptions, true);
        //}

        private string DeserializeOptionsDetailsBySupplier(string serializedOptionsDetailsBySupplier)
        {
            Dictionary<int, RPRouteOptionSupplier> optionsDetailsBySupplier = rpRouteDataManager.DeserializeOptionsDetailsBySupplier(serializedOptionsDetailsBySupplier);
            return Vanrise.Common.Serializer.Serialize(optionsDetailsBySupplier, true);
        }

        private string DeserializeOptionsByPolicy(string serializedOptionsByPolicy)
        {
            Dictionary<Guid, IEnumerable<RPRouteOption>> optionsByPolicy = rpRouteDataManager.DeserializeOptionsByPolicy(serializedOptionsByPolicy);
            return Vanrise.Common.Serializer.Serialize(optionsByPolicy, true);
        }

        //private string DeserializeSupplierZodeMatchesWithRate(string serializedSupplierZodeMatchesWithRate)
        //{
        //    List<SupplierCodeMatchWithRate> supplierCodeMatchWithRate = codeMatchesDataManager.DeserializeSupplierCodeMatches(serializedSupplierZodeMatchesWithRate);
        //    return Vanrise.Common.Serializer.Serialize(supplierCodeMatchWithRate, true);
        //}

        #endregion
    }

    public class VRMailMessageTemplateTask
    {
        #region Public Method

        public void VRMailMessageTemplate_Main()
        {
            Console.WriteLine("Ali Atoui: VRMailMessageTemplate");

            Guid guid = new Guid("E21CD125-61F0-4091-A03E-200CFE33F6E3");
            Carrier carrier = new Carrier() { Id = 100, CustomerId = 101 };
            User user = new User() { Email = "aatoui@vanrise.com", Name = "Ali Atoui" };

            Dictionary<string, dynamic> objects = new Dictionary<string, dynamic>();
            objects.Add("Carrier-ON", carrier);
            objects.Add("AliAtoui-ON", user);

            VRMailManager vrMailManager = new VRMailManager();
            vrMailManager.SendMail(guid, objects);

            Console.ReadLine();
        }

        #endregion

        #region Private Classes

        private class Carrier
        {
            public int Id { get; set; }

            public int CustomerId { get; set; }
        }

        private class User
        {
            public string Email { get; set; }
            public string Name { get; set; }
        }

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

    public class ExecuteRuntime
    {
        public void Runtime_Main()
        {
            var runtimeServices = new List<RuntimeService>();

            SchedulerService schedulerService = new SchedulerService() { Interval = new TimeSpan(0, 0, 1) };
            runtimeServices.Add(schedulerService);

            BPRegulatorRuntimeService bpRegulatorRuntimeService = new BPRegulatorRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bpRegulatorRuntimeService);

            BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bpService);

            QueueRegulatorRuntimeService queueRegulatorService = new QueueRegulatorRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(queueRegulatorService);

            QueueActivationRuntimeService queueActivationService = new QueueActivationRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(queueActivationService);

            SummaryQueueActivationRuntimeService summaryQueueActivationService = new SummaryQueueActivationRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(summaryQueueActivationService);

            Vanrise.Integration.Business.DataSourceRuntimeService dsRuntimeService = new Vanrise.Integration.Business.DataSourceRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(dsRuntimeService);

            Vanrise.Common.Business.BigDataRuntimeService bigDataService = new Vanrise.Common.Business.BigDataRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bigDataService);

            CachingRuntimeService cachingRuntimeService = new CachingRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(cachingRuntimeService);

            CachingDistributorRuntimeService cachingDistributorRuntimeService = new CachingDistributorRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(cachingDistributorRuntimeService);

            DataGroupingExecutorRuntimeService dataGroupingExecutorRuntimeService = new Vanrise.Common.Business.DataGroupingExecutorRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(dataGroupingExecutorRuntimeService);

            DataGroupingDistributorRuntimeService dataGroupingDistributorRuntimeService = new Vanrise.Common.Business.DataGroupingDistributorRuntimeService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(dataGroupingDistributorRuntimeService);

            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();

            Console.ReadKey();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Entities;
using TOne.WhS.RouteSync.Entities;
using TOne.WhS.RouteSync.Idb;
using TOne.WhS.RouteSync.TelesIdb;
using TOne.WhS.RouteSync.TelesIdb.Postgres;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.MainExtensions.GenericRuleCriteriaFieldValues;
using Vanrise.GenericData.Transformation.Entities;

namespace TOne.WhS.DBSync.Business
{
    public class TelesSwitchMigrationParser : SwitchMigrationParser
    {
        private string _configuration;

        public TelesSwitchMigrationParser(string configuration)
        {
            _configuration = configuration;
        }

        public override SwitchData GetSwitchData(MigrationContext context, int switchId, Dictionary<string, CarrierAccount> allCarrierAccounts)
        {
            return ReadXml(context, switchId, allCarrierAccounts);
        }

        private SwitchData ReadXml(MigrationContext context, int switchId, Dictionary<string, CarrierAccount> allCarrierAccounts)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(_configuration);
            if (xml.DocumentElement == null)
                return null;

            XmlNode parametersNode = xml.DocumentElement.SelectSingleNode("Parameters");
            XmlNode carrierMappingsNode = xml.DocumentElement.SelectSingleNode("CarrierMapping");

            TelesCarrierMapping carrierMappings = BuildCarrierMapping(carrierMappingsNode, context, allCarrierAccounts);

            TelesIdbSWSync synchroniser = new TelesIdbSWSync
            {
                CarrierMappings = carrierMappings != null ? carrierMappings.RoutingMapping : null,
                MappingSeparator = ";",
                NumberOfOptions = GetNumberOfOptions(parametersNode),
                SupplierOptionsSeparator = GetSupplierOptionsSeparator(parametersNode),
                NumberOfMappings = CheckUseTwoSuppliersMapping(parametersNode) ? 2 : default(int?)
            };

            string connectionString;
            string redundantConnectionString;
            GetConnectionStrings(parametersNode, out connectionString, out redundantConnectionString);

            string schemaName = GetSchemaName(parametersNode);

            if (!string.IsNullOrEmpty(connectionString))
            {
                synchroniser.DataManager = new IdbPostgresDataManager()
                {
                    ConnectionString = GetIdbConnectionString(schemaName, connectionString),
                    RedundantConnectionStrings = !string.IsNullOrEmpty(redundantConnectionString) ? new List<IdbConnectionString>() { GetIdbConnectionString(schemaName, redundantConnectionString) } : null
                };
            }

            var isSwitchRouteSynchronizerValidContext = new TOne.WhS.RouteSync.Entities.IsSwitchRouteSynchronizerValidContext();
            bool isSwitchValid = synchroniser.IsSwitchRouteSynchronizerValid(isSwitchRouteSynchronizerValidContext);
            if (!isSwitchValid)
                throw new VRBusinessException(string.Join(" - ", isSwitchRouteSynchronizerValidContext.ValidationMessages));

            List<MappingRule> mappingRules = null;
            if (carrierMappings != null && carrierMappings.CDRMapping != null)
                mappingRules = BuildMapingRules(switchId, carrierMappings.CDRMapping, context.EffectiveAfterDate);

            return new SwitchData
            {
                MappingRules = mappingRules,
                SwitchRouteSynchronizer = synchroniser
            };
        }

        private TelesCarrierMapping BuildCarrierMapping(XmlNode carrierMappingsNode, MigrationContext context, Dictionary<string, CarrierAccount> allCarrierAccounts)
        {
            if (carrierMappingsNode == null)
                return null;

            var routingMappings = new Dictionary<string, CarrierMapping>();
            var CDRMappings = new Dictionary<string, CarrierMapping>();

            foreach (XmlNode carrierMappingNode in carrierMappingsNode.ChildNodes)
            {
                if (carrierMappingNode == null || carrierMappingNode.Attributes == null)
                    continue;

                XmlNode carrierAccountIdNode = carrierMappingNode.Attributes["CarrierAccountID"];
                if (carrierAccountIdNode == null)
                    continue;

                string carrierAccountId = carrierAccountIdNode.InnerText;

                CarrierAccount carrier;
                if (!allCarrierAccounts.TryGetValue(carrierAccountId, out carrier))
                {
                    context.WriteWarning(string.Format("Carrier Account ID {0} doesn't exist in Carrier Accounts", carrierAccountId));
                    continue;
                }

                XmlNode inNode = carrierMappingNode.Attributes["In"];
                XmlNode outNode = carrierMappingNode.Attributes["Out"];

                string inTrunk = inNode != null ? inNode.InnerText : null;
                string outTrunk = outNode != null ? outNode.InnerText : null;

                Gateway inGateways = BuildGatewayList(inTrunk);
                Gateway outGateways = BuildGatewayList(outTrunk);

                CarrierMapping routingCarrierMapping = new CarrierMapping
                {
                    CarrierId = carrier.CarrierAccountId,
                    CustomerMapping = (carrier.AccountType == CarrierAccountType.Customer || carrier.AccountType == CarrierAccountType.Exchange) ? (inGateways.RoutingGateways.Any() ? inGateways.RoutingGateways : null) : null,
                    SupplierMapping = (carrier.AccountType == CarrierAccountType.Supplier || carrier.AccountType == CarrierAccountType.Exchange) ? (outGateways.RoutingGateways.Any() ? outGateways.RoutingGateways : null) : null
                };
                routingMappings.Add(routingCarrierMapping.CarrierId.ToString(), routingCarrierMapping);

                // to dictionnaries are added to prevent changes from route synch
                CarrierMapping CDRcarrierMapping = new CarrierMapping
                {
                    CarrierId = carrier.CarrierAccountId,
                    CustomerMapping = (carrier.AccountType == CarrierAccountType.Customer || carrier.AccountType == CarrierAccountType.Exchange) ? (inGateways.CDRGateways.Any() ? inGateways.CDRGateways : null) : null,
                    SupplierMapping = (carrier.AccountType == CarrierAccountType.Supplier || carrier.AccountType == CarrierAccountType.Exchange) ? (outGateways.CDRGateways.Any() ? outGateways.CDRGateways : null) : null
                };
                CDRMappings.Add(CDRcarrierMapping.CarrierId.ToString(), CDRcarrierMapping);
            }
            return new TelesCarrierMapping
            {
                CDRMapping = CDRMappings,
                RoutingMapping = routingMappings
            };
        }

        private Gateway BuildGatewayList(string trunk)
        {
            var gatewayObject = new Gateway
            {
                CDRGateways = new List<string>(),
                RoutingGateways = new List<string>()
            };

            if (string.IsNullOrEmpty(trunk))
                return gatewayObject;

            string[] trunks = trunk.Split(';');

            foreach (string splittedTrunk in trunks)
            {
                if (string.IsNullOrEmpty(splittedTrunk))
                    continue;

                string[] result = splittedTrunk.Split(':');

                int resultCount = result.Count();
                if (resultCount < 2)
                    continue;

                string mapping = result[0];
                string process = result[resultCount - 1];
                if (string.IsNullOrEmpty(mapping) || string.IsNullOrEmpty(process))
                    continue;

                if (process.ToUpper().Contains("RT"))
                    gatewayObject.RoutingGateways.Add(mapping);

                if (process.ToUpper().Contains("CDR"))
                    gatewayObject.CDRGateways.Add(mapping);
            }
            return gatewayObject;
        }

        private int GetNumberOfOptions(XmlNode parametersNode)
        {
            XmlNode maximumSuppliersNumberNode = GetXmlNodeByParameterName(parametersNode, "$Routing_Maximum_Options");

            int maximumSuppliersNumber;
            if (maximumSuppliersNumberNode == null || !int.TryParse(maximumSuppliersNumberNode.FirstChild.Value, out maximumSuppliersNumber))
                return 6;

            return maximumSuppliersNumber;
        }

        private string GetSupplierOptionsSeparator(XmlNode parametersNode)
        {
            XmlNode supplierOptionsSeparatorNode = GetXmlNodeByParameterName(parametersNode, "$Use_Separator");
            if (supplierOptionsSeparatorNode == null)
                return default(string);

            return supplierOptionsSeparatorNode.FirstChild.Value;
        }

        private bool CheckUseTwoSuppliersMapping(XmlNode parametersNode)
        {
            XmlNode useTwoSuppliersMappingsNode = GetXmlNodeByParameterName(parametersNode, "$Use_Two_Suppliers_Mappings");
            if (useTwoSuppliersMappingsNode == null)
                return false;

            if (string.Compare(useTwoSuppliersMappingsNode.FirstChild.Value, "yes", true) != 0)
                return false;

            return true;
        }

        private void GetConnectionStrings(XmlNode parametersNode, out string connectionString, out string redundantConnectionString)
        {
            connectionString = null;
            redundantConnectionString = null;

            XmlNode connectionStringsNode = GetXmlNodeByParameterName(parametersNode, "$Direct_Mode_ConnectionString");
            if (connectionStringsNode == null)
                return;

            string[] connectionStrings = connectionStringsNode.FirstChild.Value.Split('|');
            connectionString = connectionStrings[0];

            if (connectionStrings.Count() > 1)
                redundantConnectionString = connectionStrings[1];
        }

        private string GetSchemaName(XmlNode parametersNode)
        {
            XmlNode schemaNameNode = GetXmlNodeByParameterName(parametersNode, "$schemaName");
            if (schemaNameNode == null)
                return null;

            return schemaNameNode.FirstChild.Value;
        }

        private IdbConnectionString GetIdbConnectionString(string schemaName, string connectionString)
        {
            return new IdbConnectionString() { SchemaName = schemaName, ConnectionString = connectionString };
        }

        private XmlNode GetXmlNodeByParameterName(XmlNode parametersNode, string name)
        {
            return parametersNode.SelectSingleNode("Parameter[@Name='" + name + "']");
        }

        private List<MappingRule> BuildMapingRules(int switchId, Dictionary<string, CarrierMapping> carrierMappings, DateTime? processDate)
        {
            if (!carrierMappings.Any()) return null;

            var mappingRules = new List<MappingRule>();

            foreach (var carrierMapping in carrierMappings.Values)
            {
                var inTrunks = carrierMapping.CustomerMapping;
                var outTrunks = carrierMapping.SupplierMapping;

                if (inTrunks != null)
                {
                    StaticValues inStaticValues = new StaticValues
                    {
                        Values = inTrunks.Cast<Object>().ToList()
                    };
                    mappingRules.Add(MappingRuleGenerator.GetRule(carrierMapping.CarrierId, inStaticValues, switchId, processDate, 1));
                }
                if (outTrunks != null)
                {
                    StaticValues outStaticValues = new StaticValues
                    {
                        Values = outTrunks.Cast<Object>().ToList()
                    };
                    mappingRules.Add(MappingRuleGenerator.GetRule(carrierMapping.CarrierId, outStaticValues, switchId, processDate, 2));
                }
            }
            return mappingRules;
        }
    }

    public class TelesCarrierMapping
    {
        public Dictionary<string, CarrierMapping> RoutingMapping { get; set; }
        public Dictionary<string, CarrierMapping> CDRMapping { get; set; }
    }

    public class Gateway
    {
        public List<string> RoutingGateways { get; set; }
        public List<string> CDRGateways { get; set; }
    }
}

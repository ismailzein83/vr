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

namespace TOne.WhS.DBSync.Business
{
    public class TelesSwitchMigrationParser : SwitchMigrationParser
    {
        private string _configuration;

        public TelesSwitchMigrationParser(string configuration)
        {
            _configuration = configuration;
        }

        public override SwitchRouteSynchronizer GetSwitchRouteSynchronizer(MigrationContext context, Dictionary<string, CarrierAccount> allCarrierAccounts)
        {
            return ReadXml(context, allCarrierAccounts);
        }

        private TelesIdbSWSync ReadXml(MigrationContext context, Dictionary<string, CarrierAccount> allCarrierAccounts)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(_configuration);
            if (xml.DocumentElement == null)
                return null;

            XmlNode parametersNode = xml.DocumentElement.SelectSingleNode("Parameters");
            XmlNode carrierMappingsNode = xml.DocumentElement.SelectSingleNode("CarrierMapping");

            TelesIdbSWSync synchroniser = new TelesIdbSWSync()
            {
                CarrierMappings = BuildCarrierMapping(carrierMappingsNode, context, allCarrierAccounts),
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

            return synchroniser;
        }



        private Dictionary<string, CarrierMapping> BuildCarrierMapping(XmlNode carrierMappingsNode, MigrationContext context, Dictionary<string, CarrierAccount> allCarrierAccounts)
        {
            if (carrierMappingsNode == null)
                return null;

            Dictionary<string, CarrierMapping> mappings = new Dictionary<string, CarrierMapping>();

            foreach (XmlNode carrierMappingNode in carrierMappingsNode.ChildNodes)
            {
                if (carrierMappingNode == null || carrierMappingNode.Attributes == null)
                    continue;

                XmlNode carrierAccountIdNode = carrierMappingNode.Attributes["CarrierAccountID"];
                if (carrierAccountIdNode == null)
                    continue;

                string carrierAccountId = carrierAccountIdNode.InnerText;

                XmlNode inNode = carrierMappingNode.Attributes["In"];
                XmlNode outNode = carrierMappingNode.Attributes["Out"];

                string inTrunk = inNode != null ? inNode.InnerText : null;
                string outTrunk = outNode != null ? outNode.InnerText : null;

                List<string> inTrunkList = BuildTrunkList(inTrunk);
                List<string> outTrunkList = BuildTrunkList(outTrunk);

                if (inTrunkList != null || outTrunkList != null)
                {
                    CarrierAccount carrier;
                    if (allCarrierAccounts.TryGetValue(carrierAccountId, out carrier))
                    {
                        CarrierMapping carrierMapping = new CarrierMapping()
                        {
                            CarrierId = carrier.CarrierAccountId,
                            CustomerMapping = (carrier.AccountType == CarrierAccountType.Customer || carrier.AccountType == CarrierAccountType.Exchange) ? inTrunkList : null,
                            SupplierMapping = (carrier.AccountType == CarrierAccountType.Supplier || carrier.AccountType == CarrierAccountType.Exchange) ? outTrunkList : null
                        };
                        mappings.Add(carrierMapping.CarrierId.ToString(), carrierMapping);
                    }
                    else
                    {
                        context.WriteWarning(string.Format("Carrier Account ID {0} doesn't exist in Carrier Accounts", carrierAccountId));
                    }
                }
            }

            return mappings.Count > 0 ? mappings : null;
        }

        private List<string> BuildTrunkList(string trunk)
        {
            if (string.IsNullOrEmpty(trunk))
                return null;

            List<string> trunkList = new List<string>();
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
                if (string.IsNullOrEmpty(mapping) || string.IsNullOrEmpty(process) || !process.ToUpper().Contains("RT"))
                    continue;

                trunkList.Add(mapping);
            }

            return trunkList.Count > 0 ? trunkList : null;
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
    }
}

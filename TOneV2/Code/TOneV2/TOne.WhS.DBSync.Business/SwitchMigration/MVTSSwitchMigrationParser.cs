using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Entities;
using TOne.WhS.RouteSync.Entities;
using TOne.WhS.RouteSync.MVTSRadius;
using TOne.WhS.RouteSync.MVTSRadius.SQL;
using TOne.WhS.RouteSync.Radius;
using Vanrise.GenericData.Transformation.Entities;

namespace TOne.WhS.DBSync.Business
{
    public class MVTSSwitchMigrationParser : SwitchMigrationParser
    {
        private string _configuration;

        public MVTSSwitchMigrationParser(string configuration)
        {
            _configuration = configuration;
        }
        public override SwitchData GetSwitchData(MigrationContext context, int switchId, Dictionary<string, CarrierAccount> allCarrierAccounts)
        {
            return ReadXml(allCarrierAccounts, context);
        }

        private SwitchData ReadXml(Dictionary<string, CarrierAccount> allCarrierAccounts, MigrationContext context)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(_configuration);
            if (xml.DocumentElement == null)
                return null;

            XmlNode parametersNode = xml.DocumentElement.SelectSingleNode("Parameters");

            MVTSRadiusSWSync synchroniser = new MVTSRadiusSWSync()
            {
                CarrierMappings = BuildCarrierMapping(xml.DocumentElement.SelectSingleNode("CarrierMapping"), allCarrierAccounts, context),
                NumberOfOptions = GetMaximumSuppliersNumber(parametersNode),
                MappingSeparator = ";",
            };
            string connectionString;
            string redundantConnectionString;

            GetConnectionStrings(parametersNode, out connectionString, out redundantConnectionString);
            if (!string.IsNullOrEmpty(connectionString))
            {
                synchroniser.DataManager = new RadiusSQLDataManager()
                {
                    ConnectionString = GetRadiusConnection(context, connectionString, "MVTSMaxDoP"),
                    RedundantConnectionStrings = !string.IsNullOrEmpty(redundantConnectionString) ? new List<RouteSync.Radius.RadiusConnectionString> { GetRadiusConnection(context, redundantConnectionString, "MVTSMaxDoP_Redundant") } : null
                };
            }
            return new SwitchData
            {
                SwitchRouteSynchronizer = synchroniser
            };
        }

        RadiusConnectionString GetRadiusConnection(MigrationContext context, string connectionString, string maxDopKey)
        {
            var connection = new RadiusConnectionString() { ConnectionString = connectionString };
            ParameterValue maxDOPValue;
            if (context.ParameterDefinitions.TryGetValue(maxDopKey, out maxDOPValue))
                connection.MaxDoP = GetMaxDoP(maxDOPValue);
            return connection;
        }

        int? GetMaxDoP(ParameterValue maxDOPValue)
        {
            int maxDOP = 0;
            int.TryParse(maxDOPValue.Value, out maxDOP);
            return maxDOP > 0 ? maxDOP : default(int?);
        }

        private void GetConnectionStrings(XmlNode parametersNode, out string connectionString, out string redundantConnectionString)
        {
            connectionString = null;
            redundantConnectionString = null;

            XmlNode connectionStringsNode = GetXmlNodeByParameterName(parametersNode, "$Direct_Mode_Connection_String");

            if (connectionStringsNode == null)
                return;

            string[] connectionStrings = connectionStringsNode.FirstChild.Value.Split('|');
            connectionString = connectionStrings[0];

            if (connectionStrings.Count() > 1)
                redundantConnectionString = connectionStrings[1];
        }

        private int GetMaximumSuppliersNumber(XmlNode parametersNode)
        {
            XmlNode maximumSuppliersNumberNode = GetXmlNodeByParameterName(parametersNode, "$Radius_Maximum_Suppliers_Number");
            int maximumSuppliersNumber;

            if (maximumSuppliersNumberNode == null || !int.TryParse(maximumSuppliersNumberNode.FirstChild.Value, out maximumSuppliersNumber))
                return default(int);

            return maximumSuppliersNumber;
        }

        private XmlNode GetXmlNodeByParameterName(XmlNode parametersNode, string name)
        {
            return parametersNode.SelectSingleNode("Parameter[@Name='" + name + "']");
        }

        private Dictionary<string, TOne.WhS.RouteSync.MVTSRadius.MVTSRadiusSWSync.CarrierMapping> BuildCarrierMapping(XmlNode parentCarrierMappingNode, Dictionary<string, CarrierAccount> allCarrierAccounts, MigrationContext context)
        {
            if (parentCarrierMappingNode == null)
                return null;

            Dictionary<string, TOne.WhS.RouteSync.MVTSRadius.MVTSRadiusSWSync.CarrierMapping> mappings = new Dictionary<string, MVTSRadiusSWSync.CarrierMapping>();

            foreach (XmlNode carrierMappingNode in parentCarrierMappingNode.ChildNodes)
            {
                if (carrierMappingNode == null || carrierMappingNode.Attributes == null)
                    continue;

                XmlNode carrierAccountIdNode = carrierMappingNode.Attributes["CarrierAccountID"];
                if (carrierAccountIdNode == null)
                    continue;

                string carrierAccountId = carrierAccountIdNode.InnerText;

                XmlNode inNodeNode = carrierMappingNode.Attributes["In"];
                XmlNode outNode = carrierMappingNode.Attributes["Out"];


                string inTrunk = inNodeNode != null ? inNodeNode.InnerText : null;
                string outTrunk = outNode != null ? outNode.InnerText : null;

                List<string> inTrunkList = BuildTrunkList(inTrunk);
                List<string> outTrunkList = BuildTrunkList(outTrunk);

                if (inTrunkList != null || outTrunkList != null)
                {
                    CarrierAccount carrier;
                    if (allCarrierAccounts.TryGetValue(carrierAccountId, out carrier))
                    {
                        TOne.WhS.RouteSync.MVTSRadius.MVTSRadiusSWSync.CarrierMapping carrierMapping = new MVTSRadiusSWSync.CarrierMapping()
                        {
                            CarrierId = carrier.CarrierAccountId,
                            CustomerMapping = inTrunkList,
                            SupplierMapping = outTrunkList
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
    }
}
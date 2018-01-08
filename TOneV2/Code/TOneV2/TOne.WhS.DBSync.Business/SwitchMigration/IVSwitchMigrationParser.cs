using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Entities;
using TOne.WhS.RouteSync.IVSwitch;
using Vanrise.GenericData.Transformation.Entities;

namespace TOne.WhS.DBSync.Business.SwitchMigration
{
    public class IVSwitchMigrationParser : SwitchMigrationParser
    {
        private string _configuration;
        private string _blockedAccountMapping;
        public IVSwitchMigrationParser(string configuration)
        {
            _configuration = configuration;
        }
        private SwitchData ReadXml(MigrationContext context, Dictionary<string, BusinessEntity.Entities.CarrierAccount> allCarrierAccounts)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(_configuration);
            if (xml.DocumentElement == null)
                return null;

            XmlNode parametersNode = xml.DocumentElement.SelectSingleNode("Parameters");

            IVSwitchSWSync ivSwitch = new IVSwitchSWSync
            {
                RouteConnectionString = GetNodeValueAsString(parametersNode, "$Routing_ConnectionString"),
                MasterConnectionString = GetNodeValueAsString(parametersNode, "$Master_ConnectionString"),
                TariffConnectionString = GetNodeValueAsString(parametersNode, "$Tariff_ConnectionString"),
                NumberOfOptions = GetNodeValueAsInt(parametersNode, "$Routing_Maximum_Options"),
                OwnerName = "",
                Separator = ";",
                CarrierMappings = BuildCarrierMapping(xml.DocumentElement.SelectSingleNode("CarrierMapping"), allCarrierAccounts, context)
            };
            ivSwitch.BlockedAccountMapping = _blockedAccountMapping;

            return new SwitchData
            {
                SwitchRouteSynchronizer = ivSwitch
            };
        }


        private Dictionary<string, CarrierMapping> BuildCarrierMapping(XmlNode parentCarrierMappingNode, Dictionary<string, CarrierAccount> allCarrierAccounts, MigrationContext context)
        {
            if (parentCarrierMappingNode == null)
                return null;

            Dictionary<string, CarrierMapping> mappings = new Dictionary<string, CarrierMapping>();
            foreach (XmlNode carrierMappingNode in parentCarrierMappingNode.ChildNodes)
            {
                if (carrierMappingNode == null || carrierMappingNode.Attributes == null)
                    continue;
                XmlNode carrierAccountIdNode = carrierMappingNode.Attributes["CarrierAccountID"];
                if (carrierAccountIdNode == null)
                    continue;
                string carrierAccountId = carrierAccountIdNode.InnerText;
                XmlNode inNodeNode = carrierMappingNode.Attributes["InCarrier"];
                XmlNode outNode = carrierMappingNode.Attributes["OutCarrier"];
                string inCarrier = inNodeNode != null ? inNodeNode.InnerText : null;
                string outCarrier = outNode != null ? outNode.InnerText : null;

                if (carrierAccountId.Equals("BLK"))
                {
                    List<string> tempBlockedList = BuildCarrierList(outCarrier, false);
                    if (tempBlockedList != null && tempBlockedList.Count > 0)
                        _blockedAccountMapping = tempBlockedList.First();
                    continue;
                }
                List<string> inCarrierList = BuildCarrierList(inCarrier, true);
                List<string> outCarrierList = BuildCarrierList(outCarrier, false);
                if (inCarrierList != null || outCarrierList != null)
                {
                    CarrierAccount carrier;
                    if (allCarrierAccounts.TryGetValue(carrierAccountId, out carrier))
                    {
                        CarrierMapping carrierMapping = new CarrierMapping
                        {
                            CarrierId = carrier.CarrierAccountId.ToString(),
                            CustomerMapping = inCarrierList,
                            SupplierMapping = outCarrierList
                        };
                        mappings.Add(carrierMapping.CarrierId, carrierMapping);
                    }
                    else
                    {
                        context.WriteWarning(string.Format("Carrier Account ID {0} doesn't exist in Carrier Accounts",
                            carrierAccountId));
                    }
                }
            }
            return mappings;
        }
        public override SwitchData GetSwitchData(Entities.MigrationContext context, int switchId, Dictionary<string, BusinessEntity.Entities.CarrierAccount> allCarrierAccounts)
        {
            return ReadXml(context, allCarrierAccounts);
        }
        private List<string> BuildCarrierList(string trunk, bool isIn)
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
                if (!isIn && resultCount == 3)
                    mapping = string.Format("{0}:{1}", mapping, result[1]);
                trunkList.Add(mapping);
            }
            return trunkList.Count > 0 ? trunkList : null;
        }
        #region Parameters

        private string GetNodeValueAsString(XmlNode parametersNode, string parameterName)
        {
            XmlNode nodeValue = GetXmlNodeByParameterName(parametersNode, parameterName);
            return nodeValue != null ? nodeValue.FirstChild.Value : "";
        }
        private int GetNodeValueAsInt(XmlNode parametersNode, string parameterName)
        {
            string value = GetNodeValueAsString(parametersNode, parameterName);
            int integerValue;
            if (string.IsNullOrEmpty(value) || !int.TryParse(value, out integerValue))
                return default(int);
            return integerValue;
        }
        private XmlNode GetXmlNodeByParameterName(XmlNode parametersNode, string name)
        {
            return parametersNode.SelectSingleNode("Parameter[@Name='" + name + "']");
        }
        #endregion
    }
}

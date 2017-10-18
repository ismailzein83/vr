using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Entities;
using Vanrise.GenericData.MainExtensions.GenericRuleCriteriaFieldValues;

namespace TOne.WhS.DBSync.Business
{
    public class TelesSwitchParser : ISwitchParser
    {
        private string Configuration { get; set; }
        public TelesSwitchParser(string configuration)
        {
            Configuration = configuration;
        }
        private List<InParsedMapping> InParsedMappings { get; set; }

        private List<OutParsedMapping> OutParsedMappings { get; set; }

        private static readonly Regex CdrInOutParser = new Regex(
           @"(?<Cdr>[^;,:]+):([^;]+,)*CDR(=(?<Prefix>[#0-9]+)){0,1}($|((,[^;]+);)*)",
           RegexOptions.Compiled
           | RegexOptions.ExplicitCapture
           | RegexOptions.IgnoreCase);

        private void ReadXml()
        {
            InParsedMappings = new List<InParsedMapping>();
            OutParsedMappings = new List<OutParsedMapping>();
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(Configuration);
            if (xml.DocumentElement != null)
            {
                XmlNode carrierMappingNode = xml.DocumentElement.SelectSingleNode("CarrierMapping");
                if (carrierMappingNode != null)
                {
                    foreach (XmlNode anode in carrierMappingNode.ChildNodes)
                    {
                        if (anode != null && anode.Attributes != null)
                        {
                            string carrierAccountId = anode.Attributes["CarrierAccountID"] != null
                                ? anode.Attributes["CarrierAccountID"].InnerText
                                : null;
                            
                            #region IN

                            string inTrunk = anode.Attributes["In"] != null ? anode.Attributes["In"].InnerText : null;
                            if (inTrunk != null)
                            {
                                string[] inTrunkStrings = inTrunk.Split(';');
                                InParsedMapping inParsedMapping = new InParsedMapping
                                {
                                    CustomerId = carrierAccountId,
                                    InTrunk = new StaticValues()
                                };

                                string inPrefix = anode.Attributes["InPrefix"] != null
                                    ? (!anode.Attributes["InPrefix"].InnerText.Equals("---")
                                        ? anode.Attributes["InPrefix"].InnerText
                                        : null)
                                    : null;

                                if (inPrefix != null)
                                    inParsedMapping.InPrefix = new StaticValues
                                    {
                                        Values = inPrefix.Split(',').Cast<Object>().ToList()
                                    };

                                List<string> trunkList = new List<string>();
                                foreach (var inTrunkElt in inTrunkStrings)
                                {
                                    trunkList.AddRange((from Match match in CdrInOutParser.Matches(inTrunkElt.Trim())
                                                        select match.Groups["Cdr"].Value).ToList());
                                }
                                inParsedMapping.InTrunk.Values = trunkList.Cast<Object>().ToList();
                                InParsedMappings.Add(inParsedMapping);
                            }

                            #endregion
                            #region OUT

                            string outTrunk = anode.Attributes["Out"] != null ? anode.Attributes["Out"].InnerText : null;
                            if (outTrunk != null)
                            {
                                string[] outTrunkStrings = outTrunk.Split(';');
                                OutParsedMapping outParsedMapping = new OutParsedMapping
                                {
                                    SupplierId = carrierAccountId,
                                    OutTrunk = new StaticValues()
                                };

                                string outCdpnOutPrefix = anode.Attributes["OutCDPNOutPrefix"] != null
                                    ? (!anode.Attributes["OutCDPNOutPrefix"].InnerText.Equals("---")
                                        ? anode.Attributes["OutCDPNOutPrefix"].InnerText
                                        : null)
                                    : null;

                                if (outCdpnOutPrefix != null)
                                    outParsedMapping.OutPrefix = new StaticValues
                                    {
                                        Values = outCdpnOutPrefix.Split(',').Cast<Object>().ToList()
                                    };

                                List<string> outrunkList = new List<string>();
                                foreach (var outTrunkElt in outTrunkStrings)
                                {
                                    outrunkList.AddRange((from Match match in CdrInOutParser.Matches(outTrunkElt.Trim())
                                                          select match.Groups["Cdr"].Value).ToList());
                                }
                                outParsedMapping.OutTrunk.Values = outrunkList.Cast<Object>().ToList();
                                OutParsedMappings.Add(outParsedMapping);
                            }

                            #endregion
                        }
                    }
                }
            }
        }

        public void GetParsedMappings(out List<InParsedMapping> inParsedMappings, out List<OutParsedMapping> outParsedMappings)
        {
            ReadXml();
            inParsedMappings = InParsedMappings;
            outParsedMappings = OutParsedMappings;
        }
    }


}

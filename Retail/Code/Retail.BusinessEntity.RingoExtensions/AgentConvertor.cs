using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.FileIO;
using Retail.BusinessEntity.Entities;
using Vanrise.BEBridge.Entities;
using Vanrise.Common;

namespace Retail.BusinessEntity.RingoExtensions
{
    public class AgentConvertor : TargetBEConvertor
    {
        public override string Name
        {
            get
            {
                return "Agents";
            }
        }
        public override void ConvertSourceBEs(ITargetBEConvertorConvertSourceBEsContext context)
        {
            FileSourceBatch fileBatch = context.SourceBEBatch as FileSourceBatch;
            if (fileBatch == null)
                throw new NullReferenceException("fileBatch");

            Dictionary<string, ITargetBE> targetBes = new Dictionary<string, ITargetBE>();
            using (Stream stream = new MemoryStream(fileBatch.Content))
            {
                TextFieldParser parser = new TextFieldParser(stream)
                {
                    Delimiters = new string[] { "," }
                };
                while (true)
                {
                    string[] accountRecords = parser.ReadFields();
                    if (accountRecords != null)
                    {
                        SourceAgent agentData = new SourceAgent
                        {
                            Agent = new Agent()
                        };
                        accountRecords = accountRecords.Select(s => s.Trim('\'')).ToArray();
                        var sourceId = accountRecords[33];
                        if (string.IsNullOrEmpty(sourceId) || sourceId == "NA")
                            continue;
                        ITargetBE targetBe;
                        if (!targetBes.TryGetValue(sourceId, out targetBe))
                        {
                            agentData.Agent.SourceId = sourceId;
                            agentData.Agent.Name = accountRecords[34];
                            agentData.Agent.Type = accountRecords[35];
                            targetBes.Add(sourceId, agentData);
                        }
                    }
                    else
                        break;
                }
            }
            context.TargetBEs = targetBes.Values.ToList();
        }

        public override void MergeTargetBEs(ITargetBEConvertorMergeTargetBEsContext context)
        {
            SourceAgent existingBe = context.ExistingBE as SourceAgent;
            SourceAgent newBe = context.NewBE as SourceAgent;

            SourceAgent finalBe = new SourceAgent
            {
                Agent = Serializer.Deserialize<Agent>(Serializer.Serialize(existingBe.Agent))
            };

            finalBe.Agent.Name = newBe.Agent.Name;
            finalBe.Agent.Settings = newBe.Agent.Settings;
            finalBe.Agent.Type = newBe.Agent.Type;

            context.FinalBE = finalBe;
        }
    }
}

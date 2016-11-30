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
    public class DistributorConvertor : TargetBEConvertor
    {
        public override string Name
        {
            get
            {
                return "Distributors";
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
                        SourceDistributor distributorData = new SourceDistributor
                        {
                            Distributor = new Distributor()
                        };
                        accountRecords = accountRecords.Select(s => s.Trim('\'')).ToArray();
                        var sourceId = accountRecords[36];
                        if (string.IsNullOrEmpty(sourceId) || sourceId == "NA")
                            continue;
                        ITargetBE targetBe;
                        if (!targetBes.TryGetValue(sourceId, out targetBe))
                        {
                            distributorData.Distributor.SourceId = sourceId;
                            distributorData.Distributor.Name = accountRecords[37];
                            distributorData.Distributor.Type = accountRecords[38];
                            targetBes.Add(sourceId, distributorData);
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
            SourceDistributor existingBe = context.ExistingBE as SourceDistributor;
            SourceDistributor newBe = context.NewBE as SourceDistributor;

            SourceDistributor finalBe = new SourceDistributor
            {
                Distributor = Serializer.Deserialize<Distributor>(Serializer.Serialize(existingBe.Distributor))
            };

            finalBe.Distributor.Name = newBe.Distributor.Name;
            finalBe.Distributor.Settings = newBe.Distributor.Settings;
            finalBe.Distributor.Type = newBe.Distributor.Type;
            finalBe.Distributor.SourceId = newBe.Distributor.SourceId;
            
            context.FinalBE = finalBe;
        }
    }
}

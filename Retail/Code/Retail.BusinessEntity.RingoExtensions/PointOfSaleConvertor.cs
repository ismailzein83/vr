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
    public class PointOfSaleConvertor : TargetBEConvertor
    {
        public override string Name
        {
            get
            {
                return "Pont of Sales";
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
                        SourcePOS posData = new SourcePOS
                        {
                            PointOfSale = new PointOfSale()
                        };
                        accountRecords = accountRecords.Select(s => s.Trim('\'')).ToArray();

                        var sourceId = accountRecords[30];
                        if (string.IsNullOrEmpty(sourceId) || sourceId == "NA")
                            continue;
                        ITargetBE targetBe;
                        if (!targetBes.TryGetValue(sourceId, out targetBe))
                        {
                            posData.PointOfSale.SourceId = sourceId;
                            posData.PointOfSale.Name = accountRecords[31];
                            posData.PointOfSale.Type = accountRecords[32];
                            targetBes.Add(sourceId, posData);
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
            SourcePOS existingBe = context.ExistingBE as SourcePOS;
            SourcePOS newBe = context.NewBE as SourcePOS;

            SourcePOS finalBe = new SourcePOS
            {
                PointOfSale = Serializer.Deserialize<PointOfSale>(Serializer.Serialize(existingBe.PointOfSale))
            };

            finalBe.PointOfSale.Name = newBe.PointOfSale.Name;
            finalBe.PointOfSale.Settings = newBe.PointOfSale.Settings;
            finalBe.PointOfSale.Type = newBe.PointOfSale.Type;

            context.FinalBE = finalBe;
        }
    }
}

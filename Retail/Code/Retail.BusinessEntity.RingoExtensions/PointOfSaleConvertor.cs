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

namespace Retail.Ringo.MainExtensions
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
                        SourceAccountData posData = new SourceAccountData
                        {
                            Account = new Account()
                        };
                        accountRecords = accountRecords.Select(s => s.Trim('\'')).ToArray();

                        var sourceId = accountRecords[30];
                        if (string.IsNullOrEmpty(sourceId) || sourceId == "NA")
                            continue;
                        ITargetBE targetBe;
                        if (!targetBes.TryGetValue(sourceId, out targetBe))
                        {
                            posData.Account.SourceId = sourceId;
                            posData.Account.Name = accountRecords[31];
                            posData.Account.TypeId = new Guid("2A4D4D3A-EC47-4CB0-9D72-23263A20BA71");
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
            SourceAccountData existingBe = context.ExistingBE as SourceAccountData;
            SourceAccountData newBe = context.NewBE as SourceAccountData;

            SourceAccountData finalBe = new SourceAccountData
            {
                Account = Serializer.Deserialize<Account>(Serializer.Serialize(existingBe.Account))
            };

            finalBe.Account.Name = newBe.Account.Name;
            finalBe.Account.Settings = newBe.Account.Settings;
            finalBe.Account.TypeId = newBe.Account.TypeId;

            context.FinalBE = finalBe;
        }
    }
}

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
    public class DistributorConvertor : TargetBEConvertor
    {
        public override string Name
        {
            get
            {
                return "Distributors";
            }
        }
        public Guid AccountBEDefinitionId { get; set; }
        public Guid AccountTypeId { get; set; }
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
                    string line = parser.ReadLine();
                    if (string.IsNullOrEmpty(line))
                        break;
                    line = line.Replace(", ", " ");
                    string[] accountRecords = line.Split(',');
                    SourceAccountData distributorData = new SourceAccountData
                    {
                        Account = new Account()
                    };
                    accountRecords = accountRecords.Select(s => s.Trim('\'')).ToArray();
                    var sourceId = accountRecords[36];
                    if (string.IsNullOrEmpty(sourceId) || sourceId == "NA")
                        continue;
                    ITargetBE targetBe;
                    if (!targetBes.TryGetValue(sourceId, out targetBe))
                    {
                        distributorData.Account.SourceId = sourceId;
                        distributorData.Account.Name = accountRecords[37];
                        distributorData.Account.TypeId = this.AccountTypeId;// new Guid("FE70C894-36FD-412F-BFD3-D4C0E543925C");
                        targetBes.Add(sourceId, distributorData);
                    }
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
            finalBe.Account.SourceId = newBe.Account.SourceId;

            context.FinalBE = finalBe;
        }
    }
}

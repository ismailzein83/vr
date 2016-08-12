using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.RingoExtensions;
using Vanrise.BEBridge.BP.Arguments;
using Vanrise.BEBridge.Entities;
using Vanrise.BEBridge.MainExtensions.SourceBEReaders;
using Vanrise.Common;

namespace Retail.Runtime.Tasks
{
    public class RabihTask : ITask
    {
        public void Execute()
        {
            BEReceiveDefinition beDefinition = new BEReceiveDefinition
            {
                BEReceiveDefinitionId = Guid.NewGuid(),
                Name = "Ringo Subscriber Account Recieve Definition",
                Settings = new BEReceiveDefinitionSettings
                {
                    SourceBEReader = new FileSourceReader
                    {
                        Setting = new FileSourceReaderSetting
                        {
                            Directory = @"c:\RingoSubscriberFiles",
                            Extension = "CSV",
                            Mask = ""
                        }
                    },
                    TargetBEConvertor = new RingoFileAccountConvertor(),
                    TargetBESynchronizer = new AccountSynchronizer()
                }
            };

            string str = Serializer.Serialize(beDefinition);

            SourceBESyncProcessInput processInput = new SourceBESyncProcessInput
            {
                BEReceiveDefinitionIds = new List<Guid> { new Guid("01bac79f-f20d-4d8c-8c39-efe51908c35c") },
                 UserId = 1
                  
            };

            str = Serializer.Serialize(processInput);
            
        }
    }
}

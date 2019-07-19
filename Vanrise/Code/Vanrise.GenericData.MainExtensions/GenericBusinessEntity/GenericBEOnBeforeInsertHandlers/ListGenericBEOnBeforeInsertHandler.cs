using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
namespace Vanrise.GenericData.MainExtensions.GenericBusinessEntity.GenericBEOnBeforeInsertHandlers
{
    public class GenericBEOnBeforeInsertHandlerDefinition
    {
        public string Name { get; set; }
        public GenericBEOnBeforeInsertHandler Settings { get; set; }
    }
    public class ListGenericBEOnBeforeInsertHandler : GenericBEOnBeforeInsertHandler
    {
        public override Guid ConfigId { get { return new Guid("8C2D5923-CF6E-4C6C-B38F-AB74B2B98730"); } }
        public List<GenericBEOnBeforeInsertHandlerDefinition> Handlers { get; set; }
        public override void Execute(IGenericBEOnBeforeInsertHandlerContext context)
        {
            if (Handlers != null && Handlers.Count > 0)
            {
                foreach (var handler in Handlers)
                {
                    if (handler != null && handler.Settings != null)
                    {
                        var handlerContext = new GenericBEOnBeforeInsertHandlerContext
                        {
                            BusinessEntityDefinitionId = context.BusinessEntityDefinitionId,
                            DefinitionSettings = context.DefinitionSettings,
                            GenericBusinessEntity = context.GenericBusinessEntity,
                            OldGenericBusinessEntity = context.OldGenericBusinessEntity,
                            OperationType = context.OperationType,
                            OutputResult = new OutputResult()
                            {
                                Result = true,
                                Messages = new List<string>()
                            }
                        };
                        handler.Settings.Execute(handlerContext);
                        if (handlerContext.OutputResult.Messages != null && handlerContext.OutputResult.Messages.Count > 0)
                            context.OutputResult.Messages.AddRange(handlerContext.OutputResult.Messages);
                        if (!handlerContext.OutputResult.Result)
                            context.OutputResult.Result = false;
                    }
                }
            }
        }
    }
}




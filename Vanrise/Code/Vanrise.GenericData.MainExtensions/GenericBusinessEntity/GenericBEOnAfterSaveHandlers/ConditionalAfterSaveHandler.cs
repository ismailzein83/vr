using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;

namespace Vanrise.GenericData.MainExtensions.GenericBusinessEntity.GenericBEOnAfterSaveHandlers
{
    public class ConditionalAfterSaveHandler : GenericBEOnAfterSaveHandler
    {
        public override Guid ConfigId
        {
            get { return new Guid("3CC1F256-2775-4A08-ADF4-B74DF4DF3F03"); }
        }

        public override void Execute(IGenericBEOnAfterSaveHandlerContext context)
        {
            if (this.Handlers != null)
            {
                foreach (var handler in Handlers)
                {
                    var genericBESaveConditionContext = new GenericBESaveConditionContext
                    {
                        DefinitionSettings = context.DefinitionSettings,
                        NewEntity = context.NewEntity,
                        OldEntity = context.OldEntity,
                    };
                    if (handler.Condition == null || handler.Condition.IsMatch(genericBESaveConditionContext))
                    {
                        handler.Handler.Execute(new GenericBEOnAfterSaveHandlerContext
                        {
                            DefinitionSettings = context.DefinitionSettings,
                            BusinessEntityDefinitionId = context.BusinessEntityDefinitionId,
                            NewEntity = context.NewEntity,
                            OldEntity = context.OldEntity
                        });
                    }
                }
            }
        }
        public List<ConditionalAfterSaveHandlerItem> Handlers { get; set; }
    }
    public class ConditionalAfterSaveHandlerItem
    {
        public Guid ConditionalAfterSaveHandlerItemId { get; set; }
        public string Name { get; set; }
        public GenericBEOnAfterSaveHandler Handler { get; set; }
        public GenericBESaveCondition Condition { get; set; } 
    }
}

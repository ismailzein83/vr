using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;

namespace Vanrise.GenericData.MainExtensions.GenericBusinessEntity.GenericBEOnBeforeGetFilteredHandlers
{
    public class SetLoggedInUserIdBeforeGetFilteredHandler : GenericBEOnBeforeGetFilteredHandler
    {
        public override Guid ConfigId => new Guid("DB531799-AD7B-45D8-B9FE-475A30488B00");

        public string UserIdFieldName { get; set; }

        public RequiredPermissionSettings RequiredPermission { get; set; }

        public override void PrepareQuery(IGenericBEOnBeforeGetFilteredHandlerPrepareQueryContext context)
        {

            if (SecurityContext.Current.TryGetLoggedInUserId(out int? _userId))
            {
                SecurityManager securityManager = new SecurityManager();
                if (RequiredPermission == null || !securityManager.IsAllowed(RequiredPermission, _userId.Value))
                {
                    if (context.Query.Filters == null)
                        context.Query.Filters = new List<GenericBusinessEntityFilter>();

                    context.Query.Filters.Add(new GenericBusinessEntityFilter
                    {
                        FieldName = UserIdFieldName,
                        FilterValues = new List<object> { _userId }
                    });
                }
            }
        }


        public override void onBeforeAdd(IGenericBEOnBeforeAddHandlerContext context)
        {
        }

        public override void onBeforeUpdate(IGenericBEOnBeforeUpdateHandlerContext context)
        {
        }
    }
}


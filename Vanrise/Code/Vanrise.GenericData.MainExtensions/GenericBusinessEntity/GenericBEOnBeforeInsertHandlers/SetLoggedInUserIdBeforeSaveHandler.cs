using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.Security.Business;

namespace Vanrise.GenericData.MainExtensions.GenericBusinessEntity.GenericBEOnBeforeInsertHandlers
{
    public class SetLoggedInUserIdBeforeSaveHandler : GenericBEOnBeforeInsertHandler
    {
        public override Guid ConfigId => new Guid("D0B33C05-7078-4731-9688-4724F73699DD");

        public string UserIdFieldName { get; set; }

        public override void Execute(IGenericBEOnBeforeInsertHandlerContext context)
        {
            if (SecurityContext.Current.TryGetLoggedInUserId(out int? _userId))
            {
                if (context.GenericBusinessEntity.FieldValues == null)
                    context.GenericBusinessEntity.FieldValues = new Dictionary<string, object>();

                context.GenericBusinessEntity.FieldValues.Add(UserIdFieldName, _userId);
            }

        }
    }
}


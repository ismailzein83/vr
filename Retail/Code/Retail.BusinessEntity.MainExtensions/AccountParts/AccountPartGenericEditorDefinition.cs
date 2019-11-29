using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.GenericData.Entities;

namespace Retail.BusinessEntity.MainExtensions.AccountParts
{
    public class AccountPartGenericEditorDefinition : AccountPartDefinitionSettings
    {
        public override Guid ConfigId => new Guid("5B3C2BA0-2ACD-4013-AC24-68F36DDF241D");
        public Guid DataRecordTypeId { get; set; }
        public List<AccountPartGenericEditorItem> Items { get; set; }
        public override bool IsPartValid(IAccountPartDefinitionIsPartValidContext context)
        {
            AccountPartGenericEditor part = context.AccountPartSettings.CastWithValidate<AccountPartGenericEditor>("context.AccountPartSettings");
            return true;
        }
    }

    public class AccountPartGenericEditorItem
    {
        public string Title { get; set; }
        public VRGenericEditorDefinitionSetting Settings { get; set; }

    }
}

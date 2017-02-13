using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using Retail.Teles.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Teles.Business
{
    public class TelesEnterpriseGenericField : AccountGenericField
    {
        public Guid _enterpriseBEDefinitionId;
        public TelesEnterpriseGenericField(Guid enterpriseBEDefinitionId)
        {
            _enterpriseBEDefinitionId = enterpriseBEDefinitionId;
        }
        public override string Name
        {
            get { return "TelesEnterprise"; }
        }

        public override string Title
        {
            get { return "Teles Enterprise"; }
        }

        public override Vanrise.GenericData.Entities.DataRecordFieldType FieldType
        {
            get {
                return new Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityType { BusinessEntityDefinitionId = _enterpriseBEDefinitionId };
            }
        }

        public override dynamic GetValue(IAccountGenericFieldContext context)
        {
            AccountBEManager accountBEManager = new AccountBEManager();
            var enterpriseExtendedSettings = accountBEManager.GetExtendedSettings<EnterpriseAccountMappingInfo>(context.Account);
            if (enterpriseExtendedSettings != null)
            {
                return enterpriseExtendedSettings.TelesEnterpriseId;
            }
            return null;
        }
    }
}

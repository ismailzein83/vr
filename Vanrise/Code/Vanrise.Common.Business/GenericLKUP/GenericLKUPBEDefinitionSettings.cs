using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class GenericLKUPBEDefinitionSettings : Vanrise.GenericData.Entities.BusinessEntityDefinitionSettings
    {
        public static Guid s_configId = new Guid("F0DEC732-929C-4F75-AA35-9E19298D3092");
        public override Guid ConfigId { get { return s_configId; } }
        
        public GenericLKUPDefinitionExtendedSettings ExtendedSettings { get; set; }
        public override string DefinitionEditor
        {
            get { return "vr-common-genericlkup-be-definition"; }
        }
        public override string IdType
        {
            get { return "System.Guid"; }
        }

        public override string ManagerFQTN
        {
            get { return "Vanrise.Common.Business.GenericLKUPDefinitionManager, Vanrise.Common.Business"; }
        }

        public override string SelectorUIControl
        {
            get { return "vr-common-genericlkupitem-selector"; }
        }
    }
}

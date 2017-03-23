using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.Notification.Business
{
    public class VRAlertLevelBEDefinitionSettings : BusinessEntityDefinitionSettings
    {

        public static Guid s_configId = Guid.Empty;
        public override Guid ConfigId { get { throw new NotImplementedException(); } }
        public override string SelectorFilterEditor { get; set; }
        public override string DefinitionEditor
        {
            get { throw new NotImplementedException(); }
        }
        public override string IdType
        {
            get { throw new NotImplementedException(); }
        }
        public override string ManagerFQTN
        {
            get { throw new NotImplementedException(); }
        }
        public override string SelectorUIControl
        {
            get { throw new NotImplementedException(); }
        }
    }
}

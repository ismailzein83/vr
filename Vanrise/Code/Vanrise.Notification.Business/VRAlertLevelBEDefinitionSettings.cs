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

        public static Guid s_configId = new Guid("0B44D3F3-AA62-4289-8EB3-D93269515036");
        public override Guid ConfigId { get { return s_configId; } }
        public override string SelectorFilterEditor { get; set; }
        public override string DefinitionEditor
        {
            get { return "vr-notification-alertleveldefinitionbe-editor"; }
        }
        public override string IdType
        {

            get { return "System.Guid"; }
        }
        public override string ManagerFQTN
        {
            get { return "Vanrise.Notification.Business.VRAlertLevelManager,Vanrise.Notification.Business"; }
        }
        public override string SelectorUIControl
        {
            get { return "vr-notification-alertlevel-selector"; }
        }
    }
}

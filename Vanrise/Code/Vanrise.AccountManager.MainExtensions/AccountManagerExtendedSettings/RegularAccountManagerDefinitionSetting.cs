using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountManager.Business;

namespace Vanrise.AccountManager.MainExtensions
{
    public class RegularAccountManagerDefinitionSetting : AccountManagerDefinitionExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("3041F5F3-484F-4F05-9A79-3C953A37E0B9"); }
        }

        public override string RuntimeEditor
        {
            get { return "vr-accountmanager-runtime"; }
        }
    }
}

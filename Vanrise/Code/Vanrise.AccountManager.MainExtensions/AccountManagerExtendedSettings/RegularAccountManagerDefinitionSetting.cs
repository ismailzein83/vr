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
            get { throw new NotImplementedException(); }
        }

        public override string RuntimeEditor
        {
            get { throw new NotImplementedException(); }
        }
    }
}

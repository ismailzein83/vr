using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions
{
    public class EditGenericBEAction : GenericBEActionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("293B2FAB-6ABE-4BE7-AD58-7D9FA0BA9524"); }
        }
        public override string ActionTypeName { get { return "EditGenericBEAction"; } }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions
{
   public  class ListEditorDefinitionSetting : VRGenericEditorDefinitionSetting
    {
        public override Guid ConfigId
        {
            get { return new Guid("99458A8F-28B4-44F9-A8F0-EF20ED53061C"); }
        }
        public override string RuntimeEditor
        {
            get
            {
                return "vr-genericdata-listeditor-runtime";
            }
        }
    }
}

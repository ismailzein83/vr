using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions
{
    public class TimeFilterDefinitionSettings : GenericBEFilterDefinitionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("6FFB05B7-497B-4303-9CF0-61CA0F965D7D"); }
        }

        public override string RuntimeEditor
        {
            get { return "vr-genericdata-genericbe-filterruntime-time"; }
        }

    }

}

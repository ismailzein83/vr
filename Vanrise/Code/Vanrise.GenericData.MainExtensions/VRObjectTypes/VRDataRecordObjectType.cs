using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.GenericData.MainExtensions.VRObjectTypes
{
    public class VRDataRecordObjectType : VRObjectType
    {
        public override string PropertyEvaluatorExtensionType
        {
            get { return "VR_GenericData_DataRecordObjectType_PropertyEvaluator"; }
        }

        public int RecordTypeId { get; set; }
    }
}

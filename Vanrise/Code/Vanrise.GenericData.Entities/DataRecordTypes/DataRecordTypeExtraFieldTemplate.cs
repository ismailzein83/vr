using System;
using Vanrise.Entities;

namespace Vanrise.GenericData.Entities
{
    public class DataRecordTypeExtraFieldTemplate : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "VR_GenericData_DataRecordTypeExtraField";

        public string Editor { get; set; }
    }
}
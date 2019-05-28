using System.Collections.Generic;
using Vanrise.Entities;

namespace Vanrise.GenericData.Entities
{
    public enum ContainerType { Root = 0, Tab = 1, Row = 2, Column = 3 }
    public class GenericBEEditorDefinitionSettingsConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "VR_GenericData_GenericBEEditorDefinitionSettings";

        public string Editor { get; set; }

       public List<ContainerType> ValidContainers { get; set; }
    }
}
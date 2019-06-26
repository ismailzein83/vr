using System;
using System.Collections.Generic;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions
{
    public class RowsContainerEditorDefinitionSetting : VRGenericEditorDefinitionSetting
    {
        public override Guid ConfigId { get { return new Guid("747F6659-2541-4008-A9CF-56A604E3F63C"); } }

        public override string RuntimeEditor { get { return "vr-genericdata-rowscontainereditor-runtime"; } }

        public override List<GridColumnAttribute> GetGridColumnsAttributes(IGetGenericEditorColumnsInfoContext context)
        {
            List<GridColumnAttribute> columnsAttributes = new List<GridColumnAttribute>();

            if (RowContainers!=null && RowContainers.Count > 0)
            {
                foreach(var rowContainer in RowContainers)
                {
                    if(rowContainer!=null && rowContainer.RowSettings!=null && rowContainer.RowSettings.Count > 0)
                    {
                        foreach (var editorDefinition in rowContainer.RowSettings)
                        {
                            var attributes = editorDefinition.GetGridColumnsAttributes(new GetGenericEditorColumnsInfoContext
                            {
                                DataRecordTypeId = context.DataRecordTypeId
                            });
                            if (attributes != null && attributes.Count > 0)
                                columnsAttributes.AddRange(attributes);
                        }
                    }
                }
            }
            return columnsAttributes;
        }

        public List<VRRowContainer> RowContainers { get; set; }
    }

    public class VRRowContainer
    {
        public List<VRGenericEditorDefinitionSetting> RowSettings { get; set; }
    }
}
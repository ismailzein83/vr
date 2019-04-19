using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions
{
	public class GenericEditorDefinitionSetting : VRGenericEditorDefinitionSetting
	{
		public override Guid ConfigId
		{
			get { return new Guid("5BE30B11-8EE3-47EB-8269-41BDAFE077E1"); }
		}
		public List<GenericEditorRow> Rows { get; set; }
		public override string RuntimeEditor
		{
			get
			{
				return "vr-genericdata-genericeditorsetting-runtime";
			}
		}

		public override void TryTranslate()
		{
			VRLocalizationManager vrLocalizationManager = new VRLocalizationManager();
			if (Rows != null)
			{
				foreach (var row in Rows)
				{
					if (row.Fields != null)
					{
						foreach (var filed in row.Fields)
						{
							if (!String.IsNullOrEmpty(filed.TextResourceKey))
							{
								filed.FieldTitle = vrLocalizationManager.GetTranslatedTextResourceValue(filed.TextResourceKey, filed.FieldTitle);
							}
						}
					}
				}
			}
		}
	}
}

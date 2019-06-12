﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions
{
    public class BasicAdvancedFilterDefinitionSettings : GenericBEFilterDefinitionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("8082FFC6-6781-4501-AA31-02493D74CB8F"); }
        }

        public override string RuntimeEditor
        {
            get { return "vr-genericdata-genericbe-filterruntime-basicadvanced"; }
        }
        public List<BasicAdvancedFilterItem> Filters { get; set; }

		public override void ApplyTranslation(IGenericBETranslationContext context)
		{
			if (Filters != null)
			{
				foreach (var filter in Filters)
				{
					filter.FilterSettings.ApplyTranslation(context);
				}
			}
		}
	}
    public class BasicAdvancedFilterItem
    {
        public Guid BasicAdvancedFilterItemId { get; set; }
        public string Name { get; set; }
        public bool ShowInBasic { get; set; }
        public GenericBEFilterDefinitionSettings FilterSettings { get; set; }
    }
}

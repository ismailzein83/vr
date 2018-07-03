﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class AutoImportSPLSettings : Vanrise.Entities.SettingData
    {
        public List<PricelistTypeMapping> PricelistTypeMappingList { get; set; }
        public List<SupplierAutoImportTemplate> SupplierAutoImportTemplateList { get; set; }
        public List<InternalAutoImportTemplate> InternalAutoImportTemplateList { get; set; }
    }
}

﻿using Retail.BusinessEntity.Entities;
using System;

namespace Retail.BusinessEntity.MainExtensions.PackageTypes
{

    public class RecurChargePackageDefinitionSettings : PackageDefinitionExtendedSettings
    {
        public override Guid ConfigId { get { return new Guid("E326482A-9AB5-4715-848F-11CAF4940040"); } }

        public override string RuntimeEditor { get { return "retail-be-packagesettings-extendedsettings-recurcharge"; } }

        public RecurringChargeEvaluatorDefinitionSettings EvaluatorDefinitionSettings { get; set; }
    }
}

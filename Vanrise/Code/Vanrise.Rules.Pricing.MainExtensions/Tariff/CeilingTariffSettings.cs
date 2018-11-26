using System;
using System.Collections.Generic;
using System.Text;
using Vanrise.Common.Business;

namespace Vanrise.Rules.Pricing.MainExtensions.Tariff
{
    public class CeilingTariffSettings : RegularTariffSettings
    {
        public override Guid ConfigId { get { return new Guid("8B86004A-754B-45C2-ADB5-CAEB220287B0"); } }
    }
}

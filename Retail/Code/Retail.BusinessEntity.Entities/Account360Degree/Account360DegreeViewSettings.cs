using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class Account360DegreeViewSettings : Vanrise.Entities.VRComponentTypeSettings
    {
        public override Guid VRComponentTypeConfigId
        {
            get { return new Guid("C026CF1E-FD37-4DC3-B0BC-ECF6BA2C197C"); }
        }

        public AccountCondition AvailabilityCondition { get; set; }

        public string NavigationSectionName { get; set; }

        public Account360DegreeViewExtendedSettings ExtendedSettings { get; set; }
    }
}

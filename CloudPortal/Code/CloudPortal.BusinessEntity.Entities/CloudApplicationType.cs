using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudPortal.BusinessEntity.Entities
{
    public enum ApplicationCompanyRegistrationType { SingleCompany = 0, MultipleCompanies = 1 }

    public class CloudApplicationType
    {
        public ApplicationCompanyRegistrationType CompanyRegistrationType { get; set; }
    }
}
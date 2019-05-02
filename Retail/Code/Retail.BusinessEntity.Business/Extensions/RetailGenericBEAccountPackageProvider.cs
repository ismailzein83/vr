using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Business
{
    public class RetailGenericBEAccountPackageProvider : AccountPackageProvider, IAccountPackageProvider
    {
        public override Guid ConfigId { get { return new Guid("0A7639D0-C96E-4CCE-9352-D6BE8A875FE0"); } }
        public Guid BusinessEntityDefinitionID { get; set; }
        public string AccountIDFieldName { get; set; }
        public string BEDFieldName { get; set; }
        public string EEDFieldName { get; set; }
        public string IDFieldName { get; set; }
        public string PackageFieldName { get; set; }
        public override Dictionary<AccountEventTime, List<RetailAccountPackage>> GetRetailAccountPackages(IAccountPackageProviderGetRetailAccountPackagesContext context)
        {
            throw new NotImplementedException();
        }
    }
}

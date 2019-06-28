using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Common;

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

        public override bool DoesUserHaveAddPackageAccess(IPackageDefinitionAccessContext context)
        {
            return true;
        }

        public override bool DoesUserHaveEditPackageAccess(IPackageDefinitionAccessContext context)
        {
            return true;
        }

        public override bool DoesUserHaveViewPackageAccess(IPackageDefinitionAccessContext context)
        {
            return true;
        }

        public override Dictionary<AccountEventTime, List<RetailAccountPackage>> GetRetailAccountPackages(IAccountPackageProviderGetRetailAccountPackagesContext context)
        {
            GenericBusinessEntityManager genericBusinessEntityManager = new GenericBusinessEntityManager();
            AccountPackageManager accountPackageManager = new AccountPackageManager();
            var retailAccountPackagesByAccountEventTime = new Dictionary<AccountEventTime, List<RetailAccountPackage>>();

            if (context.AccountEventTimeList != null)
            {
                foreach (var accountEventTime in context.AccountEventTimeList)
                {
                    var filterGroup = new RecordFilterGroup
                    {
                        LogicalOperator = RecordQueryLogicalOperator.And,
                        Filters = new List<RecordFilter>
                        {
                            new ObjectListRecordFilter{FieldName = AccountIDFieldName, CompareOperator= ListRecordFilterOperator.In, Values = new List<object>(){accountEventTime.AccountId } },
                            new DateTimeRecordFilter{FieldName = BEDFieldName, CompareOperator = DateTimeRecordFilterOperator.LessOrEquals, Value = accountEventTime.EventTime},
                            new RecordFilterGroup
                            {
                                LogicalOperator = RecordQueryLogicalOperator.Or,
                                Filters = new List<RecordFilter>
                                {
                                    new DateTimeRecordFilter{FieldName = EEDFieldName, CompareOperator = DateTimeRecordFilterOperator.Greater, Value = accountEventTime.EventTime },
                                    new EmptyRecordFilter{FieldName = EEDFieldName}
                                }
                            }
                        }
                    };
                    var packages = new List<RetailAccountPackage>();
                    var packageEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(this.BusinessEntityDefinitionID, null, filterGroup);
                    if (packageEntities != null)
                    {
                        foreach (var packageEntity in packageEntities)
                        {
                            packages.Add(new RetailAccountPackage()
                            {
                                AccountBEDefinitionId = context.AccountBEDefinitionId,
                                AccountId = (long)packageEntity.FieldValues.GetRecord(AccountIDFieldName),
                                AccountPackageId = (long)packageEntity.FieldValues.GetRecord(IDFieldName),
                                BED = (DateTime)packageEntity.FieldValues.GetRecord(BEDFieldName),
                                EED = (DateTime?)packageEntity.FieldValues.GetRecord(EEDFieldName),
                                PackageId = (int)packageEntity.FieldValues.GetRecord(PackageFieldName)
                            });
                        }
                    }
                    List<RetailAccountPackage> retailAccountPackages = retailAccountPackagesByAccountEventTime.GetOrCreateItem(accountEventTime);
                    retailAccountPackages.AddRange(packages);
                }
            }
            return retailAccountPackagesByAccountEventTime;
        }
    }
}

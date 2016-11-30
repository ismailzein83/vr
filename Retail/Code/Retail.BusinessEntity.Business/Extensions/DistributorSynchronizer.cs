using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retail.BusinessEntity.Entities;
using Vanrise.BEBridge.Entities;
using Vanrise.Common;

namespace Retail.BusinessEntity.Business
{

    public class DistributorSynchronizer : TargetBESynchronizer
    {
        public override string Name
        {
            get
            {
                return "Distributors";
            }
        }
        public override void InsertBEs(ITargetBESynchronizerInsertBEsContext context)
        {
            if (context.TargetBE == null)
                throw new NullReferenceException("context.TargetBE");
            DistributorManager distributorManager = new DistributorManager();
            foreach (var targetAccount in context.TargetBE)
            {
                long distributorId;
                SourceDistributor distributorData = targetAccount as SourceDistributor;
                distributorManager.TryAddDistributor(distributorData.Distributor, out distributorId);
            }
        }

        public override bool TryGetExistingBE(ITargetBESynchronizerTryGetExistingBEContext context)
        {
            DistributorManager distributorManager = new DistributorManager();
            Distributor distributor = distributorManager.GetDistributorBySourceId(context.SourceBEId as string);
            if (distributor != null)
            {
                context.TargetBE = new SourceDistributor
                {
                    Distributor = Serializer.Deserialize<Distributor>(Serializer.Serialize(distributor))
                };
                return true;
            }
            return false;
        }

        public override void UpdateBEs(ITargetBESynchronizerUpdateBEsContext context)
        {
            if (context.TargetBE == null)
                throw new NullReferenceException("context.TargetBE");
            DistributorManager distributorManager = new DistributorManager();

            foreach (var target in context.TargetBE)
            {
                SourceDistributor distributorData = target as SourceDistributor;
                Distributor distributor = new Distributor
                {
                    Id = distributorData.Distributor.Id,
                    Type = distributorData.Distributor.Type,
                    Name = distributorData.Distributor.Name,
                    Settings = distributorData.Distributor.Settings,
                    SourceId = distributorData.Distributor.SourceId
                };
                distributorManager.TryUpdateDistributor(distributor);
            }
        }
    }
}

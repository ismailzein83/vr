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
    public class PosSynchronizer : TargetBESynchronizer
    {
        public override void InsertBEs(ITargetBESynchronizerInsertBEsContext context)
        {
            if (context.TargetBE == null)
                throw new NullReferenceException("context.TargetBE");
            POSManager posManager = new POSManager();
            foreach (var targetAccount in context.TargetBE)
            {
                long posId;
                SourcePOS posData = targetAccount as SourcePOS;
                posManager.TryAddPOS(posData.PointOfSale, out posId);
            }
        }

        public override bool TryGetExistingBE(ITargetBESynchronizerTryGetExistingBEContext context)
        {
            POSManager posManager = new POSManager();
            PointOfSale pointOfSale = posManager.GetPOSBySourceId(context.SourceBEId as string);
            if (pointOfSale != null)
            {
                context.TargetBE = new SourcePOS()
                {
                     PointOfSale = Serializer.Deserialize<PointOfSale>(Serializer.Serialize(pointOfSale))
                };
                return true;
            }
            return false;
        }

        public override void UpdateBEs(ITargetBESynchronizerUpdateBEsContext context)
        {
            if (context.TargetBE == null)
                throw new NullReferenceException("context.TargetBE");
            POSManager posManager = new POSManager();

            foreach (var target in context.TargetBE)
            {
                SourcePOS distributorData = target as SourcePOS;
                PointOfSale pointOfSale = new PointOfSale
                {
                    Id = distributorData.PointOfSale.Id,
                    Type = distributorData.PointOfSale.Type,
                    Name = distributorData.PointOfSale.Name,
                    Settings = distributorData.PointOfSale.Settings,
                    SourceId = distributorData.PointOfSale.SourceId
                };
                posManager.TryUpdatePOS(pointOfSale);
            }
        }
    }
}

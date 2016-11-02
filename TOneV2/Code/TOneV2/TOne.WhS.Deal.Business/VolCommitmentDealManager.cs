using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using System.Threading.Tasks;
using TOne.WhS.Deal.Entities;
using TOne.WhS.Deal.Data;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Caching;
using TOne.WhS.Deal.Entities.Settings;
using System.ComponentModel;

namespace TOne.WhS.Deal.Business
{
    public class VolCommitmentDealManager : BaseDealManager
    {

        #region Public Methods      
        
        public Vanrise.Entities.IDataRetrievalResult<DealDefinitionDetail> GetFilteredVolCommitmentDeals(Vanrise.Entities.DataRetrievalInput<VolCommitmentDealQuery> input)
        {
            var cachedEntities = this.GetCachedVolCommitmentDeals();
            Func<DealDefinition, bool> filterExpression = (deal) =>
            {
                if (input.Query.Name != null && !deal.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    return false;
                if (input.Query.Type.HasValue && input.Query.Type.Value != (deal.Settings as VolCommitmentDealSettings).DealType)
                    return false;
                if (input.Query.CarrierAccountIds != null && !input.Query.CarrierAccountIds.Contains((deal.Settings as VolCommitmentDealSettings).CarrierAccountId))
                    return false;
                return true;
            };

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, cachedEntities.ToBigResult(input, filterExpression, DealDeinitionDetailMapper));
        }

        public override DealDefinitionDetail DealDeinitionDetailMapper(DealDefinition deal)
        {
            VolCommitmentDetail detail = new VolCommitmentDetail()
            {
                Entity = deal,
            };

            VolCommitmentDealSettings settings = deal.Settings as VolCommitmentDealSettings;
            int carrierAccountId = settings.CarrierAccountId;

            detail.CarrierAccountName = new CarrierAccountManager().GetCarrierAccountName(carrierAccountId);
            detail.TypeDescription = Utilities.GetEnumAttribute<VolCommitmentDealType, DescriptionAttribute>(settings.DealType).Description;
            detail.IsEffective = settings.BeginDate <= DateTime.Now.Date && settings.EndDate >= DateTime.Now.Date;
            return detail;
        }
        #endregion

        #region Mappers      

       

        #endregion
    }
}

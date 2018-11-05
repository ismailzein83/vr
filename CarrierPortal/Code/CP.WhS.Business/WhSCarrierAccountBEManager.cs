using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Security.Business;

namespace CP.WhS.Business
{
    public class WhSCarrierAccountBEManager : BaseBusinessEntityManager
    {

        #region Public Methods
        public IEnumerable<CarrierAccountInfo> GetRemoteCarrierAccountsInfo(string serializedFilter)
        {
            PortalConnectionManager connectionManager = new PortalConnectionManager();
            var connectionSettings = connectionManager.GetWhSConnectionSettings();
            var userId = SecurityContext.Current.GetLoggedInUserId();
            var deserializedFilter = Vanrise.Common.Serializer.Deserialize<CarrierAccountInfoFilter>(serializedFilter);
            if(deserializedFilter == null)
                deserializedFilter = new CarrierAccountInfoFilter();
            if (deserializedFilter.BusinessEntityDefinitionId.HasValue)
            {
                var whSCarrierAccountBEDefition = GetWhSCarrierAccountsBEDefinition(deserializedFilter.BusinessEntityDefinitionId.Value);
                deserializedFilter.GetCustomers = whSCarrierAccountBEDefition.GetCustomers;
                deserializedFilter.GetSuppliers = whSCarrierAccountBEDefition.GetSuppliers;
            }
            deserializedFilter.UserId = userId;
            return connectionSettings.Get<IEnumerable<CarrierAccountInfo>>(string.Format("/api/WhS_BE/CarrierAccount/GetCarrierAccountInfo?serializedFilter={0}", Vanrise.Common.Serializer.Serialize(deserializedFilter)));
        }
        #endregion

        #region Private Methods
        private WhSCarrierAccountsBEDefinition GetWhSCarrierAccountsBEDefinition(Guid businessEntityDefinitionId)
        {
            BusinessEntityDefinitionManager businessEntityDefinitionManager = new BusinessEntityDefinitionManager();
            var businessEntityDefinition = businessEntityDefinitionManager.GetBusinessEntityDefinition(businessEntityDefinitionId);
            if (businessEntityDefinition != null)
            {
                return businessEntityDefinition.Settings as WhSCarrierAccountsBEDefinition;
            }
            return null;
        }
        #endregion
        #region BaseBusinessEntityManager
        public override List<dynamic> GetAllEntities(IBusinessEntityGetAllContext context)
        {
            throw new NotImplementedException();
        }

        public override dynamic GetEntity(IBusinessEntityGetByIdContext context)
        {
            throw new NotImplementedException();
        }

        public override string GetEntityDescription(IBusinessEntityDescriptionContext context)
        {
            PortalConnectionManager connectionManager = new PortalConnectionManager();
            var connectionSettings = connectionManager.GetWhSConnectionSettings();
            return connectionSettings.Get<string>(string.Format("/api/WhS_BE/CarrierAccount/GetCarrierAccountName?carrierAccountId={0}", Convert.ToInt32(context.EntityId)));
        }

        public override dynamic GetEntityId(IBusinessEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<dynamic> GetIdsByParentEntityId(IBusinessEntityGetIdsByParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        public override dynamic GetParentEntityId(IBusinessEntityGetParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        public override bool IsCacheExpired(IBusinessEntityIsCacheExpiredContext context, ref DateTime? lastCheckTime)
        {
            throw new NotImplementedException();
        }

        public override dynamic MapEntityToInfo(IBusinessEntityMapToInfoContext context)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}

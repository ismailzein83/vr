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
            WhSConnectionManager connectionManager = new WhSConnectionManager();
            var connectionSettings = connectionManager.GetWhSConnectionSettings();
            var userId = SecurityContext.Current.GetLoggedInUserId();
            var deserializedFilter = Vanrise.Common.Serializer.Deserialize<CarrierAccountInfoFilter>(serializedFilter);
            if(deserializedFilter == null)
                deserializedFilter = new CarrierAccountInfoFilter();
            if (deserializedFilter.BusinessEntityDefinitionId.HasValue)
            {
                BusinessEntityDefinitionManager businessEntityDefinitionManager = new BusinessEntityDefinitionManager();
                var businessEntityDefinition = businessEntityDefinitionManager.GetBusinessEntityDefinition(deserializedFilter.BusinessEntityDefinitionId.Value);
                if (businessEntityDefinition != null)
                {
                    var whSCarrierAccountBEDefition = businessEntityDefinition.Settings as WhSCarrierAccountsBEDefinition;
                    deserializedFilter.GetCustomers = whSCarrierAccountBEDefition.GetCustomers;
                    deserializedFilter.GetSuppliers = whSCarrierAccountBEDefition.GetSuppliers;
                }
            }
            deserializedFilter.UserId = userId;
            return connectionSettings.Get<IEnumerable<CarrierAccountInfo>>(string.Format("/api/WhS_BE/CarrierAccount/GetCarrierAccountInfo?serializedFilter={0}", Vanrise.Common.Serializer.Serialize(deserializedFilter)));
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
            throw new NotImplementedException();
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Caching;
using Vanrise.Common;
namespace TOne.WhS.BusinessEntity.Business
{
    public class VolumeCommitmentManager
    {
        #region Fields

        CarrierAccountManager _carrierAccountManager = new CarrierAccountManager();

        #endregion
        
        #region Public Methods
        public Vanrise.Entities.IDataRetrievalResult<VolumeCommitmentDetail> GetFilteredVolumeCommitments(Vanrise.Entities.DataRetrievalInput<VolumeCommitmentQuery> input)
        {
            Dictionary<int, VolumeCommitment> cachedEntities = this.GetCachedVolumeCommitments();

            Func<VolumeCommitment, bool> filterExpression = (itm) =>
                (true);

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, cachedEntities.ToBigResult(input, filterExpression, VolumeCommitmentDetailMapper));
        }
        public IEnumerable<VolumeCommitmentInfo> GetVolumeCommitmentsInfo()
        {
            return GetCachedVolumeCommitments().MapRecords(VolumeCommitmentInfoMapper);

        }
        public VolumeCommitment GetVolumeCommitment(int volumeCommitmentId)
        {
            Dictionary<int, VolumeCommitment> cachedEntities = this.GetCachedVolumeCommitments();
            return cachedEntities.GetRecord(volumeCommitmentId);
        }
        public Vanrise.Entities.InsertOperationOutput<VolumeCommitmentDetail> AddVolumeCommitment(VolumeCommitment volumeCommitment)
        {


            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<VolumeCommitmentDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            IVolumeCommitmentDataManager dataManager = BEDataManagerFactory.GetDataManager<IVolumeCommitmentDataManager>();
            int insertedId = -1;

            if (dataManager.Insert(volumeCommitment, out insertedId))
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                volumeCommitment.VolumeCommitmentId = insertedId;
                insertOperationOutput.InsertedObject = VolumeCommitmentDetailMapper(volumeCommitment);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<VolumeCommitmentDetail> UpdateVolumeCommitment(VolumeCommitment volumeCommitment)
        {


            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<VolumeCommitmentDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IVolumeCommitmentDataManager dataManager = BEDataManagerFactory.GetDataManager<IVolumeCommitmentDataManager>();

            if (dataManager.Update(volumeCommitment))
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = VolumeCommitmentDetailMapper(this.GetVolumeCommitment(volumeCommitment.VolumeCommitmentId));
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        #endregion

        #region Private Classes

        internal class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IVolumeCommitmentDataManager _dataManager = BEDataManagerFactory.GetDataManager<IVolumeCommitmentDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreVolumeCommitmentsUpdated(ref _updateHandle);
            }
        }

        #endregion

        #region Private Methods

        Dictionary<int, VolumeCommitment> GetCachedVolumeCommitments()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetVolumeCommitments", () =>
            {
                IVolumeCommitmentDataManager dataManager = BEDataManagerFactory.GetDataManager<IVolumeCommitmentDataManager>();
                IEnumerable<VolumeCommitment> switchConnectivities = dataManager.GetVolumeCommitments();
                return switchConnectivities.ToDictionary(kvp => kvp.VolumeCommitmentId, kvp => kvp);
            });
        }

        #endregion
        
        #region Mappers

        VolumeCommitmentDetail VolumeCommitmentDetailMapper(VolumeCommitment volumeCommitment)
        {
            return new VolumeCommitmentDetail()
            {
                Entity = volumeCommitment,
                CarrierAccountName = _carrierAccountManager.GetCarrierAccountName(volumeCommitment.Settings.CarrierAccountId)
            };
        }

        VolumeCommitmentInfo VolumeCommitmentInfoMapper(VolumeCommitment volumeCommitment)
        {
            return new VolumeCommitmentInfo()
            {
                VolumeCommitmentId = volumeCommitment.VolumeCommitmentId,
                Description = volumeCommitment.Settings.Description
            };
        }
        #endregion
    }
}

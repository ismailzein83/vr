using Retail.NIM.Entities;
using System;
using System.Collections.Generic;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using System.Linq;

namespace Retail.NIM.Business
{
    public class DPManager
    {
        static Guid beDefinitionId = new Guid("fc1ef5b2-dd2a-4fd1-968c-a412651377cb");

        #region Public Methods

        public DP GetDPById(long dpId)
        {
            Dictionary<long, DP> dps = this.GetCachedDPs();
            return dps.GetRecord(dpId);
        }

        public DP GetDPByNumber(string dpNumber)
        {
            Dictionary<string, DP> dpsByNumber = this.GetCachedDPsByNumber();
            return dpsByNumber.GetRecord(dpNumber);
        }

        #endregion

        #region Private Methods

        private Dictionary<string, DP> GetCachedDPsByNumber()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedDPsByNumber", beDefinitionId, () =>
            {
                Dictionary<long, DP> cachedDPs = this.GetCachedDPs();
                return cachedDPs.Values.ToDictionary(itm => itm.Number, itm => itm);
            });
        }

        private Dictionary<long, DP> GetCachedDPs()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedDPs", beDefinitionId, () =>
            {
                List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(beDefinitionId);

                Dictionary<long, DP> results = new Dictionary<long, DP>();

                if (genericBusinessEntities != null)
                {
                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                    {
                        if (genericBusinessEntity.FieldValues == null)
                            continue;

                        DP dp = new DP()
                        {
                            Id = (long)genericBusinessEntity.FieldValues.GetRecord("Id"),
                            Name = (string)genericBusinessEntity.FieldValues.GetRecord("Name"),
                            Number = (string)genericBusinessEntity.FieldValues.GetRecord("Number"),
                            Area = (long)genericBusinessEntity.FieldValues.GetRecord("Area"),
                            Site = (long)genericBusinessEntity.FieldValues.GetRecord("Site"),
                            Status = (Guid)genericBusinessEntity.FieldValues.GetRecord("Status"),
                            Region = (int)genericBusinessEntity.FieldValues.GetRecord("Region"),
                            City = (int)genericBusinessEntity.FieldValues.GetRecord("City"),
                            Town = (int)genericBusinessEntity.FieldValues.GetRecord("Town"),
                            Street = (long)genericBusinessEntity.FieldValues.GetRecord("Street"),
                            BuildingDetails = (string)genericBusinessEntity.FieldValues.GetRecord("BuildingDetails"),
                            CreatedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("CreatedTime"),
                            CreatedBy = (int)genericBusinessEntity.FieldValues.GetRecord("CreatedBy"),
                            LastModifiedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("LastModifiedTime"),
                            LastModifiedBy = (int)genericBusinessEntity.FieldValues.GetRecord("LastModifiedBy")
                        };
                        results.Add(dp.Id, dp);
                    }
                }

                return results;
            });
        }

        #endregion
    }
}
using RecordAnalysis.Entities;
using System;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.GenericData.Entities;

namespace RecordAnalysis.Business
{
    public class C4SwitchInterconnectionManager
    {
        static readonly Guid BeDefinitionId = new Guid("5439da38-8b8f-413d-9cc0-4c4e9b67e5de");

        public int? GetInterconnectionIdBySwitchIdandTrunk(int switchId, string trunk)
        {
            if (string.IsNullOrEmpty(trunk))
                return null;

            var cachedTrunkInterconnectionBySwitch = GetCachedTrunkInterconnectionBySwitch();
            if (cachedTrunkInterconnectionBySwitch == null)
                return null;

            var interconnectionByTrunk = cachedTrunkInterconnectionBySwitch.GetRecord(switchId);
            if (interconnectionByTrunk == null)
                return null;

            C4SwitchInterconnectionEntity result = interconnectionByTrunk.GetRecord(trunk.ToLower());
            if (result == null)
                return null;

            return result.InterconnectionId;
        }

        public Dictionary<int, C4SwitchInterconnectionByTrunk> GetAllTrunkInterconnectionBySwitch()
        {
            var cachedTrunkInterconnectionBySwitch = GetCachedTrunkInterconnectionBySwitch();
            cachedTrunkInterconnectionBySwitch.ThrowIfNull("cachedTrunkInterconnectionBySwitch");
            return cachedTrunkInterconnectionBySwitch;
        }

        public Dictionary<int, List<C4SwitchInterconnectionEntity>> GetC4SwitchInterconnectionEntitiesByInterconnection()
        {
            var cachedSwitchIdByInterconnection = GetCachedC4SwitchInterconnectionEntitiesByInterconnection();
            cachedSwitchIdByInterconnection.ThrowIfNull("cachedSwitchIdByInterconnection");
            return cachedSwitchIdByInterconnection;
        }

        public C4SwitchInterconnectionEntityToSave C4SwitchInterconnectionEntityToSaveMapper(Dictionary<string, object> fieldValues)
        {
            if (fieldValues == null)
                return null;

            var entity = new C4SwitchInterconnectionEntityToSave()
            {
                SwitchInterconnectionId = (int?)fieldValues.GetRecord("Id"),
                SwitchId = (int)fieldValues.GetRecord("Switch"),
                InterconnectionId = (int)fieldValues.GetRecord("Interconnection"),
            };
            entity.Settings = fieldValues.GetRecord("Settings").CastWithValidate<C4SwitchInterconnection>("Settings", entity.SwitchInterconnectionId);
            entity.Settings.Trunks.ThrowIfNull("settings.Trunks", entity.SwitchInterconnectionId);

            foreach (var trunk in entity.Settings.Trunks)
            {
                trunk.TrunkName = trunk.TrunkName.ToLower();
            }

            return entity;
        }

        #region Private Methods

        private Dictionary<int, C4SwitchInterconnectionByTrunk> GetCachedTrunkInterconnectionBySwitch()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedC4SwitchInterconnections", BeDefinitionId, () =>
            {
                var c4SwitchInterconnectionEntities = GetCachedC4SwitchInterconnectionEntity();

                var trunkInterconnectionBySwitch = new Dictionary<int, C4SwitchInterconnectionByTrunk>();

                if (c4SwitchInterconnectionEntities != null)
                {
                    foreach (var item in c4SwitchInterconnectionEntities)
                    {
                        var interconnectionByTrunk = trunkInterconnectionBySwitch.GetOrCreateItem(item.SwitchId);

                        foreach (var trunk in item.Settings.Trunks)
                        {
                            interconnectionByTrunk.Add(trunk.TrunkName, item);
                        }
                    }
                }
                return trunkInterconnectionBySwitch;
            });
        }

        private Dictionary<int, List<C4SwitchInterconnectionEntity>> GetCachedC4SwitchInterconnectionEntitiesByInterconnection()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedC4SwitchInterconnectionEntitiesByInterconnection", BeDefinitionId, () =>
            {
                var c4SwitchInterconnectionEntities = GetCachedC4SwitchInterconnectionEntity();

                var switchIdByInterconnection = new Dictionary<int, List<C4SwitchInterconnectionEntity>>();

                if (c4SwitchInterconnectionEntities != null)
                {
                    foreach (var item in c4SwitchInterconnectionEntities)
                    {
                        var idBySwitch = switchIdByInterconnection.GetOrCreateItem(item.InterconnectionId);
                        idBySwitch.Add(item);
                    }
                }
                return switchIdByInterconnection;
            });
        }

        private IEnumerable<C4SwitchInterconnectionEntity> GetCachedC4SwitchInterconnectionEntity()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedC4SwitchInterconnectionEntity", BeDefinitionId, () =>
            {
                List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(BeDefinitionId);

                var c4SwitchInterconnectionEntity = new List<C4SwitchInterconnectionEntity>();

                if (genericBusinessEntities != null)
                {
                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                    {
                        C4SwitchInterconnectionEntity item = C4SwitchInterconnectionEntityMapper(genericBusinessEntity.FieldValues);
                        if (item == null)
                            continue;

                        c4SwitchInterconnectionEntity.Add(item);
                    }
                }
                return c4SwitchInterconnectionEntity;
            });
        }

        private C4SwitchInterconnectionEntity C4SwitchInterconnectionEntityMapper(Dictionary<string, object> fieldValues)
        {
            if (fieldValues == null)
                return null;

            var entity = new C4SwitchInterconnectionEntity()
            {
                SwitchInterconnectionId = (int)fieldValues.GetRecord("Id"),
                SwitchId = (int)fieldValues.GetRecord("Switch"),
                InterconnectionId = (int)fieldValues.GetRecord("Interconnection"),
            };
            entity.Settings = fieldValues.GetRecord("Settings").CastWithValidate<C4SwitchInterconnection>("Settings", entity.SwitchInterconnectionId);
            entity.Settings.Trunks.ThrowIfNull("settings.Trunks", entity.SwitchInterconnectionId);

            foreach (var trunk in entity.Settings.Trunks)
            {
                trunk.TrunkName = trunk.TrunkName.ToLower();
            }

            return entity;
        }

        #endregion
    }
}
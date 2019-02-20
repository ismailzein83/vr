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
            var cachedTrunkInterconnectionBySwitch = GetCachedTrunkInterconnectionBySwitch();
            if (cachedTrunkInterconnectionBySwitch == null)
                return null;

            var interconnectionByTrunk = cachedTrunkInterconnectionBySwitch.GetRecord(switchId);
            if (interconnectionByTrunk == null)
                return null;

            return interconnectionByTrunk.GetRecord(trunk);
        }


        #region Private Methods

        private Dictionary<int, InterconnectionByTrunk> GetCachedTrunkInterconnectionBySwitch()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedC4SwitchInterconnections", BeDefinitionId, () =>
            {
                List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(BeDefinitionId);

                var trunkInterconnectionBySwitch = new Dictionary<int, InterconnectionByTrunk>();

                if (genericBusinessEntities != null)
                {
                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                    {
                        if (genericBusinessEntity.FieldValues == null)
                            continue;

                        var switchId = (int)genericBusinessEntity.FieldValues.GetRecord("Switch");
                        var interconnectionId = (int)genericBusinessEntity.FieldValues.GetRecord("Interconnection");

                        var interconnectionByTrunk = trunkInterconnectionBySwitch.GetOrCreateItem(switchId);
                        var settings = genericBusinessEntity.FieldValues.GetRecord("Settings").CastWithValidate<C4SwitchInterconnection>("Settings", switchId);
                        settings.Trunks.ThrowIfNull("settings.Trunks", switchId);

                        foreach (var trunk in settings.Trunks)
                        {
                            interconnectionByTrunk.Add(trunk.TrunkName, interconnectionId);
                        }
                    }
                }
                return trunkInterconnectionBySwitch;
            });
        }

        private class InterconnectionByTrunk : Dictionary<string, int> { }
        #endregion
    }
}
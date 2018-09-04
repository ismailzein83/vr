using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using Vanrise.Caching;

namespace TOne.WhS.BusinessEntity.Business
{
    public class PointOfInterconnectManager
    {
        static Guid pointOfInterconnectBEDefinitionId = new Guid("fc6e8188-d37a-4c2c-9deb-7b944ef00991");

        GenericBusinessEntityDefinitionManager _genericBusinessEntityDefinitionManager = new GenericBusinessEntityDefinitionManager();
        GenericBusinessEntityManager _genericBusinessEntityManager = new GenericBusinessEntityManager();

        public long? GetPointOfInterconnect(int switchId, string trunk)
        {
            var cachedPointOfInterconnects = GetCachedPointOfInterconnectBySwitchIdAndTrunk();
            var pointOfInterconnect = cachedPointOfInterconnects.GetRecord(string.Format("{0}_{1}", switchId, trunk));
            if (pointOfInterconnect == null)
                return null;
            return pointOfInterconnect.PointOfInterconnectEntityId;
        }

        public IEnumerable<PointOfInterconnectEntity> GetPointOfInterconnects()
        {
            var cachedPointOfInterconnects = GetCachedPointOfInterconnect();
            return cachedPointOfInterconnects.Values;
        }

        public PointOfInterconnectEntity GetPointOfInterconnect(long pointOfInterconnectEntityId)
        {
            var cachedPointOfInterconnects = GetCachedPointOfInterconnect();
            return cachedPointOfInterconnects.GetRecord(pointOfInterconnectEntityId);
        }

        Dictionary<string, PointOfInterconnectEntity> GetCachedPointOfInterconnectBySwitchIdAndTrunk()
        {
            return _genericBusinessEntityManager.GetCachedOrCreate("GetCachedPointOfInterconnectBySwitchIdAndTrunk", pointOfInterconnectBEDefinitionId,
                () =>
                {
                    Dictionary<string, PointOfInterconnectEntity> pointOfInterconnectBySwitchIdAndtrunk = new Dictionary<string, PointOfInterconnectEntity>();

                    Dictionary<long, PointOfInterconnectEntity> pointOfInterconnectsById = GetCachedPointOfInterconnect();
                    foreach (var pointOfInterconnectEntityKvp in pointOfInterconnectsById)
                    {
                        var pointOfInterconnectEntity = pointOfInterconnectEntityKvp.Value;
                        foreach (var trunk in pointOfInterconnectEntity.Settings.Trunks)
                        {
                            string key = string.Format("{0}_{1}", pointOfInterconnectEntity.SwitchId, trunk.Trunk);
                            if (!pointOfInterconnectBySwitchIdAndtrunk.ContainsKey(key))
                                pointOfInterconnectBySwitchIdAndtrunk.Add(key, pointOfInterconnectEntity);
                        }
                    }
                    return pointOfInterconnectBySwitchIdAndtrunk;
                });
        }

        Dictionary<long, PointOfInterconnectEntity> GetCachedPointOfInterconnect()
        {
            return _genericBusinessEntityManager.GetCachedOrCreate("GetCachedPointOfInterconnect", pointOfInterconnectBEDefinitionId,
                () =>
                {
                    Dictionary<long, PointOfInterconnectEntity> pointOfInterconnectsById = new Dictionary<long, PointOfInterconnectEntity>();

                    var idFieldType = _genericBusinessEntityDefinitionManager.GetIdFieldTypeForGenericBE(pointOfInterconnectBEDefinitionId);
                    idFieldType.ThrowIfNull("idFieldType");
                    idFieldType.Name.ThrowIfNull("idFieldType.Name");
                    var pointOfInterconnects = _genericBusinessEntityManager.GetAllGenericBusinessEntities(pointOfInterconnectBEDefinitionId);
                    if (pointOfInterconnects != null)
                    {
                        foreach (var pointOfInterconnect in pointOfInterconnects)
                        {
                            PointOfInterconnectEntity pointOfInterconnectEntity = new PointOfInterconnectEntity();

                            Object currentSwitchId = -1;
                            pointOfInterconnect.FieldValues.TryGetValue("SwitchId", out currentSwitchId);
                            pointOfInterconnectEntity.SwitchId = currentSwitchId.CastWithValidateStruct<int>("SwitchId");

                            Object pointOfInterConnectId;
                            pointOfInterconnect.FieldValues.TryGetValue(idFieldType.Name, out pointOfInterConnectId);
                            pointOfInterconnectEntity.PointOfInterconnectEntityId = pointOfInterConnectId.CastWithValidateStruct<long>("pointOfInterConnectId");

                            Object name = null;
                            pointOfInterconnect.FieldValues.TryGetValue("Name", out name);
                            pointOfInterconnectEntity.Name = name.CastWithValidate<string>("name");

                            Object currentPointOfInterconnect = -1;
                            pointOfInterconnect.FieldValues.TryGetValue("TrunkDetails", out currentPointOfInterconnect);
                            pointOfInterconnectEntity.Settings = currentPointOfInterconnect.CastWithValidate<PointOfInterconnect>("trunks");
                            pointOfInterconnectEntity.Settings.Trunks.ThrowIfNull("pointOfInterconnectEntity.Settings.Trunks");

                            pointOfInterconnectsById.Add(pointOfInterconnectEntity.PointOfInterconnectEntityId, pointOfInterconnectEntity);
                        }
                    }
                    return pointOfInterconnectsById;
                });
        }
    }
}

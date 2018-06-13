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
        public long? GetPointOfInterconnect(int switchId, string truck)
        {
            var cachedPointOfInterconnects = GetCachedPointOfInterconnectBySwitchIdAndTruck();
            var pointOfInterconnect = cachedPointOfInterconnects.GetRecord(string.Format("{0}_{1}", switchId, truck));
            if (pointOfInterconnect == null)
                return null;
            return pointOfInterconnect.PointOfInterconnectEntityId;
        }

        public Dictionary<string, PointOfInterconnectEntity> GetCachedPointOfInterconnectBySwitchIdAndTruck()
        {
            return _genericBusinessEntityManager.GetCachedOrCreate("GetCachedPointOfInterconnectBySwitchIdAndTruck", pointOfInterconnectBEDefinitionId,
                () =>
                {
                    Dictionary<string, PointOfInterconnectEntity> pointOfInterconnectBySwitchIdAndTruck = new Dictionary<string, PointOfInterconnectEntity>();

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

                            foreach (var trunk in pointOfInterconnectEntity.Settings.Trunks)
                            {
                                string key = string.Format("{0}_{1}", pointOfInterconnectEntity.SwitchId, trunk.Trunk);
                                if (!pointOfInterconnectBySwitchIdAndTruck.ContainsKey(key))
                                    pointOfInterconnectBySwitchIdAndTruck.Add(key, pointOfInterconnectEntity);
                            }
                        }
                    }
                    return pointOfInterconnectBySwitchIdAndTruck;
                });
        }
    }
}

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

namespace TOne.WhS.BusinessEntity.Business
{
    public class PointOfInterconnectManager
    {
        static Guid pointOfInterconnectBEDefinitionId = new Guid("fc6e8188-d37a-4c2c-9deb-7b944ef00991");
        public long? GetPointOfInterconnect(int switchId, string truck)
        {
            GenericBusinessEntityManager genericBusinessEntityManager = new GenericBusinessEntityManager();
            var pointOfInterconnects = genericBusinessEntityManager.GetAllGenericBusinessEntities(pointOfInterconnectBEDefinitionId);
            if (pointOfInterconnects != null)
            {
                GenericBusinessEntityDefinitionManager genericBusinessEntityDefinitionManager = new GenericBusinessEntityDefinitionManager();
                foreach (var pointOfInterconnect in pointOfInterconnects)
                {
                    Object currentSwitchId = -1;
                    Object currentPointOfInterconnect = null;
                    if (pointOfInterconnect.FieldValues.TryGetValue("SwitchId", out currentSwitchId) && pointOfInterconnect.FieldValues.TryGetValue("TrunkDetails", out currentPointOfInterconnect))
                    {
                        int castedSwitchId = (int)currentSwitchId;
                        if (castedSwitchId != switchId)
                            continue;

                        PointOfInterconnect castedPointOfInterconnect = currentPointOfInterconnect as PointOfInterconnect;
                        if (castedPointOfInterconnect != null && castedPointOfInterconnect.Trunks != null && castedPointOfInterconnect.Trunks.Any(x => x.Trunk == truck))
                        {
                            var idFieldType = genericBusinessEntityDefinitionManager.GetIdFieldTypeForGenericBE(pointOfInterconnectBEDefinitionId);
                            idFieldType.ThrowIfNull("idFieldType");
                            idFieldType.Name.ThrowIfNull("idFieldType.Name");
                            Object pointOfInterConnectId;
                            if (pointOfInterconnect.FieldValues.TryGetValue(idFieldType.Name, out pointOfInterConnectId))
                                return (long)pointOfInterConnectId;
                        }

                    }
                }

            }
            return null;
        }
    }
}

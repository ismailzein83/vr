using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class VRObjectEvaluator
    {
        Dictionary<string, VRObjectVariable> objectVariables = new Dictionary<string, VRObjectVariable>();
        Dictionary<string, dynamic> objects;

        public VRObjectEvaluator(VRObjectVariableCollection vrObjectVariableCollection, Dictionary<string, dynamic> objects) 
        {
            this.objectVariables = vrObjectVariableCollection;
            this.objects = objects;
        }

        public dynamic GetPropertyValue(string objectName, string propertyName)
        {
            dynamic obj;
            VRObjectType objectType;
            VRObjectTypePropertyDefinition objectTypePropertyDefinition;
            dynamic propertyValue = null;

            GetObjectAndObjectType(objectName, propertyName, out obj, out objectType, out objectTypePropertyDefinition);

            var propertyEvaluatorContext = new VRObjectPropertyEvaluatorContext
            {
                Object = obj,
                ObjectType = objectType
            };
            
            if(objectTypePropertyDefinition != null)
                propertyValue = objectTypePropertyDefinition.PropertyEvaluator.GetPropertyValue(propertyEvaluatorContext);

            return propertyValue;
        }

        private void GetObjectAndObjectType(string objectName, string propertyName, out dynamic obj, out VRObjectType objectType, out VRObjectTypePropertyDefinition objectTypePropertyDefinition)
        {
            obj = null;
            objectType = null;
            objectTypePropertyDefinition = null;
            VRObjectVariable objectVariable = null;

            if (objectVariables.TryGetValue(objectName, out objectVariable))
            {
                VRObjectTypeDefinition objectTypeDefinition = GetObjectTypeDefinition(objectVariable.VRObjectTypeDefinitionId);
                if (objectTypeDefinition != null)
                    objectType = objectTypeDefinition.Settings.ObjectType;

                if (!objectTypeDefinition.Settings.Properties.TryGetValue(propertyName, out objectTypePropertyDefinition))
                    throw new NullReferenceException(String.Format("objectTypePropertyDefinition '{0}'", objectVariable.VRObjectTypeDefinitionId));

                //if object is null GetDefaultValue
                if (!objects.TryGetValue(objectName, out obj))
                    obj = objectType.GetDefaultValue();
                
            }
            else
            {
                throw new NullReferenceException(String.Format("objectVariable '{0}'", objectName));
            }
        }

        private VRObjectTypeDefinition GetObjectTypeDefinition(Guid objectTypeDefinitionId)
        {
            VRObjectTypeDefinition objectTypeDefinition = new VRObjectTypeDefinitionManager().GetVRObjectTypeDefinition(objectTypeDefinitionId);
            
            if (objectTypeDefinition == null)
                throw new NullReferenceException(String.Format("objectTypeDefinition '{0}'", objectTypeDefinitionId));
            if (objectTypeDefinition.Settings == null)
                throw new NullReferenceException(String.Format("objectTypeDefinition.Settings '{0}'", objectTypeDefinitionId));

            return objectTypeDefinition;
        }

    }
}

using System;
using System.Collections.Generic;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class VRObjectEvaluator
    {
        Dictionary<string, VRObjectVariable> objectVariables = new Dictionary<string, VRObjectVariable>();
        Dictionary<string, dynamic> objects;

        VRObjectTypeDefinitionManager vrObjectTypeDefinitionManager = new VRObjectTypeDefinitionManager();

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
            GetObjectAndObjectType(objectName, propertyName, out obj, out objectType, out objectTypePropertyDefinition);

            objectTypePropertyDefinition.PropertyEvaluator.ThrowIfNull("objectTypePropertyDefinition.PropertyEvaluator");

            var propertyEvaluatorContext = new VRObjectPropertyEvaluatorContext { Object = obj, ObjectType = objectType };
            dynamic propertyValue = objectTypePropertyDefinition.PropertyEvaluator.GetPropertyValue(propertyEvaluatorContext);
            return propertyValue;
        }

        private void GetObjectAndObjectType(string objectName, string propertyName, out dynamic obj, out VRObjectType objectType, out VRObjectTypePropertyDefinition objectTypePropertyDefinition)
        {
            obj = null;
            objectType = null;
            objectTypePropertyDefinition = null;

            VRObjectVariable objectVariable = null;
            if (!objectVariables.TryGetValue(objectName, out objectVariable))
                throw new NullReferenceException($"objectVariable '{objectName}'");

            VRObjectTypeDefinition objectTypeDefinition = GetObjectTypeDefinition(objectVariable.VRObjectTypeDefinitionId);
            objectTypeDefinition.ThrowIfNull("objectTypeDefinition", objectVariable.VRObjectTypeDefinitionId);
            objectTypeDefinition.Settings.ThrowIfNull("objectTypeDefinition.Settings", objectVariable.VRObjectTypeDefinitionId);
            objectTypeDefinition.Settings.ObjectType.ThrowIfNull("objectTypeDefinition.Settings.ObjectType", objectVariable.VRObjectTypeDefinitionId);

            objectType = objectTypeDefinition.Settings.ObjectType;

            if (!objectTypeDefinition.Settings.Properties.TryGetValue(propertyName, out objectTypePropertyDefinition))
                throw new NullReferenceException($"objectTypePropertyDefinition '{propertyName}' of objectTypeDefinition '{objectVariable.VRObjectTypeDefinitionId}'");

            if (objectTypePropertyDefinition.PropertyEvaluator == null)
                throw new NullReferenceException($"objectTypePropertyDefinition.PropertyEvaluator '{propertyName}' of objectTypeDefinition '{objectVariable.VRObjectTypeDefinitionId}'");

            //if object is null GetDefaultValue
            if (!objects.TryGetValue(objectName, out obj))
                obj = objectType.GetDefaultValue();
        }

        private VRObjectTypeDefinition GetObjectTypeDefinition(Guid objectTypeDefinitionId)
        {
            VRObjectTypeDefinition objectTypeDefinition = vrObjectTypeDefinitionManager.GetVRObjectTypeDefinition(objectTypeDefinitionId);
            objectTypeDefinition.ThrowIfNull("objectTypeDefinition", objectTypeDefinitionId);
            objectTypeDefinition.Settings.ThrowIfNull("objectTypeDefinition.Settings", objectTypeDefinitionId);
            return objectTypeDefinition;
        }
    }
}
using System.Collections.Generic;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class VRObjectExpressionEvaluator
    {
        VRObjectEvaluator vrObjectEvaluator;

        public VRObjectExpressionEvaluator(VRObjectVariableCollection objectVariables, Dictionary<string, dynamic> objects)
        {
            vrObjectEvaluator = new VRObjectEvaluator(objectVariables, objects);
        }

        public dynamic GetVal(string objectName, string propertyName)
        {
            return vrObjectEvaluator.GetPropertyValue(objectName, propertyName);
        }

    }
}

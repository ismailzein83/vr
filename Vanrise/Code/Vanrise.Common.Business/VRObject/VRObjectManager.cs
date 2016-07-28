using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class VRObjectManager
    {
        public Dictionary<string, dynamic> EvaluateVariables(List<VRObjectPropertyVariable> Variables, Dictionary<string, dynamic> objects, Dictionary<string, VRObjectVariable> objectVariables)
        {
            Dictionary<string, dynamic> variableValues = new Dictionary<string, dynamic>();
            foreach (var variable in Variables)
            {
                if (variableValues.ContainsKey(variable.VariableName))
                    throw new Exception(String.Format("Duplicate Variable Name '{0}'", variable.VariableName));
                dynamic obj;
                VRObjectType objectType;
                GetObjectAndType(variable, objects, objectVariables, out obj, out objectType);
                var propertyEvaluatorContext = new VRObjectPropertyEvaluatorContext
                {
                    Object = obj,
                    ObjectType = objectType
                };
                dynamic variableValue = variable.PropertyEvaluator.GetPropertyValue(propertyEvaluatorContext);
                variableValues.Add(variable.VariableName, variableValue);
            }
            return variableValues;
        }

        private void GetObjectAndType(VRObjectPropertyVariable variable, Dictionary<string, dynamic> objects, Dictionary<string, VRObjectVariable> objectVariables, out dynamic obj, out VRObjectType objectType)
        {
            if (!objects.TryGetValue(variable.ObjectName, out obj))
                throw new NullReferenceException(String.Format("Obj. ObjectName '{0}', VariableName '{1}'", variable.ObjectName, variable.VariableName));
            if (variable.PropertyEvaluator == null)
                throw new NullReferenceException(String.Format("variable.PropertyEvaluator. VariableName '{0}'", variable.VariableName));

            VRObjectVariable objectVariable;
            if (!objectVariables.TryGetValue(variable.ObjectName, out objectVariable))
                throw new NullReferenceException(String.Format("objectVariable. ObjectName '{0}', VariableName '{1}'", variable.ObjectName, variable.VariableName));
            if (objectVariable.ObjectType == null)
                throw new NullReferenceException(String.Format("objectVariable.ObjectType. ObjectName '{0}', VariableName '{1}'", variable.ObjectName, variable.VariableName));
            objectType = objectVariable.ObjectType;
        }
    }
}

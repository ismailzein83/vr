using System;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace Vanrise.BusinessProcess.MainExtensions.VRWorkflowVariableTypes
{
    public class VRWorkflowCustomClassType : VRWorkflowVariableType
    {
        public override Guid ConfigId { get { return new Guid("A6078B0f-EFa2-414F-8a25-549628DA1762"); } }
        public VRCustomClassType FieldType { get; set; }

        public override Type GetRuntimeType(IVRWorkflowVariableTypeGetRuntimeTypeContext context)
        {
            if (string.IsNullOrEmpty(this.FieldType.Namespace))
                throw new NullReferenceException("FieldType.Namespace");

            if (string.IsNullOrEmpty(this.FieldType.ClassName))
                throw new NullReferenceException("FieldType.ClassName");

            Type outputType = null;
            if (string.IsNullOrEmpty(this.FieldType.AssemblyName))
            {
                VRNamespaceManager vrNamespaceManager = new VRNamespaceManager();
                outputType = vrNamespaceManager.GetNamespaceType(this.FieldType.Namespace, this.FieldType.ClassName);

                if (outputType == null)
                    outputType = Type.GetType(string.Format("{0}.{1}", this.FieldType.Namespace, this.FieldType.ClassName));
            }
            else
            {
                outputType = Type.GetType(string.Format("{0}.{1}, {2}", this.FieldType.Namespace, this.FieldType.ClassName, this.FieldType.AssemblyName));
            }

            if (outputType == null)
            {
                string assemblyOutput = string.IsNullOrEmpty(this.FieldType.AssemblyName) ? "" : string.Format(", Assembly Name: '{0}'", this.FieldType.AssemblyName);
                throw new Exception(String.Format("Unable to get Type for Namespace :'{0}', Class name: '{1}'{2}", this.FieldType.Namespace, this.FieldType.ClassName, assemblyOutput));
            }

            return outputType;
        }

        public override string GetRuntimeTypeDescription()
        {
            return string.Format("{0}.{1}", this.FieldType.Namespace, this.FieldType.ClassName);
        }
    }
}

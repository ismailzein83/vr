using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace Vanrise.GenericData.Business
{
    public class CustomClassBusinessObjectDataProvider : BusinessObjectDataProviderExtendedSettings
    {
        public override Guid ConfigId => new Guid("7184113C-A776-4CFC-B445-2226AD252506");

        public Vanrise.Entities.VRCustomClassType FieldType { get; set; }

        BusinessObjectDataProviderExtendedSettings GetCustomClassBODataProvider()
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

            var customClassBPDataProviderObj = Activator.CreateInstance(outputType).CastWithValidate<BusinessObjectDataProviderExtendedSettings>("customClassBPDataProviderObj", $"{this.FieldType.Namespace}.{this.FieldType.ClassName}");
            return customClassBPDataProviderObj;
        }

        public override bool DoesSupportFilterOnAllFields => GetCustomClassBODataProvider().DoesSupportFilterOnAllFields;

        public override void LoadRecords(IBusinessObjectDataProviderLoadRecordsContext context)
        {
            GetCustomClassBODataProvider().LoadRecords(context);
        }
    }
}

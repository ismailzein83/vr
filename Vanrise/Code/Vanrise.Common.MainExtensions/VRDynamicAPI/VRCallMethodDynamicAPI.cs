using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Common.MainExtensions.VRDynamicCode;
using Vanrise.Entities;
using Vanrise.Common;

namespace Vanrise.Common.MainExtensions.VRDynamicAPI
{
    public class VRCallMethodDynamicAPI : VRDynamicAPIMethodSettings
    {
        public override Guid ConfigId { get { return new Guid("AAC62543-CF85-4F0D-BB42-121C7B699816"); } }
        public Guid NamespaceId { get; set; }
        public string ClassName { get; set; }
        public string MethodName { get; set; }
        public VRDynamicAPIMethodType MethodType { get; set; }
        public override void Evaluate(IVRDynamicAPIMethodSettingsContext vrDynamicAPIMethodSettingsContext)
        {
            VRNamespaceManager vrNamespaceManager = new VRNamespaceManager();
            var vrNamespace = vrNamespaceManager.GetVRNamespace(NamespaceId);
            var classMethods = vrNamespaceManager.GetAssemblyClassMethods(NamespaceId, ClassName);
            var method = classMethods.GetRecord(MethodName);
            if (method != null)
            {
                var parameters = method.GetParameters();
                StringBuilder parametersBuilder = new StringBuilder();
                if(parameters!=null && parameters.Count() > 0)
                {
                    List<VRDynamicAPIMethodParameter> inParameters = new List<VRDynamicAPIMethodParameter>();
                    foreach(var parameter in parameters)
                    {
                        inParameters.Add(new VRDynamicAPIMethodParameter()
                        {
                            ParameterName = parameter.Name,
                            ParameterType = CSharpCompiler.TypeToString(parameter.ParameterType)
                        });

                        if (parametersBuilder.Length > 0)
                        {
                            parametersBuilder.Append(", ");
                        }
                        parametersBuilder.Append(parameter.Name);
                    }
                    vrDynamicAPIMethodSettingsContext.InParameters = inParameters;
                }
                string returnedType = CSharpCompiler.TypeToString(method.ReturnType);
                vrDynamicAPIMethodSettingsContext.ReturnType = returnedType;

                string returnedValue = string.Compare(returnedType, "void", true) != 0 ? "return " : string.Empty;

                StringBuilder methodBodyBuilder = new StringBuilder();
                if (method.IsStatic)
                {
                    methodBodyBuilder.Append($@"
                    {returnedValue} #NAMESPACE#.#CLASS#.#METHOD#(#PARAMETERS#);
                    ");
                }
                else if (!method.IsStatic && method.IsPublic)
                {
                    methodBodyBuilder.Append($@"
                    #NAMESPACE#.#CLASS# x = new #NAMESPACE#.#CLASS#();
                    {returnedValue} x.#METHOD#(#PARAMETERS#);
                    ");
                }
                methodBodyBuilder.Replace("#NAMESPACE#", vrNamespace.Name);
                methodBodyBuilder.Replace("#CLASS#", ClassName);
                methodBodyBuilder.Replace("#METHOD#", MethodName);
                methodBodyBuilder.Replace("#PARAMETERS#", parametersBuilder.ToString());
                vrDynamicAPIMethodSettingsContext.MethodBody = methodBodyBuilder.ToString();
                vrDynamicAPIMethodSettingsContext.MethodType = MethodType;
            }
        }
    }
}

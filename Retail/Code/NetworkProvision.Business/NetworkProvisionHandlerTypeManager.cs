using NetworkProvision.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace NetworkProvision.Business
{
    public class NetworkProvisionHandlerTypeManager
    {
        public IEnumerable<NetworkProvisionHandlerTypeExtendedSettingsConfig> GetHandlerTypeExtendedSettingsConfigs()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<NetworkProvisionHandlerTypeExtendedSettingsConfig>(NetworkProvisionHandlerTypeExtendedSettingsConfig.EXTENSION_TYPE);
        }

        public CustomCodeCompilationOutput TryCompileCustomCode(CustomCodeNetworkProvisionHandlerType customCode)
        {
            List<string> customCodeErrors;

            bool compilationResult = TryCompileCustomCode(customCode, out customCodeErrors);

            if (compilationResult)
            {
                return new CustomCodeCompilationOutput
                {
                    CustomCodeErrors = null,
                    Result = true
                };
            }
            else
            {
                return new CustomCodeCompilationOutput
                {
                    CustomCodeErrors = customCodeErrors,
                    Result = false
                };
            }
        }

        private bool TryCompileCustomCode(CustomCodeNetworkProvisionHandlerType customCode, out List<string> customCodeErrors)
        {
            StringBuilder codeBuilder = new StringBuilder(@" 
                using System;
                using System.Collections.Generic;
                using System.Linq;
                using Vanrise.Common;
                using Vanrise.BusinessProcess;

                namespace #NAMESPACE#
                {
                    #NAMESPACEMEMBERS#

                    public class #CLASSNAME#
                    {
                        #MAINFUNCTION#
                    }
                }");

            customCode.ThrowIfNull("customCode");
            if (customCode.NamespaceMembers != null)
                codeBuilder.Replace("#NAMESPACEMEMBERS#", customCode.NamespaceMembers);
            else
                codeBuilder.Replace("#NAMESPACEMEMBERS#", "");

            if (customCode.CustomCode != null)
                codeBuilder.Replace("#MAINFUNCTION#", customCode.CustomCode);
            else
                codeBuilder.Replace("#MAINFUNCTION#", "");

            string classNamespace = CSharpCompiler.GenerateUniqueNamespace("NetworkProvision.Business.NetworkProvisionHandlerTypeManager");
            string className = "Main";
            codeBuilder.Replace("#NAMESPACE#", classNamespace);
            codeBuilder.Replace("#CLASSNAME#", className);
            var fullTypeName = string.Format("{0}.{1}", classNamespace, className);

            CSharpCompilationOutput compilationOutput;
            if (CSharpCompiler.TryCompileClass(className, codeBuilder.ToString(), out compilationOutput))
            {
                customCodeErrors = null;
                return true;
            }
            else
            {
                customCodeErrors = compilationOutput.Errors.Select(item => item.ErrorText).ToList();
                return false;
            }
        }
    }

    public class CustomCodeCompilationOutput
    {
        public List<string> CustomCodeErrors { get; set; }
        public bool Result { get; set; }
    }
}
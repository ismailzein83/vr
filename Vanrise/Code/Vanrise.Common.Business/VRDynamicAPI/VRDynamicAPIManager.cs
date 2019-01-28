using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Common.Data;
using Vanrise.Caching;
using Vanrise.Security.Entities;
using Vanrise.Common;
namespace Vanrise.Common.Business
{
    public class VRDynamicAPIManager
    {
        static CacheManager s_cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>();

        CacheManager GetCacheManager()
        {
            return s_cacheManager;
        }

        VRDynamicAPIModuleManager vrDynamicAPIModuleManager = new VRDynamicAPIModuleManager();

        #region Public Methods
        public IDataRetrievalResult<VRDynamicAPIDetails> GetFilteredVRDynamicAPIs(DataRetrievalInput<VRDynamicAPIQuery> input)
        {
            var allVRDynamicAPIs = GetCachedVRDynamicAPIs();
            Func<VRDynamicAPI, bool> filterExpression = (vrDynamicAPI) =>
            {
                if (input.Query.VRDynamicAPIModuleId.HasValue && !(vrDynamicAPI.ModuleId == input.Query.VRDynamicAPIModuleId))
                    return false;
                return true;
            };
            VRActionLogger.Current.LogGetFilteredAction(VRDynamicAPILoggableEntity.Instance, input);
            return DataRetrievalManager.Instance.ProcessResult(input, allVRDynamicAPIs.ToBigResult(input, filterExpression, VRDynamicAPIDetailMapper));

        }

        public IEnumerable<VRDynamicAPIMethodConfig> GetVRDynamicAPIMethodSettingsConfigs()
        {
            var extensionConfigurationManager = new ExtensionConfigurationManager();
            return extensionConfigurationManager.GetExtensionConfigurations<VRDynamicAPIMethodConfig>(VRDynamicAPIMethodConfig.EXTENSION_TYPE);
        }

        public InsertOperationOutput<VRDynamicAPIDetails> AddVRDynamicAPI(VRDynamicAPI vrDynamicAPI)
        {
            IVRDynamicAPIDataManager vrDynamicAPIDataManager = CommonDataManagerFactory.GetDataManager<IVRDynamicAPIDataManager>();
            InsertOperationOutput<VRDynamicAPIDetails> insertOperationOutput = new InsertOperationOutput<VRDynamicAPIDetails>();
            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            vrDynamicAPI.VRDynamicAPIId = Guid.NewGuid();
            int loggedInUserId = ContextFactory.GetContext().GetLoggedInUserId();
            vrDynamicAPI.CreatedBy = loggedInUserId;
            vrDynamicAPI.LastModifiedBy = loggedInUserId;

            bool insertActionSuccess = vrDynamicAPIDataManager.Insert(vrDynamicAPI);
            if (insertActionSuccess)
            {
                GetCacheManager().SetCacheExpired();
                VRActionLogger.Current.TrackAndLogObjectAdded(VRDynamicAPILoggableEntity.Instance, vrDynamicAPI);
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = VRDynamicAPIDetailMapper(vrDynamicAPI);
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }
            return insertOperationOutput;
        }

        public VRDynamicAPI GetVRDynamicAPIById(Guid vrDynamicAPIId)
        {
            var allVRDynamicAPIs = GetCachedVRDynamicAPIs();
            return allVRDynamicAPIs.GetRecord(vrDynamicAPIId);
        }

        public string GetVRDynamicAPIName(VRDynamicAPI vrDynamicAPI)
        {
            return (vrDynamicAPI != null) ? vrDynamicAPI.Name : null;
        }
        public UpdateOperationOutput<VRDynamicAPIDetails> UpdateVRDynamicAPI(VRDynamicAPI vrDynamicAPI)
        {
            IVRDynamicAPIDataManager vrDynamicAPIDataManager = CommonDataManagerFactory.GetDataManager<IVRDynamicAPIDataManager>();
            vrDynamicAPI.LastModifiedBy = ContextFactory.GetContext().GetLoggedInUserId();
            UpdateOperationOutput<VRDynamicAPIDetails> updateOperationOutput = new UpdateOperationOutput<VRDynamicAPIDetails>();
            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;
            bool updateActionSuccess = vrDynamicAPIDataManager.Update(vrDynamicAPI);
            if (updateActionSuccess)
            {
                GetCacheManager().SetCacheExpired();
                VRActionLogger.Current.TrackAndLogObjectUpdated(VRDynamicAPILoggableEntity.Instance, vrDynamicAPI);
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = VRDynamicAPIDetailMapper(vrDynamicAPI);
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }
            return updateOperationOutput;
        }

        public VRDynamicAPICompilationResult TryCompileVRDynamicAPI(VRDynamicAPI vrDynamicAPI)
        {
            List<string> Errors;
            GetOrCreateAPIClass(vrDynamicAPI, out Errors);
            VRDynamicAPICompilationResult vrDynamicAPICompilationResult = new VRDynamicAPICompilationResult();
            if (Errors == null)
            {
                vrDynamicAPICompilationResult.isSucceeded = true;
                return vrDynamicAPICompilationResult;
            }
            else

                vrDynamicAPICompilationResult.isSucceeded = false;
            vrDynamicAPICompilationResult.Errors= Errors;
            return vrDynamicAPICompilationResult;

        }

        #endregion

        #region Private Classes
        public class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IVRDynamicAPIDataManager vrDynamicAPIDataManager = CommonDataManagerFactory.GetDataManager<IVRDynamicAPIDataManager>();
            object _updateHandle;
            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return vrDynamicAPIDataManager.AreVRDynamicAPIsUpdated(ref _updateHandle);
            }
        }
        #endregion

        #region Private Methods
       
        private Dictionary<Guid, VRDynamicAPI> GetCachedVRDynamicAPIs()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>()
               .GetOrCreateObject("GetCachedVRDynamicAPIs", () =>
               {
                   IVRDynamicAPIDataManager vrDynamicAPIDataManager = CommonDataManagerFactory.GetDataManager<IVRDynamicAPIDataManager>();
                   List<VRDynamicAPI> vrDynamicAPIs = vrDynamicAPIDataManager.GetVRDynamicAPIs();
                   return vrDynamicAPIs.ToDictionary(vrDynamicAPI => vrDynamicAPI.VRDynamicAPIId, vrDynamicAPI => vrDynamicAPI);
               });
        }


        private Type GetOrCreateAPIClass(VRDynamicAPI vrDynamicAPI, out List<string> Errors)
        {
            string fullTypeName;
            Errors = null;
            var classDefinition = BuildAPIClassController(vrDynamicAPI, out fullTypeName);

            CSharpCompilationOutput compilationOutput;
            if (!CSharpCompiler.TryCompileClass(String.Format("APIClass_{0}", vrDynamicAPI.Name), classDefinition, out compilationOutput))
            {
                Errors = new List<string>();
                Errors = compilationOutput.ErrorMessages;
                return null;
            }
            else
                return compilationOutput.OutputAssembly.GetType(fullTypeName);
        }

        public List<Type> BuildAllDynamicAPIControllers()
        {
            List<Type> types = new List<Type>();
            VRDynamicAPIModuleManager vrDynamicAPIModuleManager = new VRDynamicAPIModuleManager();
            var allAPIs = GetCachedVRDynamicAPIs();
            foreach (var api in allAPIs)
            {
                var apiModuleName = vrDynamicAPIModuleManager.GetVRDynamicAPIModuleName(api.Value.ModuleId);
                apiModuleName.ThrowIfNull("moduleName");

                Common.CSharpCompilationOutput output;
                string fullTypeName;
                if (Common.CSharpCompiler.TryCompileClass(BuildAPIClassController(api.Value, out fullTypeName), out output))
                {
                    types.Add(output.OutputAssembly.GetType(fullTypeName));
                }
            } 
            return types;
        }
        public string BuildAPIClassController(VRDynamicAPI vrDynamicAPI, out string fullTypeName)
        {
            ISecurityManager securityManager = BEManagerFactory.GetManager<ISecurityManager>();
            vrDynamicAPI.Name.ThrowIfNull("vrDynamicAPI.Name");
            vrDynamicAPI.Settings.ThrowIfNull("vrDynamicAPI.Settings");

            StringBuilder classControllerBuilder = new StringBuilder(@" 
              using System;
              using System.Collections.Generic;
              using System.Linq;
              using System.Web;
              using System.Web.Http;
              using Vanrise.Common.Business;
              using Vanrise.Web.Base;
              using Vanrise.Entities;
              using Vanrise.Security.Entities;
              using Vanrise.Security.Business;

              namespace #NAMESPACE#
              {
                 [RoutePrefix(""#RoutePrefix#"" + ""#ControllerName#"")]
                 public class #CLASSNAME# : BaseAPIController
                 { 
                    #Methods#

                    private System.Net.Http.HttpResponseMessage GetUnauthorizedResponse()
                    {
                        System.Net.Http.HttpResponseMessage msg = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Forbidden);
                        msg.Content = new System.Net.Http.StringContent(""you are not authorized to perform this request"");
                        return msg;
                    }
                 }
              } 
           ");

            StringBuilder methodsBuilder = new StringBuilder();

            foreach (var method in vrDynamicAPI.Settings.Methods)
            {
                var context = new VRDynamicAPIMethodSettingsContext();
                methodsBuilder.AppendLine();
                method.Settings.Evaluate(context);

                StringBuilder methodBuilder = new StringBuilder(@" 
                [Http#MethodType#]
                [Route(""#MethodName#"")]
                public #ReturnType# #MethodName#(#InParameters#)
                {
                    #Permission#
                    #MethodBody#
                    #returnNull#
                }");

                if (context.MethodType == VRDynamicAPIMethodType.Get && context.ReturnType == null)
                    context.ReturnType.ThrowIfNull("context.ReturnType");
                 

                StringBuilder parametersBuilder = new StringBuilder();

                if (context.InParameters != null)
                    parametersBuilder.Append(string.Join(",", context.InParameters.MapRecords(x => string.Format("{0} {1}", x.ParameterType, x.ParameterName))));

                StringBuilder permissionsBuilder = new StringBuilder();

                permissionsBuilder.Append(@"
                     SecurityManager securityManager = new SecurityManager();");

                if (vrDynamicAPI.Settings.Security != null && vrDynamicAPI.Settings.Security.RequiredPermissions != null)
                {
                    RequiredPermissionSettings apiPermissions = vrDynamicAPI.Settings.Security.RequiredPermissions.CastWithValidate<RequiredPermissionSettings>("vrDynamicAPI.Settings.Security.RequiredPermissions");
                    if (apiPermissions.Entries.Count != 0)
                    {
                        methodBuilder.Replace("#ReturnType#", "object");

                        permissionsBuilder.Append(@"
                    if (securityManager.IsAllowed(""#requiredPermissionString#"", ContextFactory.GetContext().GetLoggedInUserId())==false)
                        {return this.GetUnauthorizedResponse();}");

                        permissionsBuilder.Replace("#requiredPermissionString#", securityManager.RequiredPermissionsToString(apiPermissions.Entries).ToString());
                        if (context.ReturnType == null || context.ReturnType=="void")
                        methodBuilder.Replace("#returnNull#", "return null;");
                        else methodBuilder.Replace("#returnNull#", "");
                    }
                }
                if (method.Security != null && method.Security.RequiredPermissions != null)
                {
                    RequiredPermissionSettings methodPermissions = method.Security.RequiredPermissions.CastWithValidate<RequiredPermissionSettings>("method.Security.RequiredPermissions");
                    if (methodPermissions.Entries.Count != 0)
                    {
                        methodBuilder.Replace("#ReturnType#", "object");

                        permissionsBuilder.Append(@"
                    if (securityManager.IsAllowed(""#requiredPermissionString#"", ContextFactory.GetContext().GetLoggedInUserId())==false)
                        {return this.GetUnauthorizedResponse();}
                           ");

                        permissionsBuilder.Replace("#requiredPermissionString#", securityManager.RequiredPermissionsToString(methodPermissions.Entries).ToString());
                        if(context.ReturnType==null || context.ReturnType == "void")
                        methodBuilder.Replace("#returnNull#","return null;" );
                        else methodBuilder.Replace("#returnNull#", "");

                    }
                }

                if (context.ReturnType == null)
                    context.ReturnType = "void";
                methodBuilder.Replace("#ReturnType#", context.ReturnType);

                methodBuilder.Replace("#MethodType#", context.MethodType.ToString());

                methodBuilder.Replace("#MethodName#", method.Name);

                methodBuilder.Replace("#InParameters#", parametersBuilder.ToString());

                methodBuilder.Replace("#Permission#", permissionsBuilder.ToString());
 
                context.MethodBody.ThrowIfNull("context.MethodBody");
                methodBuilder.Replace("#MethodBody#", context.MethodBody);

                methodBuilder.Replace("#returnNull#", "");
                methodsBuilder.Append(methodBuilder);
            }

            var moduleName = vrDynamicAPIModuleManager.GetVRDynamicAPIModuleName(vrDynamicAPI.ModuleId);
            classControllerBuilder.Replace("#RoutePrefix#",string.Format("api/{0}/", moduleName));
            classControllerBuilder.Replace("#ModuleName#", moduleName);
            classControllerBuilder.Replace("#ControllerName#", vrDynamicAPI.Name);
            classControllerBuilder.Replace("#Methods#", methodsBuilder.ToString());

            string classNamespace = CSharpCompiler.GenerateUniqueNamespace("Vanrise.DynamicAPI");
            var className = string.Format("{0}Controller",vrDynamicAPI.Name);
            classControllerBuilder.Replace("#NAMESPACE#", classNamespace);
            classControllerBuilder.Replace("#CLASSNAME#", className);

            fullTypeName = String.Format("{0}.{1}", classNamespace, className);

            return classControllerBuilder.ToString();
        }

        private class VRDynamicAPILoggableEntity : VRLoggableEntityBase
        {
            public static VRDynamicAPILoggableEntity Instance = new VRDynamicAPILoggableEntity();

            private VRDynamicAPILoggableEntity()
            {

            }

            static VRDynamicAPIManager _vrDynamicAPIManager = new VRDynamicAPIManager();

            public override string EntityUniqueName { get { return "VR_Common_VRDynamicAPI"; } }

            public override string ModuleName { get { return "Common"; } }

            public override string EntityDisplayName { get { return "VRDynamicAPI"; } }

            public override string ViewHistoryItemClientActionName { get { return "VR_Common_VRDynamicAPI_ViewHistoryItem"; } }

            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                VRDynamicAPI vrDynamicAPI = context.Object.CastWithValidate<VRDynamicAPI>("context.Object");
                return vrDynamicAPI.VRDynamicAPIId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                VRDynamicAPI vrDynamicAPI = context.Object.CastWithValidate<VRDynamicAPI>("context.Object");
                return _vrDynamicAPIManager.GetVRDynamicAPIName(vrDynamicAPI);
            }
        }


        #endregion

        #region Mappers
        public VRDynamicAPIDetails VRDynamicAPIDetailMapper(VRDynamicAPI vrDynamicAPI)
        {
            IUserManager userManager = BEManagerFactory.GetManager<IUserManager>();
            string moduleName = vrDynamicAPIModuleManager.GetVRDynamicAPIModuleName(vrDynamicAPI.ModuleId);
            return new VRDynamicAPIDetails
            {
                Name = vrDynamicAPI.Name,
                VRDynamicAPIId = vrDynamicAPI.VRDynamicAPIId,
                ModuleName= moduleName,
                Settings = vrDynamicAPI.Settings,
                CreatedTime = vrDynamicAPI.CreatedTime,
                CreatedByDescription = userManager.GetUserName(vrDynamicAPI.CreatedBy),
                LastModifiedTime = vrDynamicAPI.LastModifiedTime,
                LastModifiedByDescription = userManager.GetUserName(vrDynamicAPI.LastModifiedBy),
                APIDescription = GetAPIDescription(moduleName, vrDynamicAPI.Name)
            };

        }

        private string GetAPIDescription(string moduleName, string contorllerName)
        {
            return string.Format("api/{0}/{1}", moduleName, contorllerName);
        }
        #endregion
    }

  
}

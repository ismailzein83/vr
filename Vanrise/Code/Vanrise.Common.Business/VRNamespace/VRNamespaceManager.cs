﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Vanrise.Common.Data;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class VRNamespaceManager : IVRNamespaceManager
    {
        static AssemblyObject assemblyObject = new AssemblyObject();
        VRDevProjectManager vrDevProjectManager = new VRDevProjectManager();

        public const string USING_STATEMENTS = @"using System;
            using System.Collections.Generic;
            using System.IO;
            using System.Data;
            using System.Linq;
            using Vanrise.Common;
            using Vanrise.GenericData.Business;
            using Vanrise.GenericData.Entities;";

        #region Public Methods

        public IDataRetrievalResult<VRNamespaceDetail> GetFilteredVRNamespaces(DataRetrievalInput<VRNamespaceQuery> input)
        {
            var allVRNamespaces = this.GetCachedVRNamespaces();
            Func<VRNamespace, bool> filterExpression = (nameSpace) =>
            {
                if (Utilities.ShouldHideItemHavingDevProjectId(nameSpace.DevProjectId))
                    return false;
                if (input.Query.Name != null && !nameSpace.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    return false;

                if (input.Query.DevProjectIds != null && (!nameSpace.DevProjectId.HasValue || !input.Query.DevProjectIds.Contains(nameSpace.DevProjectId.Value)))
                    return false;
                return true;
            };
            VRActionLogger.Current.LogGetFilteredAction(VRNamespaceLoggableEntity.Instance, input);
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allVRNamespaces.ToBigResult(input, filterExpression, VRNamespaceDetailMapper));
        }

        public VRNamespace GetVRNamespace(Guid vrNamespaceId)
        {
            Dictionary<Guid, VRNamespace> cachedVRNamespaces = this.GetCachedVRNamespaces();
            return cachedVRNamespaces.GetRecord(vrNamespaceId);
        }

        public VRNamespace GetVRNamespaceByName(string vrNamespaceName)
        {
            Dictionary<string, VRNamespace> cachedVRNamespaces = this.GetCachedVRNamespacesByName();
            return cachedVRNamespaces.GetRecord(vrNamespaceName);
        }

        public string GetVRNamespaceName(VRNamespace vrNamespace)
        {
            return (vrNamespace != null) ? vrNamespace.Name : null;
        }

        public Vanrise.Entities.InsertOperationOutput<VRNamespaceDetail> AddVRNamespace(VRNamespace vrNamespace)
        {
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<VRNamespaceDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            IVRNamespaceDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRNamespaceDataManager>();

            vrNamespace.VRNamespaceId = Guid.NewGuid();

            if (dataManager.Insert(vrNamespace))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                VRActionLogger.Current.TrackAndLogObjectAdded(VRNamespaceLoggableEntity.Instance, vrNamespace);
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = VRNamespaceDetailMapper(vrNamespace);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<VRNamespaceDetail> UpdateVRNamespace(VRNamespace vrNamespace)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<VRNamespaceDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IVRNamespaceDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRNamespaceDataManager>();

            if (dataManager.Update(vrNamespace))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                VRActionLogger.Current.TrackAndLogObjectUpdated(VRNamespaceLoggableEntity.Instance, vrNamespace);
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = VRNamespaceDetailMapper(this.GetVRNamespace(vrNamespace.VRNamespaceId));
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public VRNamespaceCompilationOutput TryCompileNamespace(VRNamespace vrNamespace)
        {
            VRNamespaceCompilationOutput output;
            if (TryCompileNamespace(new List<VRNamespace>() { vrNamespace }, null, out output))
                CSharpCompiler.SetExpiredAssembly(output.OutputAssembly.FullName);

            return output;
        }

        public Type GetNamespaceType(string vrNamespaceName, string className)
        {
            VRNamespace vrNamespace = GetVRNamespaceByName(vrNamespaceName);
            if (vrNamespace == null)
                return null;

            var outputAssembly = GetCachedAssembly();
            return outputAssembly.GetType(string.Format("{0}.{1}", vrNamespaceName, className));
        }

        public void CompileVRNamespacesAssembly()
        {
            GetCachedAssembly();
        }

        public IEnumerable<VRNamespaceInfo> GetVRNamespacesInfo()
        {
            var vrNamespaces = GetCachedVRNamespaces();
            if (vrNamespaces != null && vrNamespaces.Count > 0)
            {
                Func<VRNamespace, bool> filterExpression = (item) =>
                {
                    if (Utilities.ShouldHideItemHavingDevProjectId(item.DevProjectId))
                        return false;
                    return true;
                };
                return vrNamespaces.MapRecords(VRNamespaceInfoMapper, filterExpression).OrderBy(ns => ns.Name);
            }
            return null;
        }
        public IEnumerable<VRNamespaceClassInfo> GetVRNamespaceClassesInfo(Guid vrNamespaceId)
        {
            var assemblyClasses = GetAssemblyClasses(vrNamespaceId);
            if (assemblyClasses != null && assemblyClasses.Count > 0)
            {
                return assemblyClasses.Keys.MapRecords(VRNamespaceClassInfoMapper).OrderBy(x => x.Name);
            }
            return null;
        }

        public IEnumerable<VRNamespaceClassMethodInfo> GetVRNamespaceClassMethodsInfo(Guid vrNamespaceId, string className)
        {
            var assemblyClassMethods = GetAssemblyClassMethods(vrNamespaceId, className);
            if (assemblyClassMethods != null && assemblyClassMethods.Count > 0)
            {
                return assemblyClassMethods.Keys.MapRecords(VRNamespaceClassMethodInfoMapper).OrderBy(x => x.Name);
            }
            return null;
        }
        public Dictionary<string, MethodInfo> GetAssemblyClassMethods(Guid vrNamespaceId, string className)
        {
            var classType = GetAssemblyClassType(vrNamespaceId, className);
            Dictionary<string, MethodInfo> methods = new Dictionary<string, MethodInfo>();
            if (classType != null)
            {
                MethodInfo[] methodInfos = classType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                if (methodInfos != null && methodInfos.Length > 0)
                {
                    return methodInfos.ToDictionary(key => key.Name, value => value);
                }
            }
            return methods;
        }
        #endregion

        #region Private Methods

        public Dictionary<Guid, VRNamespace> GetCachedVRNamespaces()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedVRNamespaces",
               () =>
               {
                   IVRNamespaceDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRNamespaceDataManager>();
                   return dataManager.GetVRNamespaces().ToDictionary(x => x.VRNamespaceId, x => x);
               });
        }

        private Dictionary<string, VRNamespace> GetCachedVRNamespacesByName()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedVRNamespacesByName",
               () =>
               {
                   Dictionary<Guid, VRNamespace> cachedNamespaces = GetCachedVRNamespaces();
                   return cachedNamespaces.ToDictionary(x => x.Value.Name, x => x.Value);
               });
        }

        public IEnumerable<VRDynamicCodeConfig> GetVRDynamicCodeSettingsConfigs()
        {
            var extensionConfigurationManager = new ExtensionConfigurationManager();
            return extensionConfigurationManager.GetExtensionConfigurations<VRDynamicCodeConfig>(VRDynamicCodeConfig.EXTENSION_TYPE);
        }

        public string BuildClassDefinition(VRNamespace vrNamespace)
        {
            StringBuilder classDefinitionBuilder = new StringBuilder(@" 
                namespace #NAMESPACE#
                {
                    #NAMESPACEMEMBERS#

                    #CLASSCODE#
                }");

            StringBuilder classCodes = new StringBuilder();
            StringBuilder namespaceMembers = new StringBuilder();
            VRNamespaceItemManager _vRNamespaceItemManager = new VRNamespaceItemManager();
            IEnumerable<VRNamespaceItem> vrNamespaceItems = _vRNamespaceItemManager.GetVRNamespaceItemsByNamespaceId(vrNamespace.VRNamespaceId);
            foreach (var namespaceItem in vrNamespaceItems)
            {
                if (namespaceItem.Settings != null && namespaceItem.Settings.Code != null)
                {
                    var context = new VRDynamicCodeSettingsContext();
                    classCodes.Append(namespaceItem.Settings.Code.Generate(context));
                    classCodes.AppendLine();
                    if (context.NamespaceMembers != null)
                    {
                        if (namespaceMembers.Length > 0)
                            namespaceMembers.AppendLine();
                        namespaceMembers.Append(context.NamespaceMembers);
                    }
                }
            }


            classDefinitionBuilder.Replace("#NAMESPACE#", vrNamespace.Name);
            classDefinitionBuilder.Replace("#CLASSCODE#", classCodes.ToString());
            classDefinitionBuilder.Replace("#NAMESPACEMEMBERS#", namespaceMembers.ToString());

            return classDefinitionBuilder.ToString();
        }
        private Dictionary<string, Type> GetAssemblyClasses(Guid vrNamespaceId)
        {
            var vrNamespace = GetVRNamespace(vrNamespaceId);
            vrNamespace.ThrowIfNull("vrNamespace", vrNamespaceId);

            Assembly runtimeAssembly = null;

            //if(vrNamespace.DevProjectId.HasValue)
            //{
            //    var devProjectManager = new VRDevProjectManager();
                
            //    if (devProjectManager.TryGetDevProjectAssembly(vrNamespace.DevProjectId.Value, DateTime.Parse("2000-01-01"), out runtimeAssembly))
            //    {
                    
            //    }
            //}

            if(runtimeAssembly == null)
            {
                var compilationOutput = TryCompileNamespace(vrNamespace);
                if(compilationOutput.Result)
                {
                    runtimeAssembly = compilationOutput.OutputAssembly;
                }
            }

            Dictionary<string, Type> assemblyClasses = new Dictionary<string, Type>();
            if (runtimeAssembly != null)
            {
                foreach (Type type in runtimeAssembly.GetTypes())
                {
                    if (type.IsClass && type.IsPublic)// && type.Namespace == vrNamespace.Name)
                    {
                        assemblyClasses.Add(type.Name, type);
                    }
                }
            }
            return assemblyClasses;
        }
        private Type GetAssemblyClassType(Guid vrNamespaceId, string className)
        {
            return GetAssemblyClasses(vrNamespaceId).GetRecord(className);
        }

        private Assembly GetCachedAssembly()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("CachedVRNamespacesAssembly", () =>
              {
                  VRNamespaceCompilationOutput namespaceCompilationOutput;

                  string uniqueNamespaceName = string.Format("ns.Gen_{0}", Guid.NewGuid().ToString("N"));
                  if (TryCompileNamespace(GetCachedVRNamespaces().Values, uniqueNamespaceName, out namespaceCompilationOutput))
                  {
                      if (!string.IsNullOrEmpty(assemblyObject.AssemblyName))
                          CSharpCompiler.SetExpiredAssembly(assemblyObject.AssemblyName);

                      Type.GetType(string.Format("{0}.{1}", uniqueNamespaceName, "DummyClass"));

                      assemblyObject.AssemblyName = namespaceCompilationOutput.OutputAssembly.FullName;
                      return namespaceCompilationOutput.OutputAssembly;
                  }
                  else
                  {
                      StringBuilder errorsBuilder = new StringBuilder();
                      foreach (var errorMessage in namespaceCompilationOutput.ErrorMessages)
                      {
                          errorsBuilder.AppendLine(errorMessage);
                      }

                      throw new Exception(String.Format("Compile Error when building Namespaces. Errors: {0}", errorsBuilder));
                  }
              });
        }

        private bool TryCompileNamespace(IEnumerable<VRNamespace> vrNamespaces, string uniqueNamespaceName, out VRNamespaceCompilationOutput namespaceCompilationOutput)
        {
            StringBuilder classDefinition = new StringBuilder(USING_STATEMENTS);

            foreach (VRNamespace vrNamespace in vrNamespaces)
                classDefinition.AppendLine(BuildClassDefinition(vrNamespace));

            if (!string.IsNullOrEmpty(uniqueNamespaceName))
                classDefinition.AppendLine(GenerateDummyNamespace(uniqueNamespaceName));

            CSharpCompilationOutput compilationOutput;
            if (!CSharpCompiler.TryCompileClass($"CustomNamespace_{Guid.NewGuid()}", classDefinition.ToString(), out compilationOutput, true))
            {
                StringBuilder errorsBuilder = new StringBuilder();
                if (compilationOutput.ErrorMessages != null)
                {
                    foreach (var errorMessage in compilationOutput.ErrorMessages)
                        errorsBuilder.AppendLine(errorMessage);
                }

                namespaceCompilationOutput = new VRNamespaceCompilationOutput
                {
                    OutputAssembly = compilationOutput.OutputAssembly,
                    Result = false,
                    ErrorMessages = compilationOutput.ErrorMessages
                };
            }
            else
            {
                namespaceCompilationOutput = new VRNamespaceCompilationOutput
                {
                    OutputAssembly = compilationOutput.OutputAssembly,
                    Result = true,
                    ErrorMessages = null
                };
            }
            return namespaceCompilationOutput.Result;
        }

        private string GenerateDummyNamespace(string uniqueNamespaceName)
        {
            return string.Concat("namespace ", uniqueNamespaceName, "{public class DummyClass{", "}}");
        }
        #endregion

        #region Classes

        public class VRNamespaceCompilationOutput
        {
            public Assembly OutputAssembly { get; set; }
            public bool Result { get; set; }
            public List<string> ErrorMessages { get; set; }
        }

        private class AssemblyObject
        {
            public string AssemblyName { get; set; }
        }

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IVRNamespaceDataManager _dataManager = CommonDataManagerFactory.GetDataManager<IVRNamespaceDataManager>();
            object _updateHandle;
            DateTime? _namespaceItemCacheLastCheck;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreVRNamespaceUpdated(ref _updateHandle)
                    |
                    Vanrise.Caching.CacheManagerFactory.GetCacheManager<VRNamespaceItemManager.CacheManager>().IsCacheExpired(ref _namespaceItemCacheLastCheck);
            }
        }

        private class VRNamespaceLoggableEntity : VRLoggableEntityBase
        {
            public static VRNamespaceLoggableEntity Instance = new VRNamespaceLoggableEntity();

            private VRNamespaceLoggableEntity()
            {

            }

            static VRNamespaceManager _vrNamespaceManager = new VRNamespaceManager();

            public override string EntityUniqueName { get { return "VR_Common_VRNamespace"; } }

            public override string ModuleName { get { return "Common"; } }

            public override string EntityDisplayName { get { return "VRNamespace"; } }

            public override string ViewHistoryItemClientActionName { get { return "VR_Common_VRNamespace_ViewHistoryItem"; } }

            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                VRNamespace vrNamespace = context.Object.CastWithValidate<VRNamespace>("context.Object");
                return vrNamespace.VRNamespaceId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                VRNamespace vrNamespace = context.Object.CastWithValidate<VRNamespace>("context.Object");
                return _vrNamespaceManager.GetVRNamespaceName(vrNamespace);
            }
        }

        #endregion

        #region Mappers

        public VRNamespaceDetail VRNamespaceDetailMapper(VRNamespace vrNamespace)
        {
            string devProjectName = null;
            if (vrNamespace.DevProjectId.HasValue)
            {
                devProjectName = vrDevProjectManager.GetVRDevProjectName(vrNamespace.DevProjectId.Value);
            }
            VRNamespaceDetail vrNamespaceDetail = new VRNamespaceDetail()
            {
                VRNamespaceId = vrNamespace.VRNamespaceId,
                Name = vrNamespace.Name,
                DevProjectName = devProjectName
            };
            return vrNamespaceDetail;
        }

        public VRNamespaceInfo VRNamespaceInfoMapper(VRNamespace vrNamespace)
        {
            return new VRNamespaceInfo()
            {
                Name = vrNamespace.Name,
                VRNamespaceId = vrNamespace.VRNamespaceId
            };
        }

        public VRNamespaceClassInfo VRNamespaceClassInfoMapper(string className)
        {
            return new VRNamespaceClassInfo()
            {
                Name = className
            };
        }
        public VRNamespaceClassMethodInfo VRNamespaceClassMethodInfoMapper(string methodName)
        {
            return new VRNamespaceClassMethodInfo()
            {
                Name = methodName
            };
        }

        #endregion
    }

    public class VRNamespaceVRDevProjectCodeGenerator : VRDevProjectCodeGenerator
    {
        public override void GenerateCode(IVRDevProjectCodeGeneratorGenerateCodeContext context)
        {
            var namespaceManager = new VRNamespaceManager();
            foreach (var vrNamespace in namespaceManager.GetCachedVRNamespaces().Values)
            {
                if (vrNamespace.DevProjectId == context.DevProjectId)
                {
                    string classDefinition = namespaceManager.BuildClassDefinition(vrNamespace);
                    string classDefinitionWithUsingStatements = string.Concat(VRNamespaceManager.USING_STATEMENTS, "\n\n\n", classDefinition);
                    context.AddCodeFile(vrNamespace.Name.Replace(".", "_"), classDefinitionWithUsingStatements);
                }
            }
        }
    }
}

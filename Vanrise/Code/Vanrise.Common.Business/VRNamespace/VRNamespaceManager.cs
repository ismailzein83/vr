using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Vanrise.Common.Data;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class VRNamespaceManager
    {

        static Dictionary<string, AssemblyObject> vrNamespaceAssemblies = new Dictionary<string, AssemblyObject>();

        #region Public Methods

        public IDataRetrievalResult<VRNamespaceDetail> GetFilteredVRNamespaces(DataRetrievalInput<VRNamespaceQuery> input)
        {
            var allVRNamespaces = this.GetCachedVRNamespaces();
            Func<VRNamespace, bool> filterExpression = (x) => (input.Query.Name == null || x.Name.ToLower().Contains(input.Query.Name.ToLower()));
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
            Type outputType;
            if (TryCompileNamespace(vrNamespace, null, out outputType, out output))
                CSharpCompiler.SetExpiredAssembly(output.OutputAssembly.FullName);

            return output;
        }

        public Type GetNamespaceType(string vrNamespaceName, string className)
        {
            VRNamespace vrNamespace = GetVRNamespaceByName(vrNamespaceName);
            if (vrNamespace == null)
                return null;

            var cacheName = new GetVRNamespaceTypeCacheName { Namespace = vrNamespaceName, ClassName = className };
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(cacheName,
              () =>
              {
                  AssemblyObject existingAssembly = vrNamespaceAssemblies.GetOrCreateItem(vrNamespaceName);
                  if (!string.IsNullOrEmpty(existingAssembly.AssemblyName))
                      CSharpCompiler.SetExpiredAssembly(existingAssembly.AssemblyName);

                  Type outputType;
                  VRNamespaceCompilationOutput namespaceCompilationOutput;

                  if (TryCompileNamespace(vrNamespace, className, out outputType, out namespaceCompilationOutput))
                  {
                      existingAssembly.AssemblyName = namespaceCompilationOutput.OutputAssembly.FullName;
                      return outputType;
                  }
                  else
                  {
                      StringBuilder errorsBuilder = new StringBuilder();
                      foreach (var errorMessage in namespaceCompilationOutput.ErrorMessages)
                      {
                          errorsBuilder.AppendLine(errorMessage);
                      }

                      throw new Exception(String.Format("Compile Error when building for Namespace '{0}'. Errors: {1}",
                          vrNamespaceName, errorsBuilder));
                  }
              });
        }

        private bool TryCompileNamespace(VRNamespace vrNamespace, string className, out Type outputType, out VRNamespaceCompilationOutput namespaceCompilationOutput)
        {
            outputType = null;

            var classDefinition = BuildClassDefinition(vrNamespace);

            CSharpCompilationOutput compilationOutput;
            if (!CSharpCompiler.TryCompileClass(classDefinition, out compilationOutput))
            {
                StringBuilder errorsBuilder = new StringBuilder();
                if (compilationOutput.ErrorMessages != null)
                {
                    foreach (var errorMessage in compilationOutput.ErrorMessages)
                    {
                        errorsBuilder.AppendLine(errorMessage);
                    }
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

                if (!string.IsNullOrEmpty(className))
                    outputType = compilationOutput.OutputAssembly.GetType(string.Format("{0}.{1}", vrNamespace.Name, className));
            }
            return namespaceCompilationOutput.Result;
        }

        private struct GetVRNamespaceTypeCacheName
        {
            public string Namespace { get; set; }

            public string ClassName { get; set; }
        }
        #endregion

        #region Private Classes
        private class AssemblyObject
        {
            public string AssemblyName { get; set; }
        }

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IVRNamespaceDataManager _dataManager = CommonDataManagerFactory.GetDataManager<IVRNamespaceDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreVRNamespaceUpdated(ref _updateHandle);
            }
        }

        private class VRNamespaceLoggableEntity : VRLoggableEntityBase
        {
            public static VRNamespaceLoggableEntity Instance = new VRNamespaceLoggableEntity();

            private VRNamespaceLoggableEntity()
            {

            }

            static VRNamespaceManager _vrNamespaceManager = new VRNamespaceManager();

            public override string EntityUniqueName
            {
                get { return "VR_Common_VRNamespace"; }
            }

            public override string ModuleName
            {
                get { return "Common"; }
            }

            public override string EntityDisplayName
            {
                get { return "VRNamespace"; }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "VR_Common_VRNamespace_ViewHistoryItem"; }
            }

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

        #region Private Methods

        Dictionary<Guid, VRNamespace> GetCachedVRNamespaces()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedVRNamespaces",
               () =>
               {
                   IVRNamespaceDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRNamespaceDataManager>();
                   return dataManager.GetVRNamespaces().ToDictionary(x => x.VRNamespaceId, x => x);
               });
        }

        Dictionary<string, VRNamespace> GetCachedVRNamespacesByName()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedVRNamespacesByName",
               () =>
               {
                   Dictionary<Guid, VRNamespace> cachedNamespaces = GetCachedVRNamespaces();
                   return cachedNamespaces.ToDictionary(x => x.Value.Name, x => x.Value);
               });
        }

        private string BuildClassDefinition(VRNamespace vrNamespace)
        {
            StringBuilder classDefinitionBuilder = new StringBuilder(@" 
                using System;
                using System.Collections.Generic;
                using System.IO;
                using System.Data;
                using System.Linq;
                using Vanrise.Common;
                using Vanrise.GenericData.Business;
                using Vanrise.GenericData.Entities;

                namespace #NAMESPACE#
                {
                    #CLASSCODE#
                }");

            classDefinitionBuilder.Replace("#NAMESPACE#", vrNamespace.Name);
            classDefinitionBuilder.Replace("#CLASSCODE#", vrNamespace.Settings.Code);

            return classDefinitionBuilder.ToString();
        }
        #endregion

        #region Mappers

        public VRNamespaceDetail VRNamespaceDetailMapper(VRNamespace vrNamespace)
        {
            VRNamespaceDetail vrNamespaceDetail = new VRNamespaceDetail()
            {
                VRNamespaceId = vrNamespace.VRNamespaceId,
                Name = vrNamespace.Name
            };
            return vrNamespaceDetail;
        }

        #endregion

        public class VRNamespaceCompilationOutput
        {
            public Assembly OutputAssembly { get; set; }
            public bool Result { get; set; }
            public List<string> ErrorMessages { get; set; }
        }
    }
}

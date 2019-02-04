using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;
using Vanrise.Entities;
using Vanrise.Security.Entities;

namespace Vanrise.Common.Business
{
    public class VRNamespaceItemManager
    {
        VRNamespaceManager _vrNamespaceManager = new VRNamespaceManager();

        #region Public Methods

        public IDataRetrievalResult<VRNamespaceItemDetail> GetFilteredVRNamespaceItems(DataRetrievalInput<VRNamespaceItemQuery> input)
        {
            var allVRNamespaceItems = this.GetCachedVRNamespaceItems();
            Func<VRNamespaceItem, bool> filterExpression = (namespaceItem) =>
            {
                if (input.Query.Name != null && !namespaceItem.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    return false;

                if (input.Query.NameSpaceId.HasValue && input.Query.NameSpaceId.Value != namespaceItem.VRNamespaceId)
                    return false;

                return true;
            };
            VRActionLogger.Current.LogGetFilteredAction(VRNamespaceItemLoggableEntity.Instance, input);
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allVRNamespaceItems.ToBigResult(input, filterExpression, VRNamespaceItemDetailMapper));
        }

        public VRNamespaceItem GetVRNamespaceItem(Guid vrNamespaceItemId)
        {
            Dictionary<Guid, VRNamespaceItem> cachedVRNamespaceItems = this.GetCachedVRNamespaceItems();
            return cachedVRNamespaceItems.GetRecord(vrNamespaceItemId);
        }

        public string GetVRNamespaceItemName(VRNamespaceItem vrNamespaceItem)
        {
            return (vrNamespaceItem != null) ? vrNamespaceItem.Name : null;
        }

        public Vanrise.Entities.InsertOperationOutput<VRNamespaceItemDetail> AddVRNamespaceItem(VRNamespaceItem vrNamespaceItem)
        {
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<VRNamespaceItemDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            IVRNamespaceItemDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRNamespaceItemDataManager>();

            vrNamespaceItem.VRNamespaceItemId = Guid.NewGuid();

            int loggedInUserId = ContextFactory.GetContext().GetLoggedInUserId();
            vrNamespaceItem.CreatedBy = loggedInUserId;
            vrNamespaceItem.LastModifiedBy = loggedInUserId;

            VRNamespaceItemCompilationOutput output;
            if (TryCompileNamespaceItem(vrNamespaceItem, null, out output))
            {
                CSharpCompiler.SetExpiredAssembly(output.OutputAssembly.FullName);
                if (dataManager.Insert(vrNamespaceItem))
                {
                    Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                    VRActionLogger.Current.TrackAndLogObjectAdded(VRNamespaceItemLoggableEntity.Instance, vrNamespaceItem);
                    insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                    insertOperationOutput.InsertedObject = VRNamespaceItemDetailMapper(this.GetVRNamespaceItem(vrNamespaceItem.VRNamespaceItemId));
                }
                else
                {
                    insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
                }
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<VRNamespaceItemDetail> UpdateVRNamespaceItem(VRNamespaceItem vrNamespaceItem)
        {

            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<VRNamespaceItemDetail>();
            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            vrNamespaceItem.LastModifiedBy = ContextFactory.GetContext().GetLoggedInUserId();

            IVRNamespaceItemDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRNamespaceItemDataManager>();


            VRNamespaceItemCompilationOutput output;
            if (TryCompileNamespaceItem(vrNamespaceItem, null, out output))
            {
                CSharpCompiler.SetExpiredAssembly(output.OutputAssembly.FullName);
                if (dataManager.Update(vrNamespaceItem))
                {
                    Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                    VRActionLogger.Current.TrackAndLogObjectUpdated(VRNamespaceItemLoggableEntity.Instance, vrNamespaceItem);
                    updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                    updateOperationOutput.UpdatedObject = VRNamespaceItemDetailMapper(this.GetVRNamespaceItem(vrNamespaceItem.VRNamespaceItemId));
                }
                else
                {
                    updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
                }
            }

            return updateOperationOutput;
        }
        public IEnumerable<VRDynamicCodeConfig> GetVRDynamicCodeSettingsConfigs()
        {
            var extensionConfigurationManager = new ExtensionConfigurationManager();
            return extensionConfigurationManager.GetExtensionConfigurations<VRDynamicCodeConfig>(VRDynamicCodeConfig.EXTENSION_TYPE);
        }

        public VRNamespaceItemCompilationOutput TryCompileNamespaceItem(VRNamespaceItem vrNamespaceItem)
        {
            VRNamespaceItemCompilationOutput output;
            if (TryCompileNamespaceItem(vrNamespaceItem, null, out output))
                CSharpCompiler.SetExpiredAssembly(output.OutputAssembly.FullName);

            return output;
        }

        public IEnumerable<VRNamespaceItem> GetVRNamespaceItemsByNamespaceId(Guid vrNamespaceId)
        {
            var allVRNamespaceItems = this.GetCachedVRNamespaceItems();
            return allVRNamespaceItems.FindAllRecords(x => x.VRNamespaceId == vrNamespaceId);
        }

        #endregion

        #region Private Methods

        private Dictionary<Guid, VRNamespaceItem> GetCachedVRNamespaceItems()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedVRNamespaceItems",
               () =>
               {
                   IVRNamespaceItemDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRNamespaceItemDataManager>();
                   return dataManager.GetVRNamespaceItems().ToDictionary(x => x.VRNamespaceItemId, x => x);
               });
        }

        private bool TryCompileNamespaceItem(VRNamespaceItem vrNamespaceItem, string uniqueNamespaceItemName, out VRNamespaceItemCompilationOutput namespaceItemCompilationOutput)
        {
            StringBuilder classDefinition = new StringBuilder(@"using System;
            using System.Collections.Generic;
            using System.IO;
            using System.Data;
            using System.Linq;
            using Vanrise.Common;
            using Vanrise.GenericData.Business;
            using Vanrise.GenericData.Entities;");


            classDefinition.AppendLine(BuildClassDefinition(vrNamespaceItem));

            if (!string.IsNullOrEmpty(uniqueNamespaceItemName))
                classDefinition.AppendLine(GenerateDummyNamespaceItem(uniqueNamespaceItemName));

            CSharpCompilationOutput compilationOutput;
            if (!CSharpCompiler.TryCompileClass(classDefinition.ToString(), out compilationOutput, true))
            {
                StringBuilder errorsBuilder = new StringBuilder();
                if (compilationOutput.ErrorMessages != null)
                {
                    foreach (var errorMessage in compilationOutput.ErrorMessages)
                        errorsBuilder.AppendLine(errorMessage);
                }

                namespaceItemCompilationOutput = new VRNamespaceItemCompilationOutput
                {
                    OutputAssembly = compilationOutput.OutputAssembly,
                    Result = false,
                    ErrorMessages = compilationOutput.ErrorMessages
                };
            }
            else
            {
                namespaceItemCompilationOutput = new VRNamespaceItemCompilationOutput
                {
                    OutputAssembly = compilationOutput.OutputAssembly,
                    Result = true,
                    ErrorMessages = null
                };
            }
            return namespaceItemCompilationOutput.Result;
        }

        private string GenerateDummyNamespaceItem(string uniqueNamespaceItemName)
        {
            return string.Concat("namespace ", uniqueNamespaceItemName, "{public class DummyClass{", "}}");
        }

        private string BuildClassDefinition(VRNamespaceItem vrNamespaceItem)
        {
            VRNamespace vrNamespace = _vrNamespaceManager.GetVRNamespace(vrNamespaceItem.VRNamespaceId);
            StringBuilder classDefinitionBuilder = new StringBuilder(@" 
                namespace #NAMESPACE#
                {
                    #NAMESPACEMEMBERS#

                    #CLASSCODE#
                }");

            StringBuilder classCode = new StringBuilder();
            StringBuilder namespaceMembers = new StringBuilder();
            if (vrNamespaceItem.Settings != null && vrNamespaceItem.Settings.Code != null)
            {
                var context = new VRDynamicCodeSettingsContext();
                classCode.Append(vrNamespaceItem.Settings.Code.Generate(context));
                if (context.NamespaceMembers != null)
                {
                    if (namespaceMembers.Length > 0)
                        namespaceMembers.AppendLine();
                    namespaceMembers.Append(context.NamespaceMembers);
                }
            }

            classDefinitionBuilder.Replace("#NAMESPACE#", vrNamespace.Name);
            classDefinitionBuilder.Replace("#CLASSCODE#", classCode.ToString());
            classDefinitionBuilder.Replace("#NAMESPACEMEMBERS#", namespaceMembers.ToString());

            return classDefinitionBuilder.ToString();
        }

        #endregion

        #region Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IVRNamespaceItemDataManager _dataManager = CommonDataManagerFactory.GetDataManager<IVRNamespaceItemDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreVRNamespaceItemUpdated(ref _updateHandle);
            }
        }

        public class VRNamespaceItemCompilationOutput
        {
            public Assembly OutputAssembly { get; set; }
            public bool Result { get; set; }
            public List<string> ErrorMessages { get; set; }
        }

        private class VRNamespaceItemLoggableEntity : VRLoggableEntityBase
        {
            public static VRNamespaceItemLoggableEntity Instance = new VRNamespaceItemLoggableEntity();

            private VRNamespaceItemLoggableEntity()
            {

            }

            static VRNamespaceItemManager _vrNamespaceItemManager = new VRNamespaceItemManager();

            public override string EntityUniqueName { get { return "VR_Common_VRNamespaceItem"; } }

            public override string ModuleName { get { return "Common"; } }

            public override string EntityDisplayName { get { return "VRNamespaceItem"; } }

            public override string ViewHistoryItemClientActionName { get { return "VR_Common_VRNamespaceItem_ViewHistoryItem"; } }

            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                VRNamespaceItem vrNamespaceItem = context.Object.CastWithValidate<VRNamespaceItem>("context.Object");
                return vrNamespaceItem.VRNamespaceItemId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                VRNamespaceItem vrNamespaceItem = context.Object.CastWithValidate<VRNamespaceItem>("context.Object");
                return _vrNamespaceItemManager.GetVRNamespaceItemName(vrNamespaceItem);
            }
        }

        #endregion

        #region Mappers

        public VRNamespaceItemDetail VRNamespaceItemDetailMapper(VRNamespaceItem vrNamespaceItem)
        {
            VRNamespaceItemDetail vrNamespaceItemDetail = new VRNamespaceItemDetail()
            {
                VRNamespaceItemId = vrNamespaceItem.VRNamespaceItemId,
                Name = vrNamespaceItem.Name,
                CreatedByDescription = BEManagerFactory.GetManager<IUserManager>().GetUserName(vrNamespaceItem.CreatedBy)
            };
            return vrNamespaceItemDetail;
        }

        #endregion
    }
}

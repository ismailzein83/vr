using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Security.Data;
using Vanrise.Security.Entities;
using Vanrise.Common;
using Vanrise.Caching;

namespace Vanrise.Security.Business
{
    public class PermissionManager
    {
        #region Fields / Constructors

        GroupManager _groupManager;
        UserManager _userManager;
        BusinessEntityNodeManager _beNodeManager;
        BusinessEntityManager _beManager;

        public PermissionManager()
        {
            _groupManager = new GroupManager();
            _userManager = new UserManager();
            _beNodeManager = new BusinessEntityNodeManager();
            _beManager = new BusinessEntityManager();
        }

        #endregion

        #region Public Methods

        public Vanrise.Entities.IDataRetrievalResult<PermissionDetail> GetFilteredEntityPermissions(Vanrise.Entities.DataRetrievalInput<PermissionQuery> input)
        {
            IEnumerable<Permission> filteredPermissions = GetEntityPermissions(input.Query.EntityType, input.Query.EntityId);
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult<PermissionDetail>(input, filteredPermissions.ToBigResult(input, null, PermissionDetailMapper));
        }
               
        public IEnumerable<Permission> GetEntityPermissions(EntityType entityType, string entityId)
        {
            List<Permission> entityPermissions = new List<Permission>();
            
            IEnumerable<Permission> cachedPermissions = GetCachedPermissions();
            BusinessEntityNode targetNode = _beNodeManager.GetBusinessEntityNode(entityType, entityId);

            do
            {
                IEnumerable<Permission> nodePermissions = cachedPermissions.FindAllRecords(x => x.EntityType == targetNode.EntType && x.ComparePermissionEntityId(targetNode.EntityId.ToString()));
                entityPermissions.AddRange(nodePermissions);

                if (targetNode.BreakInheritance) break;
                targetNode = targetNode.Parent;

            } while (targetNode != null);

            foreach (Permission entityPermission in entityPermissions)
                entityPermission.PermissionPath = _beNodeManager.GetBusinessEntityNodePath(_beManager.GetBusinessEntityName(entityPermission.EntityType, entityPermission.EntityId));

            return entityPermissions;
        }

        public IEnumerable<PermissionDetail> GetHolderPermissions(HolderType holderType, string holderId)
        {
            IEnumerable<Permission> cachedPermissions = GetCachedPermissions();
            return cachedPermissions.MapRecords(PermissionDetailMapper, permission => permission.HolderType == holderType && permission.HolderId == holderId);
        }

        public EffectivePermissionsWrapper GetEffectivePermissions(int userId)
        {
            List<int> groups = _groupManager.GetUserGroups(userId);
            IEnumerable<Permission> cachedPermissions = GetCachedPermissions();
            List<BusinessEntityNode> businessEntityHierarchy = _beNodeManager.GetEntityNodes();

            List<Dictionary<string, Dictionary<string, Flag>>> listOfAllPermissions = new List<Dictionary<string, Dictionary<string, Flag>>>();

            IEnumerable<Permission> userPermissions = cachedPermissions.FindAllRecords(x => x.HolderType == HolderType.USER && x.HolderId == userId.ToString());
            listOfAllPermissions.Add(this.ConvertPermissionsToPathDictionary(userPermissions, businessEntityHierarchy));


            foreach (int groupId in groups)
            {
                IEnumerable<Permission> groupPermissions = cachedPermissions.FindAllRecords(x => x.HolderType == HolderType.GROUP && x.HolderId == groupId.ToString());
                if (groupPermissions.Count() > 0)
                    listOfAllPermissions.Add(this.ConvertPermissionsToPathDictionary(groupPermissions, businessEntityHierarchy));
            }

            Dictionary<string, List<string>> allowPermissions = new Dictionary<string, List<string>>();
            Dictionary<string, List<string>> denyPermissions = new Dictionary<string, List<string>>();

            foreach (Dictionary<string, Dictionary<string, Flag>> singlePermission in listOfAllPermissions)
            {
                foreach (KeyValuePair<string, Dictionary<string, Flag>> permkvp in singlePermission)
                {
                    List<string> allowValues = new List<string>();
                    List<string> denyValues = new List<string>();

                    foreach (KeyValuePair<string, Flag> flagKVP in permkvp.Value)
                    {
                        if (flagKVP.Value == Flag.ALLOW)
                        {
                            allowValues.Add(flagKVP.Key);
                        }
                        else
                        {
                            denyValues.Add(flagKVP.Key);
                        }
                    }

                    //Add distinct allow and deny permissions from the list of all permissions
                    if (allowPermissions.ContainsKey(permkvp.Key))
                    {
                        foreach (string item in allowValues)
                        {
                            List<string> allowPermissionsValues = allowPermissions[permkvp.Key];
                            if (!allowPermissionsValues.Contains(item))
                                allowPermissionsValues.Add(item);
                        }
                    }
                    else if (allowValues.Count > 0)
                    {
                        allowPermissions.Add(permkvp.Key, allowValues);
                    }

                    if (denyPermissions.ContainsKey(permkvp.Key))
                    {
                        foreach (string item in denyValues)
                        {
                            List<string> denyPermissionsValues = denyPermissions[permkvp.Key];
                            if (!denyPermissionsValues.Contains(item))
                                denyPermissionsValues.Add(item);
                        }
                    }
                    else if (denyValues.Count > 0)
                    {
                        denyPermissions.Add(permkvp.Key, denyValues);
                    }
                }
            }


            //Filter the allowPermissions based on the denyPermissions list
            foreach (KeyValuePair<string, List<string>> allowPerm in allowPermissions)
            {
                if (denyPermissions.ContainsKey(allowPerm.Key))
                {
                    List<string> filteredValues = new List<string>();
                    foreach (string allowFlag in allowPerm.Value)
                    {
                        if (!denyPermissions[allowPerm.Key].Contains(allowFlag))
                        {
                            filteredValues.Add(allowFlag);
                        }
                    }
                    allowPerm.Value.Clear();
                    allowPerm.Value.AddRange(filteredValues);
                }
            }

            Dictionary<string, Dictionary<string, Flag>> processedListofPermissions = new Dictionary<string, Dictionary<string, Flag>>();

            //Loop on all allowPermissions and build the result value from it, make sure to add the flags of denied as well
            //that are respective to allowpermission
            foreach (KeyValuePair<string, List<string>> item in allowPermissions)
            {
                Dictionary<string, Flag> allflags = new Dictionary<string, Flag>();
                foreach (string allowStr in item.Value)
                {
                    allflags.Add(allowStr, Flag.ALLOW);
                }

                if (denyPermissions.ContainsKey(item.Key))
                {
                    foreach (string denyStr in denyPermissions[item.Key])
                    {
                        allflags.Add(denyStr, Flag.DENY);
                    }
                }

                processedListofPermissions.Add(item.Key, allflags);
            }

            //Now loop and add all missing denypermissions that we could not add in the previous loop
            foreach (KeyValuePair<string, List<string>> missingFlag in denyPermissions)
            {
                if (!processedListofPermissions.ContainsKey(missingFlag.Key))
                {
                    Dictionary<string, Flag> missingDenyflags = new Dictionary<string, Flag>();
                    foreach (string denyStr in missingFlag.Value)
                    {
                        missingDenyflags.Add(denyStr, Flag.DENY);
                    }

                    processedListofPermissions.Add(missingFlag.Key, missingDenyflags);
                }
            }

            EffectivePermissionsWrapper resultWrapper = new EffectivePermissionsWrapper();
            resultWrapper.EffectivePermissions = processedListofPermissions;

            foreach (BusinessEntityNode node in businessEntityHierarchy)
            {
                if (node.BreakInheritance)
                    resultWrapper.BreakInheritanceEntities.Add(node.GetRelativePath());

                IEnumerable<BusinessEntityNode> results = node.Descendants().Where(x => x.BreakInheritance == true);
                if (results != null)
                {
                    foreach (BusinessEntityNode subNode in results)
                    {
                        if (subNode.BreakInheritance)
                            resultWrapper.BreakInheritanceEntities.Add(subNode.GetRelativePath());
                    }
                }
            }

            return resultWrapper;
        }
        public bool HasUpdatePermissions(IEnumerable<Permission> permissions)
        {
           
            StringBuilder builder = new StringBuilder();
            bool isFirstPermission = true;
            foreach (var p in permissions.OrderBy(itm => itm.EntityId))
            {
                if (!isFirstPermission)
                    builder.Append("&");
                isFirstPermission = false;
                Guid entityId = new Guid(p.EntityId);
                string beName = null;
                if (p.EntityType == EntityType.ENTITY)
                {
                    var be  = new BusinessEntityManager().GetBusinessEntityById(entityId);
                    be.ThrowIfNull("be", p.EntityId);
                    beName = be.Name.Trim();
                }
                else
                {
                    var bem = new BusinessEntityModuleManager().GetBusinessEntityModuleById(entityId);
                    bem.ThrowIfNull("be", p.EntityId);
                    beName = bem.Name.Trim();
                }                
                builder.Append(beName);
                builder.Append(":");

                string permission = GetDifferenceWithCurrentPermissionFlags(p.PermissionFlags, p.HolderType, p.HolderId, p.EntityType, p.EntityId);
                builder.Append(permission);
            }
            return ContextFactory.GetContext().IsAllowed(builder.ToString());
        }

        public bool HasDeletePermissions(HolderType holderType, string holderId, EntityType entityType, string entityId)
        {

            StringBuilder builder = new StringBuilder();           
            Guid beentityId = new Guid(entityId);
            string beName = null;
            if (entityType == EntityType.ENTITY)
            {
                var be = new BusinessEntityManager().GetBusinessEntityById(beentityId);
                be.ThrowIfNull("be", beentityId);
                beName = be.Name.Trim();
            }
            else
            {
                var bem = new BusinessEntityModuleManager().GetBusinessEntityModuleById(beentityId);
                bem.ThrowIfNull("be", beentityId);
                beName = bem.Name.Trim();
            }
            builder.Append(beName);
            builder.Append(":");

            string permission = GetDifferenceWithCurrentPermissionFlags(null,holderType,holderId, entityType,entityId);
            builder.Append(permission);
            return ContextFactory.GetContext().IsAllowed(builder.ToString());
        }

        public Vanrise.Entities.UpdateOperationOutput<object> UpdatePermissions(IEnumerable<Permission> permissions)
        {
            UpdateOperationOutput<object> updateOperationOutput = new UpdateOperationOutput<object>();

            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (HasUpdatePermissions(permissions))
            {
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = null;
                IPermissionDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IPermissionDataManager>();

                foreach (Permission item in permissions)
                {
                    if (item.PermissionFlags.Count == 0)
                    {
                        if (!dataManager.DeletePermission(item))
                        {
                            updateOperationOutput.Result = UpdateOperationResult.Failed;
                        }
                    }
                    else if (!dataManager.UpdatePermission(item))
                    {
                        updateOperationOutput.Result = UpdateOperationResult.Failed;
                    }
                }
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
            }
            else
            {
                updateOperationOutput.Message = "you don't have permission to perform this action";
                updateOperationOutput.ShowExactMessage = true;
            }
           

            return updateOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<object> DeletePermissions(HolderType holderType, string holderId, EntityType entityType, string entityId)
        {
            UpdateOperationOutput<object> updateOperationOutput = new UpdateOperationOutput<object>();

            IPermissionDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IPermissionDataManager>();
            bool deleted = dataManager.DeletePermission(holderType, holderId, entityType, entityId);

            if (deleted)
            {
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.Failed;
            }

            updateOperationOutput.UpdatedObject = null;
            return updateOperationOutput;
        }
        
        #endregion

        #region Private Methods

        IEnumerable<Permission> GetCachedPermissions()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetPermissions",
            () =>
            {
                IPermissionDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IPermissionDataManager>();
                return dataManager.GetPermissions();
            });
        }

        Dictionary<string, Dictionary<string, Flag>> ConvertPermissionsToPathDictionary(IEnumerable<Permission> permissions, IEnumerable<BusinessEntityNode> businessEntityHierarchy)
        {
            Dictionary<string, Dictionary<string, Flag>> effectivePermissions = new Dictionary<string, Dictionary<string, Flag>>();

            foreach (Permission item in permissions)
            {
                string relativePath = _beNodeManager.GetBusinessEntityNodePath(item.EntityType, item.EntityId);

                if (relativePath == null)
                    throw new NullReferenceException(String.Format("{0} {1} has permissions on {2} {3} that does not exist", item.HolderType, item.HolderId, item.EntityType, item.EntityId));

                //Map the Permission Flags into a dictionary of strings
                Dictionary<string, Flag> effectiveFlags = new Dictionary<string, Flag>();

                foreach (PermissionFlag flag in item.PermissionFlags)
                    effectiveFlags.Add(flag.Name, flag.Value);

                //Add a new record to the dictionary of effective permissions that is about the path as a key and the list of allowed permissions as a value
                //TODO: this is to be cached later and moved to the base API Controller and done once in a user session
                effectivePermissions.Add(relativePath, effectiveFlags);
            }

            return effectivePermissions;
        }
        private string GetDifferenceWithCurrentPermissionFlags(List<PermissionFlag> checkflags, HolderType holderType, string holderId, EntityType entityType, string entityId)
        {
            StringBuilder builder = new StringBuilder();
            HashSet<string> diffrencecheckflags = new HashSet<string>();

            var currentuserpermission = GetCachedPermissions().FindRecord(x => x.HolderType == holderType && x.HolderId == holderId && x.EntityType == entityType && x.EntityId == entityId);
            if (currentuserpermission != null)
            {
                foreach (var itm in currentuserpermission.PermissionFlags)
                {
                    if (checkflags == null || !checkflags.Any(x => x.Name == itm.Name && x.Value == itm.Value))
                        diffrencecheckflags.Add(itm.Name);
                }
            }
            if (checkflags != null)
            {
                foreach (var itm in checkflags)
                {
                    if (currentuserpermission == null || !currentuserpermission.PermissionFlags.Any(x => x.Name == itm.Name && x.Value == itm.Value))
                        diffrencecheckflags.Add(itm.Name);
                }
            }
            bool isFirstOption = true;
            foreach (string s in diffrencecheckflags.Select(s => s.Trim()))
            {
                if (!isFirstOption)
                    builder.Append(",");
                isFirstOption = false;
                builder.Append(s);
            }

            return builder.ToString();
        }
        #endregion

        #region Private Classes

        public class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IPermissionDataManager _dataManager = SecurityDataManagerFactory.GetDataManager<IPermissionDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired()
            {
                return _dataManager.ArePermissionsUpdated(ref _updateHandle);
            }
        }

        #endregion

        #region Mappers

        PermissionDetail PermissionDetailMapper(Permission permission)
        {
            int holderId = Convert.ToInt32(permission.HolderId);

            return new PermissionDetail()
            {
                Entity = permission,
                HolderName = permission.HolderType == HolderType.GROUP ? _groupManager.GetGroupName(holderId) : _userManager.GetUserName(holderId),
                EntityName = _beManager.GetBusinessEntityName(permission.EntityType, permission.EntityId)
            };
        }
        
        #endregion
    }

    public class EffectivePermissionsWrapper
    {
        public EffectivePermissionsWrapper()
        {
            this.EffectivePermissions = new Dictionary<string, Dictionary<string, Flag>>();
            this.BreakInheritanceEntities = new HashSet<string>();
        }

        public Dictionary<string, Dictionary<string, Flag>> EffectivePermissions { get; set; }

        public HashSet<string> BreakInheritanceEntities { get; set; }
    }
}

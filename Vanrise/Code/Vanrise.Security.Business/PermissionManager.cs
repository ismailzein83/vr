using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Security.Data;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Business
{
    public class PermissionManager
    {
        public List<Permission> GetPermissions()
        {
            IPermissionDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IPermissionDataManager>();
            return dataManager.GetPermissions();
        }

        public List<Permission> GetPermissionsByHolder(HolderType holderType, string holderId)
        {
            IPermissionDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IPermissionDataManager>();
            return dataManager.GetPermissionsByHolder(holderType, holderId);
        }

        public List<Permission> GetPermissionsByEntity(EntityType entityType, string entityId)
        {
            List<Permission> allPermissions = this.GetPermissions();

            BusinessEntityManager businessEntityManager = new BusinessEntityManager();
            List<BusinessEntityNode> businessEntityHierarchy = businessEntityManager.GetEntityNodes();

            BusinessEntityNode targetNode = null;

            foreach (BusinessEntityNode item in businessEntityHierarchy)
            {
                targetNode = item.Descendants().Where(x => x.EntType == entityType && x.EntityId.ToString() == entityId).FirstOrDefault();
                if (targetNode != null)
                {
                    break;
                }
            }

            List<Permission> permissionsResultList = new List<Permission>();

            do
            {
                List<Permission> nodePermissions = allPermissions.FindAll(x => x.EntityType == targetNode.EntType && x.EntityId == targetNode.EntityId.ToString());
                permissionsResultList.AddRange(nodePermissions);

                if (targetNode.BreakInheritance)
                    break;

                targetNode = targetNode.Parent;

            } while (targetNode != null);

            permissionsResultList = this.FillPermissionsListwithPathes(permissionsResultList, businessEntityHierarchy);

            return permissionsResultList;
        }

        public Vanrise.Entities.UpdateOperationOutput<object> DeletePermission(HolderType holderType, string holderId, EntityType entityType, string entityId)
        {
            UpdateOperationOutput<object> updateOperationOutput = new UpdateOperationOutput<object>();

            IPermissionDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IPermissionDataManager>();
            bool result = dataManager.DeletePermission(holderType, holderId, entityType, entityId);
            if (result)
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
            else
                updateOperationOutput.Result = UpdateOperationResult.Failed;

            updateOperationOutput.UpdatedObject = null;

            return updateOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<object> UpdatePermissions(IEnumerable<Permission> permissions)
        {
            UpdateOperationOutput<object> updateOperationOutput = new UpdateOperationOutput<object>();

            updateOperationOutput.Result = UpdateOperationResult.Succeeded;
            updateOperationOutput.UpdatedObject = null;

            IPermissionDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IPermissionDataManager>();

            foreach (Permission item in permissions)
            {
                if (item.PermissionFlags.Count == 0)
                {
                    if(!dataManager.DeletePermission(item))
                    {
                        updateOperationOutput.Result = UpdateOperationResult.Failed;
                    }
                }
                else if (!dataManager.UpdatePermission(item))
                {
                    updateOperationOutput.Result = UpdateOperationResult.Failed;
                }
            }

            return updateOperationOutput;
        }

        public EffectivePermissionsWrapper GetEffectivePermissions(int userId)
        {
            GroupManager roleManager = new GroupManager();
            List<int> groups = roleManager.GetUserGroups(userId);

            //TODO: consider these are read from the cache
            PermissionManager permissionManager = new PermissionManager();
            List<Permission> permissionsRecords = permissionManager.GetPermissions();

            //This should be read from cache
            BusinessEntityManager businessEntityManager = new BusinessEntityManager();
            List<BusinessEntityNode> businessEntityHierarchy = businessEntityManager.GetEntityNodes();

            List<Dictionary<string, Dictionary<string, Flag>>> listOfAllPermissions = new List<Dictionary<string, Dictionary<string, Flag>>>();

            List<Permission> userPermissions = permissionsRecords.FindAll(x => x.HolderType == HolderType.USER && x.HolderId == userId.ToString());
            listOfAllPermissions.Add(this.ConvertPermissionsToPathDictionary(userPermissions, businessEntityHierarchy));


            foreach (int groupId in groups)
            {
                List<Permission> rolePermissions = permissionsRecords.FindAll(x => x.HolderType == HolderType.ROLE && x.HolderId == groupId.ToString());
                if (rolePermissions.Count > 0)
                    listOfAllPermissions.Add(this.ConvertPermissionsToPathDictionary(rolePermissions, businessEntityHierarchy));
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

        private Dictionary<string, Dictionary<string, Flag>> ConvertPermissionsToPathDictionary(List<Permission> permissions, List<BusinessEntityNode> businessEntityHierarchy)
        {
            Dictionary<string, Dictionary<string, Flag>> effectivePermissions = new Dictionary<string, Dictionary<string, Flag>>();

            foreach (Permission item in permissions)
            {
                BusinessEntityNode result = null;

                //Get the node that matches the permission item in the business entity hierarchy tree
                foreach (BusinessEntityNode node in businessEntityHierarchy)
                {
                    result = node.Descendants().Where(x => x.EntType == item.EntityType && x.EntityId.ToString() == item.EntityId).FirstOrDefault();
                    if (result != null)
                    {
                        break;
                    }
                }

                //Map the Permission Flags into a dictionary of strings
                Dictionary<string, Flag> effectiveFlags = new Dictionary<string, Flag>();

                foreach (PermissionFlag flag in item.PermissionFlags)
                {
                    effectiveFlags.Add(flag.Name, flag.Value);
                }

                string relativePath = result.GetRelativePath();

                //The get relative path will convert the business entity node into a path string
                //Add a new record to the dictionary of effective permissions that is about the path as a key and the list of allowed permissions as a value
                //TODO: this is to be cached later and moved to the base API Controller and done once in a user session
                effectivePermissions.Add(relativePath, effectiveFlags);
            }

            return effectivePermissions;
        }

        private List<Permission> FillPermissionsListwithPathes(List<Permission> permissions, List<BusinessEntityNode> businessEntityHierarchy)
        {
            foreach (Permission item in permissions)
            {
                BusinessEntityNode result = null;

                //Get the node that matches the permission item in the business entity hierarchy tree
                foreach (BusinessEntityNode node in businessEntityHierarchy)
                {
                    result = node.Descendants().Where(x => x.EntType == item.EntityType && x.EntityId.ToString() == item.EntityId).FirstOrDefault();
                    if (result != null)
                    {
                        break;
                    }
                }
                
                item.PermissionPath = result.GetRelativePath();

            }

            return permissions;
        }

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

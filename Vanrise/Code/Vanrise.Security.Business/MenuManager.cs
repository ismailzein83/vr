using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Data;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Business
{
    public class MenuManager
    {
        public List<MenuItem> GetMenuItems(string token)
        {
            SecurityToken secToken = Common.Serializer.Deserialize<SecurityToken>(Common.TempEncryptionHelper.Decrypt(token));

            Dictionary<string, Dictionary<string, Flag>> effectivePermissions = this.GetEffectivePermissions(secToken.UserId);
            //string x = Common.Serializer.Serialize(effectivePermissions);

            IModuleDataManager moduleDataManager = SecurityDataManagerFactory.GetDataManager<IModuleDataManager>();
            List<Module> modules = moduleDataManager.GetModules();
            
            IViewDataManager viewDataManager = SecurityDataManagerFactory.GetDataManager<IViewDataManager>();
            List<View> views = viewDataManager.GetViews();

            List<MenuItem> retVal = new List<MenuItem>();

            foreach (Module item in modules)
            {
                if(item.ParentId == 0)
                {
                    MenuItem rootItem = GetModuleMenu(item, modules, views, effectivePermissions);
                    if(rootItem.Childs.Count > 0)
                        retVal.Add(rootItem);
                }
            }

            return retVal;
        }

        private MenuItem GetModuleMenu(Module module, List<Module> modules, List<View> views, Dictionary<string, Dictionary<string, Flag>> effectivePermissions)
        {
            MenuItem menu = new MenuItem() { Name = module.Name, Location = module.Url, Icon = module.Icon };

            List<Module> subModules = modules.FindAll(x => x.ParentId == module.ModuleId);

            List<View> childViews = views.FindAll(x => x.ModuleId == module.ModuleId);

            if (childViews.Count > 0)
            {
                menu.Childs = new List<MenuItem>();
                foreach (View viewItem in childViews)
                {
                    if(viewItem.RequiredPermissions == null || isAllowed(viewItem.RequiredPermissions, effectivePermissions))
                    {
                        MenuItem viewMenu = new MenuItem() { Name = viewItem.Name, Location = viewItem.Url };
                        menu.Childs.Add(viewMenu);
                    }
                }
            }

            if (subModules.Count > 0)
            {
                foreach (Module item in subModules)
                {
                    menu.Childs.Add(GetModuleMenu(item, modules, views, effectivePermissions));
                }
            }

            return menu;
        }


        private Dictionary<string, Dictionary<string, Flag>> GetEffectivePermissions(int userId)
        {
            //TODO: consider these are read from the cache
            PermissionManager permissionManager = new PermissionManager();
            List<Permission> permissionsRecords = permissionManager.GetPermissions();

            List<Dictionary<string, Dictionary<string, Flag>>> listOfAllPermissions = new List<Dictionary<string, Dictionary<string, Flag>>>();

            //This should be read from cache
            List<BusinessEntityNode> businessEntityHierarchy = permissionManager.GetEntityNodes();

            List<Permission> userPermissions = permissionsRecords.FindAll(x => x.HolderType == HolderType.USER && x.HolderId == userId.ToString());
            listOfAllPermissions.Add(this.ConvertPermissionsToPathDictionary(userPermissions, businessEntityHierarchy));

            RoleManager roleManager = new RoleManager();
            List<int> roles = roleManager.GetUserRoles(userId);

            foreach (int roleId in roles)
            {
                List<Permission> rolePermissions = permissionsRecords.FindAll(x => x.HolderType == HolderType.ROLE && x.HolderId == roleId.ToString());
                if(rolePermissions.Count > 0)
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

                    if (!allowPermissions.ContainsKey(permkvp.Key) && allowValues.Count > 0)
                        allowPermissions.Add(permkvp.Key, allowValues);

                    if (!denyPermissions.ContainsKey(permkvp.Key) && denyValues.Count > 0)
                        denyPermissions.Add(permkvp.Key, denyValues);
                }
            }

            //Filter the allowPermissions based on the denyPermissions list
            foreach (KeyValuePair<string, List<string>> allowPerm in allowPermissions)
            {
                if(denyPermissions.ContainsKey(allowPerm.Key))
                {
                    List<string> filteredValues = new List<string>();
                    foreach (string allowFlag in allowPerm.Value)
                    {
                        if(!denyPermissions[allowPerm.Key].Contains(allowFlag))
                        {
                            filteredValues.Add(allowFlag);
                        }
                    }
                    allowPerm.Value.Clear();
                    allowPerm.Value.AddRange(filteredValues);
                }
            }

            Dictionary<string, Dictionary<string, Flag>> finalResult = new Dictionary<string, Dictionary<string, Flag>>();

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

                finalResult.Add(item.Key, allflags);
            }

            //Now loop and add all missing denypermissions that we could not add in the previous loop
            foreach (KeyValuePair<string, List<string>> missingFlag in denyPermissions)
            {
                if(!finalResult.ContainsKey(missingFlag.Key))
                {
                    Dictionary<string, Flag> missingDenyflags = new Dictionary<string, Flag>();
                    foreach (string denyStr in missingFlag.Value)
                    {
                        missingDenyflags.Add(denyStr, Flag.DENY);
                    }

                    finalResult.Add(missingFlag.Key, missingDenyflags);
                }
            }

            return finalResult;
        }

        private Dictionary<string, Dictionary<string, Flag>> ConvertPermissionsToPathDictionary(List<Permission> permissions, List<BusinessEntityNode> businessEntityHierarchy)
        {
            Dictionary<string, Dictionary<string, Flag>> convertedPermissions = new Dictionary<string, Dictionary<string, Flag>>();

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

                //The get relative path will convert the business entity node into a path string
                //Add a new record to the dictionary of effective permissions that is about the path as a key and the list of allowed permissions as a value
                //TODO: this is to be cached later and moved to the base API Controller and done once in a user session
                convertedPermissions.Add(result.GetRelativePath(), effectiveFlags);
            }

            return convertedPermissions;
        }

        private BusinessEntityNode GetBusinessEntityNode(Permission permission, BusinessEntityNode node, List<BusinessEntityNode> businessEntityHierarchy)
        {
            if(node.EntType == permission.EntityType && node.EntityId.ToString() == permission.EntityId)
            {
                return node;
            }
            else if(node.Children != null && node.Children.Count > 0)
            {
                foreach(BusinessEntityNode subNode in node.Children)
                {
                    return GetBusinessEntityNode(permission, subNode, node.Children);
                }
            }

            return null;
        }

        private bool isAllowed(Dictionary<string, List<string>> requiredPermissions, Dictionary<string, Dictionary<string, Flag>> effectivePermissions)
        {
            //Assume that the view is allowed, and start looping until you find an exception that prevents the user from seeing this view
            bool result = true;

            foreach (KeyValuePair<string, List<string>> kvp in requiredPermissions)
            {
                result = CheckPermissions(kvp.Key, kvp.Value, effectivePermissions, new HashSet<string>());
                if (!result)
                    break;
            }

            return result;
        }

        private bool CheckPermissions(string requiredPath, List<string> requiredFlags, Dictionary<string, Dictionary<string, Flag>> effectivePermissions, HashSet<string> allowedFlags)
        {
            bool result = true;

            Dictionary<string, Flag> effectivePermissionFlags;
            if (effectivePermissions.TryGetValue(requiredPath, out effectivePermissionFlags))
            {
                Flag fullControlFlag;
                if (effectivePermissionFlags.TryGetValue("Full Control", out fullControlFlag))
                {
                    if (fullControlFlag == Flag.DENY)
                        return false;
                    else
                    {
                        foreach(var flag in requiredFlags)
                            allowedFlags.Add(flag);
                    }
                }
                else
                {
                    foreach (string requiredFlag in requiredFlags)
                    {
                        Flag effectivePermissionFlag;
                        if (effectivePermissionFlags.TryGetValue(requiredFlag, out effectivePermissionFlag))
                        {
                            if (effectivePermissionFlag == Flag.DENY)
                            {
                                return false;
                            }
                            else
                            {
                                allowedFlags.Add(requiredFlag);
                            }
                        }
                    }
                }
            }

            //The required path might be in one level up, then check it on that level recursively
            int index = requiredPath.LastIndexOf('/');
            if (index > 0)
            {
                //Keep looping recursively until you finish trimming the whole string requiredPath
                string oneLevelUp = requiredPath.Remove(index);
                result = CheckPermissions(oneLevelUp, requiredFlags, effectivePermissions, allowedFlags);
            }
            else
            {
                //Validation logic
                foreach (string item in requiredFlags)
                {
                    if (!allowedFlags.Contains(item))
                        return false;
                }
            }

            return result;
        }
        
    }
}

using System.Collections.Generic;
using System.Linq;
using NHibernate.Criterion;

namespace TABS.Security
{
    /// <summary>
    /// Represents a TABS User. Generally used by the Web Context to encapsulate options.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Gets or Sets the option value for the user
        /// </summary>
        /// <param name="option">The option name</param>
        /// <returns>The value for the given option</returns>
        public string this[string optionName] { get { return this.Entity[optionName]; } set { this.Entity[optionName] = value; } }
        internal static List<SecurityEssentials.User> _All;
        public static List<SecurityEssentials.User> All
        {
            get
            {
                if (_All != null) return _All;

                _All = new List<SecurityEssentials.User>();
                using (NHibernate.ISession session = SecurityEssentials.Configuration.Default.SessionFactory.OpenSession())
                {
                    _All = session.CreateCriteria(typeof(SecurityEssentials.User))
                        .AddOrder(new Order("Name", true))
                        .List<SecurityEssentials.User>().ToList();
                }
                return _All;
            }
        }

        public static void ClearCachedCollections()
        {
            _All = null;
        }

        public static List<SecurityEssentials.User> ActiveUsers
        {
            get { return All.Where(u => u.IsActive).ToList(); }
        }

        /// <summary>
        /// The Security Essentials' User Entity
        /// </summary>
        public SecurityEssentials.User Entity { get; protected set; }

        // Cannot create a user without a User Entity
        private User() { }

        /// <summary>
        /// Create a User with underlying Security Essentials User Entity 
        /// </summary>
        /// <param name="entity"></param>
        public User(SecurityEssentials.User entity)
        {
            this.Entity = entity;
        }

        /// <summary>
        /// Save the underlying user entity
        /// </summary>
        public void Save(bool validate, out string message)
        {
            this.Entity.Save(validate, out message);
        }

        /// <summary>
        /// Gets the set of roles that this user belongs to
        /// </summary>
        public Iesi.Collections.Generic.ISet<SecurityEssentials.Role> Roles { get { return this.Entity.Roles; } }

        /// <summary>
        /// Determine if this user has a given permission
        /// </summary>
        /// <param name="permission">The permission wanted</param>
        /// <returns></returns>
        public bool HasPermission(SecurityEssentials.Permission permission)
        {
            return this.Entity.HasPermission(permission);
        }

        /// <summary>
        /// Determine if this user has a given permission
        /// </summary>
        /// <param name="permission">The name of the permission wanted</param>
        /// <returns>False if user has a permission by the given name</returns>
        public bool HasPermission(string permissionPath)
        {
            SecurityEssentials.Permission wanted = SecurityEssentials.Permission.Get(permissionPath);
            return (wanted != null) ? this.Entity.HasPermission(wanted) : false;
        }


    }
}

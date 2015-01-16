using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace TABS
{
    public partial class ObjectAssembler
    {
        static log4net.ILog log = log4net.LogManager.GetLogger(typeof(ObjectAssembler));

        #region Refreshable Types Area

        protected static System.Reflection.TypeFilter typeFilter = new System.Reflection.TypeFilter(IsTypeFullName);

        internal static bool IsTypeFullName(Type typeObj, Object criteriaObj)
        {
            if (typeObj.FullName == criteriaObj.ToString())
                return true;
            else
                return false;
        }

        /// <summary>
        /// Clear "All" Collections (to rebuild them)
        /// </summary>
        public static void ClearAllCollections()
        {
            lock (SyncRoot)
            {
                // Security Essentials Collections
                SecurityEssentials.WebsiteMenuItem.RefreshAll();
                SecurityEssentials.Role.RefreshAll();
                SecurityEssentials.Permission.RefreshAll();

                // TABS Collections
                foreach (Type type in GetRefreshableTypes())
                {
                    try
                    {
                        ClearCachedCollections(type);
                    }
                    catch (Exception ex)
                    {
                        log.Error("Could not Clear All Collections for Type " + type, ex);
                    }
                }

                // Reload Services
                log.InfoFormat("Cleared All Collections. System has: {0} Flagged Services, {1} Carrier Accounts", FlaggedService.All.Count, CarrierAccount.All.Count);
            }
        }

        /// <summary>
        /// Invoke the ClearCachedCollections for the type specified. 
        /// The type must inherit the interface <typeparamref name="TABS.Interfaces.ICachedCollectionContainer">ICachedCollectionContainer</typeparamref>
        /// </summary>
        /// <param name="type">The type for which to clear the cached collections</param>
        public static void ClearCachedCollections(Type type)
        {
            type.InvokeMember("ClearCachedCollections", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.InvokeMethod | System.Reflection.BindingFlags.IgnoreReturn, null, null, null);
        }

        public static List<Type> GetRefreshableTypes()
        {
            List<Type> types = new List<Type>();

            // TABS Assembly
            foreach (Type type in System.Reflection.Assembly.GetExecutingAssembly().GetTypes())
            {
                if (!type.IsInterface)
                    if (type.FindInterfaces(typeFilter, "TABS.Interfaces.ICachedCollectionContainer").Length > 0)
                        types.Add(type);
            }

            // Plugins
            foreach (Type type in TABS.Plugins.Framework.PluginManager.PluginAssemblies.SelectMany(a => a.GetTypes()))
            {
                if (!type.IsInterface)
                    if (type.FindInterfaces(typeFilter, "TABS.Interfaces.ICachedCollectionContainer").Length > 0)
                        types.Add(type);
            }

            // Addons
            foreach (Type type in Addons.AddonManager.AddonAssemblies.SelectMany(a => a.GetTypes()))
            {
                if (!type.IsInterface)
                    if (type.FindInterfaces(typeFilter, "TABS.Interfaces.ICachedCollectionContainer").Length > 0)
                        types.Add(type);
            }

            return types;
        }

        #endregion Refreshable Types Area

        /// <summary>
        /// Get an object with a known type and return it (strongly typed).
        /// Note: the type has to be known to NHibernated (mapping?)
        /// </summary>
        /// <typeparam name="T">The type of the object</typeparam>
        /// <param name="id">The object's id (it has to be of the same type as the id defined in the mapping!</param>
        /// <returns>The object if found, and null otherwise</returns>
        public static T Get<T>(object id)
        {
            if (CurrentSession != null)
            {
                return CurrentSession.Get<T>(id);
            }
            else
            {
                T result = default(T);
                using (NHibernate.ISession session = DataConfiguration.OpenSession())
                {
                    result = session.Get<T>(id);
                }
                return result;
            }
        }

        /// <summary>
        /// Load an object of the given class
        /// </summary>
        /// <param name="persistedType">The Object's Type (Class)</param>
        /// <param name="objectId">The ID</param>
        /// <returns>The object or null if it could not find it</returns>
        public static object Load(Type persistedType, object objectId)
        {
            return CurrentSession.Load(persistedType, objectId);
        }

        /// <summary>
        /// Return a list of all objects of the given class
        /// </summary>
        public static IList<T> GetList<T>()
        {
            if (System.Web.HttpContext.Current == null)
            {
                IList<T> list;
                using (NHibernate.ISession session = DataConfiguration.OpenSession())
                {
                    list = session.CreateCriteria(typeof(T)).List<T>();
                }
                return list;
            }
            else
                return CurrentSession.CreateCriteria(typeof(T)).List<T>();
        }

        /// <summary>
        /// Try to save  the object passed.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool Save(object obj, out Exception ex)
        {
            bool success = false;
            NHibernate.ITransaction transaction = CurrentSession.BeginTransaction();
            try
            {
                CurrentSession.Save(obj);
                transaction.Commit();
                success = true;
                ex = null;
            }
            catch (Exception e)
            {
                log.Error("SaveOrUpdate Error", e);
                ex = e;
                transaction.Rollback();
                success = false;
            }
            return success;
        }
        public static bool Save(object obj, NHibernate.ITransaction transaction,bool CommitTransactiononSucess, out Exception ex)
        {
            bool success = false;
            
            try
            {
                CurrentSession.Save(obj);
                if(CommitTransactiononSucess==true)
                transaction.Commit();
                success = true;
                ex = null;
            }
            catch (Exception e)
            {
                log.Error("SaveOrUpdate Error", e);
                ex = e;
                if (CommitTransactiononSucess == true)
                   transaction.Rollback();
                success = false;
            }
            return success;
        }
        public static bool Update(object obj, out Exception ex)
        {
            bool success = false;
            NHibernate.ITransaction transaction = CurrentSession.BeginTransaction();
            try
            {
                CurrentSession.Update(obj);
                transaction.Commit();
                success = true;
                ex = null;
            }
            catch (Exception e)
            {
                log.Error("Update Error - Object: " + obj.ToString(), e);
                ex = e;
                transaction.Rollback();
                success = false;
            }
            return success;
        }
        public static bool Update(object obj,NHibernate.ITransaction transaction,bool CommitTransactiononSucess, out Exception ex)
        {
            bool success = false;
            //NHibernate.ITransaction transaction = CurrentSession.BeginTransaction();
            try
            {
                CurrentSession.Update(obj);
                if(CommitTransactiononSucess==true)
                transaction.Commit();
                success = true;
                ex = null;
            }
            catch (Exception e)
            {
                log.Error("Update Error - Object: " + obj.ToString(), e);
                ex = e;
                if(CommitTransactiononSucess==true)
                transaction.Rollback();
                success = false;
            }
            return success;
        }
        /// <summary>
        /// Try to save (or update) the object passed.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool SaveOrUpdate(object obj, out Exception ex)
        {
            bool success = false;

            using (NHibernate.ITransaction transaction = CurrentSession.BeginTransaction())
            {
                try
                {
                    CurrentSession.SaveOrUpdate(obj);
                    transaction.Commit();
                    success = true;
                    ex = null;
                }
                catch (Exception e)
                {
                    log.Error(string.Format("SaveOrUpdate Error - Type: {0}, Object: {1}", obj.GetType(), obj), e);
                    ex = e;
                    transaction.Rollback();
                    success = false;
                }
            }
            return success;
        }
        public static bool SaveOrUpdate(object obj,NHibernate.ITransaction transaction,bool CommitTransactiononSucess, out Exception ex)
        {
            bool success = false;

           // using (NHibernate.ITransaction transaction = CurrentSession.BeginTransaction())
            //{
                try
                {
                    CurrentSession.SaveOrUpdate(obj);
                    if(CommitTransactiononSucess==true)
                       transaction.Commit();
                    success = true;
                    ex = null;
                }
                catch (Exception e)
                {
                    log.Error(string.Format("SaveOrUpdate Error - Type: {0}, Object: {1}", obj.GetType(), obj), e);
                    ex = e;
                   if(CommitTransactiononSucess==true)
                    transaction.Rollback();
                    success = false;
                }
            //}
            return success;
        }
     
        /// <summary>
        /// Try to delete the enumerable passed.
        /// </summary>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        public static bool Delete(IEnumerable enumerable, out Exception ex)
        {
            bool success = false;
            ex = null;

            if (enumerable.GetEnumerator().MoveNext())
            {
                enumerable.GetEnumerator().Reset();

                NHibernate.ITransaction transaction = CurrentSession.BeginTransaction();
                try
                {
                    foreach (object obj in enumerable)
                        CurrentSession.Delete(obj);
                    transaction.Commit();
                    success = true;
                    ex = null;
                }
                catch (Exception e)
                {
                    log.Error("Delete Error", e);
                    ex = e;
                    transaction.Rollback();
                    success = false;
                }
            }
            else
            {
                // No elements, return true?
                success = true;
            }
            return success;
        }

        /// <summary>
        /// Save a collection of objects into the database using a single transaction (session as well).
        /// </summary>
        /// <param name="enumerable"></param>
        /// <param name="ex"></param>
        /// <returns>True on success, false otherwise (exception if any is returned in ex)</returns>
        public static bool SaveOrUpdate(NHibernate.ISession session, IEnumerable enumerable, out Exception ex)
        {
            bool success = false;
            ex = null;

            if (enumerable.GetEnumerator().MoveNext())
            {
                try { enumerable.GetEnumerator().Reset(); }
                catch { }

                NHibernate.ITransaction transaction = session.BeginTransaction();
                try
                {
                    foreach (object obj in enumerable)
                        session.SaveOrUpdate(obj);
                    transaction.Commit();
                    success = true;
                    ex = null;
                }
                catch (Exception e)
                {
                    ex = e;
                    transaction.Rollback();
                    success = false;
                }
            }
            else
            {
                // No elements, return true?
                success = true;
            }
            return success;
        }
        public static bool SaveOrUpdate(NHibernate.ISession session, IEnumerable enumerable, NHibernate.ITransaction transaction,bool CommitTransactiononSucess, out Exception ex)
        {
            bool success = false;
            ex = null;

            if (enumerable.GetEnumerator().MoveNext())
            {
                try { enumerable.GetEnumerator().Reset(); }
                catch { }

                //NHibernate.ITransaction transaction = session.BeginTransaction();
                try
                {
                    foreach (object obj in enumerable)
                        session.SaveOrUpdate(obj);
                    if(CommitTransactiononSucess==true)
                       transaction.Commit();
                    success = true;
                    ex = null;
                }
                catch (Exception e)
                {
                    ex = e;
                    if( CommitTransactiononSucess==true)
                    transaction.Rollback();
                    success = false;
                }
            }
            else
            {
                // No elements, return true?
                success = true;
            }
            return success;
        }


        /// <summary>
        /// Save a collection of objects into the database using a single transaction, and the current Session.
        /// </summary>
        /// <param name="enumerable"></param>
        /// <param name="ex"></param>
        /// <returns>True on success, false otherwise (exception if any is returned in ex)</returns>
        public static bool SaveOrUpdate(IEnumerable enumerable, out Exception ex)
        {
            return SaveOrUpdate(CurrentSession, enumerable, out ex);
        }
        public static bool SaveOrUpdate(IEnumerable enumerable, NHibernate.ITransaction transaction, bool CommitTransactiononSucess, out Exception ex)
        {
            return SaveOrUpdate(CurrentSession, enumerable,transaction,CommitTransactiononSucess, out ex);
        }
        /// <summary>
        /// Save XOR Update a collection of objects into the database using a single transaction (session as well).
        /// I.E Use either a "Save" or an "Update" for all the enumerables.
        /// </summary>
        /// <param name="enumerable"></param>
        /// <param name="ex"></param>
        /// <returns>True on success, false otherwise (exception if any is returned in ex)</returns>
        public static bool SaveXorUpdate(NHibernate.ISession session, IEnumerable enumerable, out Exception ex, bool useSave)
        {
            bool success = false;
            ex = null;
            object toSave = null;
            int count = -1;
            NHibernate.ITransaction transaction = session.BeginTransaction();
            try
            {
                foreach (object obj in enumerable)
                {
                    toSave = obj;
                    count++;
                    success = true;
                    if (useSave) session.Save(obj);
                    else session.Update(obj);
                }
                if (success) transaction.Commit();
                ex = null;
            }
            catch (Exception e)
            {
                ex = new Exception(string.Format("Error While Saving Object {0} at count: {1}", toSave, count), e);
                transaction.Rollback();
                success = false;
            }
            return success;
        }

        /// <summary>
        /// Save XOR Update a collection of objects into the database using a single transaction (session as well).
        /// I.E Use either a "Save" or an "Update" for all the enumerables.
        /// </summary>
        /// <param name="enumerable"></param>
        /// <param name="ex"></param>
        /// <returns>True on success, false otherwise (exception if any is returned in ex)</returns>
        public static bool SaveXorUpdate(IEnumerable enumerable, out Exception ex, bool useSave)
        {
            return SaveXorUpdate(CurrentSession, enumerable, out ex, useSave);
        }

        /// <summary>
        /// Try to delete the object passed.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool Delete(object obj)
        {
            bool success = false;
            using (NHibernate.ISession session = DataConfiguration.OpenSession())
            {
                NHibernate.ITransaction transaction = session.BeginTransaction();
                try
                {
                    session.Delete(obj);
                    transaction.Commit();
                    success = true;
                }
                catch
                {
                    transaction.Rollback();
                    success = false;
                }
            }
            return success;
        }

        /// <summary>
        /// Try to delete the object passed.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool Delete(object obj, out Exception ex)
        {
            ex = null;
            bool success = false;
            using (NHibernate.ISession session = DataConfiguration.OpenSession())
            {
                NHibernate.ITransaction transaction = session.BeginTransaction();
                try
                {
                    session.Delete(obj);
                    transaction.Commit();
                    success = true;
                }
                catch(Exception e)
                {
                    transaction.Rollback();
                    ex = e;
                    success = false;
                }
            }
            return success;
        }

    }
}

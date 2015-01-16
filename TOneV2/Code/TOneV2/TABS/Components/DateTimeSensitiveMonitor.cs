using System;

namespace TABS.Components
{
    public class DateTimeSensitiveMonitor
    {
        // The one and only instance
        static DateTimeSensitiveMonitor instance = new DateTimeSensitiveMonitor();

        // The set of Entities to watch
        Iesi.Collections.Generic.HashedSet<Interfaces.IDateTimeSensitive> _Entities;

        /// <summary>
        /// Create the Instance Monitor
        /// </summary>
        private DateTimeSensitiveMonitor()
        {
            // Create the monitored entities Collection
            _Entities = new Iesi.Collections.Generic.HashedSet<TABS.Interfaces.IDateTimeSensitive>();
            
            // Create the type monitors (for own zones, ToD considerations, etc...)
            foreach (Type type in ObjectAssembler.GetRefreshableTypes())
            {
                TypeMonitor typeMonitor = new TypeMonitor(type);
                _Entities.Add(typeMonitor);
            }
        }

        /// <summary>
        /// The Monitoring Execution Function
        /// </summary>
        protected void RefreshMonitored()
        {
            lock (typeof(DateTimeSensitiveMonitor))
            {
                // Refresh all monitored entities
                foreach (Interfaces.IDateTimeSensitive entity in _Entities)
                    entity.Refresh();
            }
        }

        /// <summary>
        /// Start Monitoring an entity
        /// </summary>
        /// <param name="entity">The date-time sensitive entity to monitor</param>
        /// <returns>True if entity was already monitored, false otherwise.</returns>
        public static bool Add(Interfaces.IDateTimeSensitive entity)
        {
            lock (typeof(DateTimeSensitiveMonitor))
            {
                // If another equivalent entity was monitored, it will be removed
                bool IsRemoved = Remove(entity);
                instance._Entities.Add(entity);
                return IsRemoved;
            }
        }

        /// <summary>
        /// Stop monitoring an entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static bool Remove(Interfaces.IDateTimeSensitive entity)
        {
            lock (typeof(DateTimeSensitiveMonitor))
            {
                return instance._Entities.Remove(entity);
            }
        }

        /// <summary>
        /// Refresh all monitored instances
        /// </summary>
        /// <returns></returns>
        public static void Refresh()
        {
            lock (typeof(DateTimeSensitiveMonitor))
            {
                instance.RefreshMonitored();
            }
        }
    }
}

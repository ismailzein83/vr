using System;

namespace TABS.Components
{
    [Serializable]
    public class NotifierSet<T> : Iesi.Collections.Generic.HashedSet<T>
    {      
        private bool _AllowNotifications = true;
        
        public delegate void CollectionChangedDelegate(T item, ItemChangeType changeType);
        public event CollectionChangedDelegate CollectionChanged;

        public override bool Add(T o)
        {
            bool added = base.Add(o);
            if (AllowNotifications && added)
                if (CollectionChanged != null) CollectionChanged(o, ItemChangeType.ItemAdded);
            return added;
        }

        public override bool Remove(T o)
        {
            bool removed = base.Add(o);
            if (AllowNotifications && removed)
                if (CollectionChanged != null) CollectionChanged(o, ItemChangeType.ItemRemoved);
            return removed;
        }

        public bool AllowNotifications
        {
            get
            {
                return _AllowNotifications;
            }
            set
            {
                _AllowNotifications = value;
            }
        }

    }
}

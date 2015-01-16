using System.Collections.Generic;

namespace TABS.Lookups
{
    /// <summary>
    /// Lookup types are lookups for general purpose usage like countries, etc...
    /// </summary>
    public class LookupType: Interfaces.ICachedCollectionContainer
    {
        #region Static
        internal static Dictionary<string, LookupType> _All;
        
        /// <summary>
        /// All the lookup types, the key is the "Name" of the type.
        /// </summary>
        public static Dictionary<string, LookupType> All
        {
            get
            {
                lock (ObjectAssembler.SyncRoot)
                {

                    if (_All == null)
                    {
                        _All = ObjectAssembler.GetAllLookupTypes();
                    }
                }
                return _All;
            }
        }

        /// <summary>
        /// Needed for marker interface to be called by reflection
        /// </summary>
        public static void ClearCachedCollections()
        {
            _All = null;
            TABS.Components.CacheProvider.Clear(typeof(LookupType).FullName);
        }

        #endregion Static

        #region Data Members
        protected int _LookupTypeID;
        protected string _Name;
        protected IList<LookupValue> _Values;
        
        public virtual int LookupTypeID
        {
            get
            {
                return _LookupTypeID;
            }
        }

        public virtual string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
            }
        }

        public virtual IList<LookupValue> Values
        {
            get
            {
                return _Values;
            }
            set
            {
                _Values = value;
            }
        }

        public override string ToString()
        {
            return Name;
        }
        #endregion Data Members

        #region Business Members

        /// <summary>
        /// Move the Lookup Value at the given index in the given direction.
        /// </summary>
        /// <param name="index">The index at which the Lookup Value will change position</param>
        /// <param name="up">True if the Value will move Up - False will move down.</param>
        /// <returns>True only if operation is successful</returns>
        public virtual bool MoveValue(int index, bool isUp)
        {
            int otherIndex = isUp ? index - 1 : index + 1;
            if (index >= 0 && otherIndex >= 0 && index < Values.Count && otherIndex < Values.Count)
            {
                LookupValue value = Values[index];
                LookupValue otherValue = Values[otherIndex];
                int otherOrdinal = otherValue.Ordinal;
                
                // Swap ordinals
                otherValue.Ordinal = value.Ordinal;
                value.Ordinal = otherOrdinal;

                // Swap indexes
                Values[index] = otherValue;
                Values[otherIndex] = value;

                return true;
            }
            else
                return false;
        }

        #endregion Business Members
    }
}

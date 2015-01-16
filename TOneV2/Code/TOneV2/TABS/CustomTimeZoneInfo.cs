using System;
using System.Collections.Generic;

/// <summary>
/// Summary description for CustomTimeZoneInfo
/// </summary>
namespace TABS
{
    public class CustomTimeZoneInfo : Components.BaseEntity, Interfaces.ICachedCollectionContainer
    {
        public CustomTimeZoneInfo() { }

        public virtual int ID { get; set; }
        public virtual short BaseUtcOffset { get; set; } // in minutes 
        public virtual string DisplayName { get; set; }

        public virtual TimeSpan BaseUtcOffsetTimeSpan { get { return TimeSpan.FromMinutes(BaseUtcOffset); } }

        internal static List<CustomTimeZoneInfo> _All;
        public static List<CustomTimeZoneInfo> All
        {
            get
            {
                if (_All != null) return _All;
                _All = TABS.ObjectAssembler.GetAllTimeZones();
                if (_All == null || _All.Count == 0)
                {
                    CustomTimeZoneInfo info = ObjectAssembler.CreateCustomTimeZoneInfo();
                    _All.Add(info);
                }
                return _All;
            }
        }

        public override string Identifier { get { return "CustomTimeZoneInfo:" + ID; } }

        /// <summary>
        /// Needed for marker interface to be called by reflection
        /// </summary>
        public static void ClearCachedCollections()
        {
            _All = null;
            TABS.Components.CacheProvider.Clear(typeof(CustomTimeZoneInfo).FullName);
        }


        public string FullName { get { return string.Concat(this.BaseUtcOffsetTimeSpan.ToString(), this.DisplayName); } }

        public override bool Equals(object obj)
        {
            CustomTimeZoneInfo that = obj as CustomTimeZoneInfo;
            if (that == null) return false;
            return this.FullName.Trim().ToLower().Equals(that.FullName.Trim().ToLower());
        }

        public override int GetHashCode() { return this.FullName.GetHashCode(); }
    }
}
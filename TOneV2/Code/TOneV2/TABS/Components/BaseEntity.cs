using System;
using System.Xml.Serialization;

namespace TABS.Components
{
    [Serializable]
    public abstract class BaseEntity : NHibernate.Classic.ILifecycle
    {
        public virtual bool UserTrackingEnabled { get; set; }
        public abstract string Identifier { get; }
        [XmlIgnore]
        private SecurityEssentials.User _User;
        [XmlIgnore]
        public virtual SecurityEssentials.User User
        {
            get { return _User; }
            set { _User = value; }
        }

        public BaseEntity() { this.User = SecurityEssentials.Web.Helper.CurrentWebUser; UserTrackingEnabled = true; }

        #region ILifecycle Members

        public virtual NHibernate.Classic.LifecycleVeto OnDelete(NHibernate.ISession s)
        {
            this.User = SecurityEssentials.Web.Helper.CurrentWebUser;
            if (this.User == SecurityEssentials.User.Unknown) this.User = null;
            return NHibernate.Classic.LifecycleVeto.NoVeto;
        }

        public virtual void OnLoad(NHibernate.ISession s, object id)
        {
            /*
            Interfaces.IDateTimeSensitive entity = this as Interfaces.IDateTimeSensitive;
            if (entity != null) DateTimeSensitiveMonitor.Add(entity);
             */
        }
 
        public virtual NHibernate.Classic.LifecycleVeto OnSave(NHibernate.ISession s)
        {
            if (UserTrackingEnabled)
            {
                this.User = SecurityEssentials.Web.Helper.CurrentWebUser;
                if (this.User == SecurityEssentials.User.Unknown) this.User = null;
            }
            return NHibernate.Classic.LifecycleVeto.NoVeto;
        }

        public virtual NHibernate.Classic.LifecycleVeto OnUpdate(NHibernate.ISession s)
        {
            if (UserTrackingEnabled)
            {
                this.User = SecurityEssentials.Web.Helper.CurrentWebUser;
                if (this.User == SecurityEssentials.User.Unknown) this.User = null;
            }
            return NHibernate.Classic.LifecycleVeto.NoVeto;
        }

        #endregion
        
        public override bool Equals(object obj)
        {
            BaseEntity other = obj as BaseEntity;
            if (other != null)
                return this.Identifier.Equals(other.Identifier);
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            if (Identifier.EndsWith(":0")) return base.GetHashCode();
            else return Identifier.GetHashCode();
        }
        
        public override string ToString()
        {
            return Identifier;
        }
    }
}
using System;

namespace TABS.Lookups
{
    public class LookupValue : IComparable
    {
        protected int _LookupValueID;
        protected string _Value;
        protected LookupType _LookupType;
        protected int _Ordinal;

        public virtual int LookupValueID
        {
            get
            {
                return _LookupValueID;
            }
        }

        public virtual string Value
        {
            get
            {
                return _Value;
            }
            set
            {
                _Value = value;
            }
        }

        public virtual LookupType LookupType
        {
            get
            {
                return _LookupType;
            }
            set
            {
                _LookupType=value;
            }

        }

        public virtual int Ordinal
        {
            get
            {
                return _Ordinal;
            }
            set
            {
                _Ordinal = value;
            }
        }

        public override bool Equals(object obj)
        {
            LookupValue other = obj as LookupValue;
            if (other != null)
            {
                if (this.LookupValueID > 0 && this.LookupValueID == other.LookupValueID) 
                    return true;
                else
                    return (this.LookupType == other.LookupType && this.Value == other.Value);                 
            }
            else return false;
        }

        public override int GetHashCode()
        {
            return (LookupType.Name+":"+this.Value).GetHashCode();
        }

        public override string ToString()
        {
            return Value;
        }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            LookupValue other = obj as LookupValue;
            if (other != null)
                return this.Value.CompareTo(other.Value);
            else
                return 0;
        }

        #endregion
    }
}

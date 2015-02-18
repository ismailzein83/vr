using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Entities
{
    public class Zone
    {
        protected int _ZoneID;
        public virtual int ZoneID
        {
            get { return _ZoneID; }
            set { _ZoneID = value; }
        }

        protected CodeGroupInfo _CodeGroup;
        public virtual CodeGroupInfo CodeGroup
        {
            get { return _CodeGroup; }
            set { _CodeGroup = value; }
        }

        protected string _Name;
        public virtual string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        protected string _SupplierID;
        public virtual string SupplierID
        {
            get { return _SupplierID; }
            set { _SupplierID = value; }
        }

        protected short _ServicesFlag;
        public virtual short ServicesFlag
        {
            get { return _ServicesFlag; }
            set { _ServicesFlag = value; }
        }

        protected Nullable<DateTime> _BeginEffectiveDate;
        protected Nullable<DateTime> _EndEffectiveDate;

        public virtual Nullable<DateTime> BeginEffectiveDate
        {
            get { return _BeginEffectiveDate; }
            set { _BeginEffectiveDate = value; }
        }

        public virtual Nullable<DateTime> EndEffectiveDate
        {
            get { return _EndEffectiveDate; }
            set { _EndEffectiveDate = value; }
        }

    }
}

using System;
using System.Collections.Generic;

namespace TABS
{
    [Serializable]
    public abstract class PriceListBase : Components.DateTimeEffectiveEntity , System.ComponentModel.IListSource
    {
        #region Data Members

        private int _ID;
        private bool _IsSent;
        private CarrierAccount _Supplier;
        private CarrierAccount _Customer;
        private string _Description;
        private Currency _Currency;
        private string _SourceFileName;

        public virtual bool IsSent
        {
            get { return _IsSent; }
            set { _IsSent = value; }
        } 
        public virtual int ID
        {
            get { return _ID; }
            set { _ID = value; }
        }       

        public virtual CarrierAccount Supplier
        {
            get { return _Supplier; }
            set { _Supplier = value; }
        }

        public virtual CarrierAccount Customer
        {
            get { return _Customer; }
            set { _Customer = value; }
        }

        public virtual string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }

        public virtual Currency Currency
        {
            get { return _Currency; }
            set { _Currency = value; }
        }       

        public virtual string SourceFileName
        {
            get { return _SourceFileName; }
            set { _SourceFileName = value; }
        }

        #endregion Data Members

        #region Business Members
        
        protected Dictionary<Zone, RateBase> _Rates;

        public virtual Dictionary<Zone, RateBase> Rates
        {
            get
            {
                if (_Rates == null)
                {
                    _Rates = ObjectAssembler.GetRates(this);
                }
                return _Rates;
            }
            set
            {
                _Rates = value;
            }
        }
        #endregion Business Members

        public PriceListBase()
        {

        }

        protected PriceListBase(int id)
        {
            _ID = id;
        }

        public override void Deactivate(bool isRecursive, DateTime when)
        {
            if (isRecursive)
            {
                foreach (Rate rate in this.Rates.Values)
                    rate.Deactivate(isRecursive, when);
            }
            EndEffectiveDate = when;
        }

		#region IListSource Members

		public bool ContainsListCollection
		{
			get { return false; }
		}

		public System.Collections.IList GetList()
		{
			List<RateBase> rates = new List<RateBase>(this.Rates.Values);
            return rates;
		}

		#endregion        

    }
}

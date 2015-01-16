using System;
using System.Collections.Generic;

namespace TABS
{
    [Serializable]
    public class Currency : Components.BaseEntity, Interfaces.ICachedCollectionContainer
    {
        /// <summary>
        /// Needed for marker interface to be called by reflection
        /// </summary>
        public static void ClearCachedCollections()
        {
            _All = null;
            _Main = null;
            _Visible = null;
            _Hidden = null;
            TABS.Components.CacheProvider.Clear(typeof(Currency).FullName);
        }

        public override string Identifier { get { return "Currency:" + Symbol; } }

        #region Static
        internal static Currency _Main;

        /// <summary>
        /// Get or set the main currency for the system
        /// </summary>
        public static Currency Main
        {
            get
            {
                if (_Main == null)
                {
                    _Main = ObjectAssembler.GetMainCurrency();
                }
                return _Main;
            }
            set
            {
                _Main = value;
            }
        }

        internal static Dictionary<string, Currency> _All, _Visible, _Hidden;
        public static Dictionary<string, Currency> All
        {
            get
            {
                if (_All == null)
                {
                    _All = ObjectAssembler.GetAllCurrencies();
                }
                return _All;
            }
        }

        /// <summary>
        /// The list of visible currencies
        /// </summary>
        public static Dictionary<string, Currency> Visible
        {
            get
            {
                if (_Visible == null)
                {
                    _Visible = new Dictionary<string, Currency>(All.Count);
                    foreach (Currency currency in All.Values)
                        //if (currency.IsVisible)
                        _Visible[currency.Symbol] = currency;
                }
                return _Visible;
            }
        }
        public static Dictionary<string, Currency> Hidden
        {
            get
            {
                if (_Hidden == null)
                {
                    _Hidden = new Dictionary<string, Currency>(All.Count);
                    foreach (Currency currency in All.Values)
                        if (!currency.IsVisible)
                            _Hidden[currency.Symbol] = currency;
                }
                return _Hidden;
            }
        }
        #endregion Static

        #region Data Members

        private string _Symbol = string.Empty;
        private string _Name = string.Empty;
        private string _IsMainCurrency;
        private string _IsVisible;
        private float _LastRate;
        private DateTime? _LastUpdated;

        /// <summary>
        /// Gets the name of this currency
        /// </summary>
        public virtual string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        /// <summary>
        /// Gets the Symbol for this currency
        /// </summary>
        public virtual string Symbol
        {
            get { return _Symbol; }
            set { _Symbol = value; }
        }

        /// <summary>
        /// Sets or Gets if this Currency is the main currency 
        /// </summary>
        public virtual bool IsMainCurrency
        {
            get { return "Y".Equals(_IsMainCurrency); }
            set { _IsMainCurrency = value ? "Y" : "N"; }
        }

        /// <summary>
        /// Set the visibility state of a currency
        /// </summary>
        public virtual bool IsVisible
        {
            get { return "Y".Equals(_IsVisible); }
            set
            {
                _IsVisible = value ? "Y" : "N";

                // Update Visible currencies
                if (value) Currency.Visible[this.Symbol] = this;
                else Currency.Visible.Remove(this.Symbol);

                // Update Hidden currencies
                if (!value) Currency.Hidden[this.Symbol] = this;
                else Currency.Hidden.Remove(this.Symbol);
            }
        }

        /// <summary>
        /// Sets or gets the last rate for this currency
        /// </summary>
        public virtual float LastRate
        {
            get { return _LastRate; }
            set { _LastRate = value; }
        }

        public virtual DateTime? LastUpdated
        {
            get { return _LastUpdated; }
            set { _LastUpdated = value; }
        }

        #endregion Data Memebers

        public string LongName
        {
            get
            {
                return string.Format("[{0}] {1}", Symbol, Name);
            }

        }

        public virtual float LastInverseRate
        {
            get
            {
                return (1.0f / LastRate);
            }
        }

        public override int GetHashCode()
        {
            return this.Symbol.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            Currency other = obj as Currency;
            if (other == null)
                return false;
            else
                return other.Symbol.Equals(this.Symbol);
        }

        public override string ToString()
        {
            return Symbol;
        }

        /// <summary>
        /// Update all currencies (write changes to database)
        /// </summary>
        public static void UpdateAll()
        {
            ObjectAssembler.UpdateAllCurrencies();
        }

        public string ExchangeRateDisplay
        {
            get
            {
                return string.Format("1 {0}  = {1:0.000000} {2}", Main.Symbol, this.LastRate, this.Symbol);
            }
        }


        public string InverseExchangeRateDisplay
        {
            get
            {
                return string.Format("1 {0}  = {1:0.000000} {2}", this.Symbol, this.LastInverseRate, Main.Symbol);
            }
        }
    }
}
using System;

namespace TABS
{
    public class CarrierAccountConnection : Components.BaseEntity, IComparable<CarrierAccountConnection>
    {
        public override string Identifier { get { return "CarrierAccountConnection:" + ID; } }

        #region Data Members

        private int _ID;
        private ConnectionType _ConnectionType;
        private string _TAG = string.Empty;
        private string _Value = string.Empty;
        private string _GateWay = string.Empty;
        private CarrierAccount _CarrierAccount;
        private Switch _Switch;

        public virtual int ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        public virtual CarrierAccount CarrierAccount
        {
            get { return _CarrierAccount; }
            set { _CarrierAccount = value; }
        }

        public virtual Switch Switch
        {
            get { return _Switch; }
            set { _Switch = value; }
        }

        public virtual ConnectionType ConnectionType
        {
            get { return _ConnectionType; }
            set { _ConnectionType = value; }
        }

        public virtual string TAG
        {
            get { return _TAG; }
            set { _TAG = value; }
        }

        public virtual string Value
        {
            get { return _Value; }
            set { _Value = value; }
        }

        public virtual string GateWay
        {
            get { return _GateWay; }
            set { _GateWay = value; }
        }

        public virtual string DefinitionDisplay
        {
            get
            {
                return string.Format("[{0}] {1} - {2} : {3} - GW : {4}", this.Switch, this.ConnectionType, this.TAG, this.Value, this.GateWay);
            }
        }

        public override string ToString()
        {
            return DefinitionDisplay;
        }

        #endregion Data Memebers

        #region IComparable<CarrierAccountConnection> Members

        public int CompareTo(CarrierAccountConnection other)
        {
            int result = 0;
            if (this.Switch != other.Switch)
                result = this.Switch.Name.CompareTo(other.Switch.Name);
            else
                if (this.ConnectionType != other.ConnectionType)
                    result = this.ConnectionType.ToString().CompareTo(other.ConnectionType.ToString());
                else
                    if (this.TAG != other.TAG)
                        result = this.TAG.CompareTo(other.TAG);
                    else
                    {
                        switch (this.ConnectionType)
                        {
                            case ConnectionType.TDM:
                                result = int.Parse(this.Value).CompareTo(int.Parse(other.Value));
                                break;
                            default:
                                result = string.Compare(this.Value, other.Value, StringComparison.InvariantCultureIgnoreCase);
                                break;
                        }
                    }
            return result;
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;

namespace TABS
{
    [Serializable]
    public class SpecialRequest : Components.DateTimeEffectiveEntity, Interfaces.IZoneSupplied
    {
        public override string Identifier { get { return "SpecialRequest:" + SpecialRequestID; } }

        private int _SpecialRequestID;
        private CarrierAccount _Customer;
        private Zone _Zone;
        private string _Code;
        private CarrierAccount _Supplier;
        private byte? _Percentage;

        public virtual string Reason { get; set; }

        private SpecialRequestType _SpecialRequestType;



        public virtual int SpecialRequestID
        {
            get { return _SpecialRequestID; }
            set { _SpecialRequestID = value; }
        }

        public virtual CarrierAccount Customer
        {
            get { return _Customer; }
            set { _Customer = value; }
        }

        public virtual CarrierAccount Supplier
        {
            get { return _Supplier; }
            set { _Supplier = value; }
        }

        public virtual Zone Zone
        {
            get { return _Zone; }
            set { _Zone = value; }
        }

        public virtual string Code
        {
            get { return _Code; }
            set { _Code = value; }
        }

        protected string _IncludeSubCodes;
        public virtual bool IncludeSubCodes
        {
            get { return "Y".Equals(_IncludeSubCodes); }
            set { _IncludeSubCodes = value ? "Y" : "N"; }
        }


        public virtual RouteChangeHeader RouteChangeHeader { get; set; }

        public static IList<SpecialRequest> All
        {
            get
            {
                return DataConfiguration.CurrentSession
                 .CreateQuery(@"
                    FROM SpecialRequest S
                        WHERE ((S.BeginEffectiveDate < :when AND (S.EndEffectiveDate IS NULL OR S.EndEffectiveDate > :when)) OR S.BeginEffectiveDate > :when)")
                      .SetParameter("when", DateTime.Now)
                 .List<TABS.SpecialRequest>();
            }
        }

        public static IList<TABS.SpecialRequest> GetExistingSpecilaRequests(TABS.CarrierAccount customer, TABS.Zone zone, string code, DateTime when)
        {
            return TABS.DataConfiguration.CurrentSession
                          .CreateQuery(@"
                    FROM SpecialRequest S 
                        WHERE 
                                (S.Customer IS NULL OR S.Customer = :Customer) 
                            AND (S.Zone IS NULL OR S.Zone = :Zone) 
                            AND (S.Code IS NULL OR S.Code LIKE :CodeValue) 
                            AND ((S.BeginEffectiveDate < :when AND (S.EndEffectiveDate IS NULL OR S.EndEffectiveDate > :when)) OR (S.BeginEffectiveDate > :when AND S.EndEffectiveDate > S.BeginEffectiveDate) )")
                    .SetParameter("Customer", customer)
                    .SetParameter("Zone", zone)
                    .SetParameter("CodeValue", code == null ? "%%" : code)
                    .SetParameter("when", when)
                .List<TABS.SpecialRequest>();
        }

        public virtual Nullable<byte> Priority { get; set; }

        public virtual Nullable<byte> NumberOfTries { get; set; }
        public virtual string ExcludedCodes { get; set; }
        

        public virtual byte? Percentage { get { return _Percentage; } set { _Percentage = value; } }

        public virtual SpecialRequestType SpecialRequestType { get { return _SpecialRequestType; } set { _SpecialRequestType = value; } }


        public string MatchedZone
        {
            get
            {
                string zoneName = string.Empty;

                if (string.IsNullOrEmpty(this.Code)) return string.Empty;

                TABS.Code code_ = TABS.CodeMap.CurrentOurCodes.Find(this.Code, TABS.CarrierAccount.SYSTEM, DateTime.Now);
                if (code_ != null)
                    zoneName = code_.Zone.Name;
                return zoneName;
            }
        }

        public override string ToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("Special Request");
            if (Customer != null) sb.AppendFormat(", Customer: {0}", Customer);
            if (Zone != null) sb.AppendFormat(", Zone: {0}", Zone);
            if (Code != null) sb.AppendFormat(", Code: {0}", Code);
            if (Supplier != null) sb.AppendFormat(", Supplier: {0}", Supplier);
            //if (Priority != null) sb.AppendFormat(", Priority: {0}", Priority);
            //if (NumberOfTries != null) sb.AppendFormat(", NumberOfTries: {0}", NumberOfTries);
            //sb.AppendFormat(", Type: {0}", SpecialRequestType);
            return sb.ToString();
        }

        public override bool Equals(object obj)
        {
            SpecialRequest other = obj as SpecialRequest;
            if (other == null)
                return base.Equals(obj);
            else
            {
                if (this.SpecialRequestID == 0)
                {
                    return
                        (
                            (this.Customer == null || this.Customer.Equals(other.Zone))
                            &&
                            (this.Supplier == null || this.Supplier.Equals(other.Zone))
                            &&
                            (this.Zone == null || this.Zone.Equals(other.Zone))
                            &&
                            (this.Code == null || this.Code.Equals(other.Code))
                            &&
                            this.NumberOfTries.Equals(other.NumberOfTries)
                            &&
                            this.Priority.Equals(other.Priority)
                            &&
                            this.SpecialRequestType.Equals(other.SpecialRequestType)
                            &&
                            this.BeginEffectiveDate.Equals(other.BeginEffectiveDate)
                        );
                }
                else
                    return this.SpecialRequestID.Equals(other.SpecialRequestID);
            }
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

    }
}

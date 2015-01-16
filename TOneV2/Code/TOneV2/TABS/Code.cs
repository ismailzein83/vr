using System;
using System.Collections;
using System.Collections.Generic;

namespace TABS
{
    [Serializable]
    public class Code : Components.DateTimeEffectiveEntity, IComparable<Code>, Interfaces.IZoneSupplied
    {
        public override string Identifier { get { return "Code:" + ID; } }

        protected long _ID;
        protected string _Code;
        protected Zone _Zone;

        public virtual long ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        public virtual string Value
        {
            get { return _Code; }
            set { _Code = value; }
        }

        public virtual Zone Zone
        {
            get { return _Zone; }
            set { _Zone = value; }
        }

        public virtual CarrierAccount Supplier
        {
            get { return (Zone == null) ? null : Zone.Supplier; }
            set { }
        }

        public virtual CarrierAccount Customer
        {
            get { return (Zone == null) ? null : Zone.Supplier; }
            set { }
        }


        public override string ToString()
        {
            return Value;
        }
        public override int GetHashCode()
        {
            if (this.Value == null)
                return base.GetHashCode();
            else
                return this.Value.GetHashCode();
        }
        /// <summary>
        /// Get the code range (as text) for the specified enumerable
        /// </summary>
        /// <param name="codes">The codes to return as a range</param>
        /// <returns>A string representing the codes as a range: 
        /// a comma, dash-range seperated code list from the effective codes.
        /// Example: 96170, 9613, 9617-9619
        /// </returns>
        public static string GetCodeRange(IEnumerable<Code> codes)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            long previous = long.MinValue;
            bool insideRange = false;
            foreach (Code code in codes)
            {
                long current = 0;
                try
                {
                    current = long.Parse(code.Value);
                    if (current == (previous + 1) && code.EndEffectiveDate == null)
                    {
                        if (!insideRange)
                        {
                            insideRange = true;
                        }
                    }
                    else
                    {
                        if (insideRange)
                        {
                            sb.Append("-");
                            sb.Append(previous);
                            sb.Append(";");
                            sb.AppendFormat("{0}{1}", current, code.EndEffectiveDate.HasValue ? string.Format("*({0:yyyy-MM-dd})", code.EndEffectiveDate) : string.Empty);
                        }
                        else
                        {
                            if (sb.Length > 0) sb.Append(";");
                            sb.AppendFormat("{0}{1}", current, code.EndEffectiveDate.HasValue ? string.Format("*({0:yyyy-MM-dd})", code.EndEffectiveDate) : string.Empty);
                        }
                        insideRange = false;
                    }
                    previous = current;
                }
                catch
                {
                    if (sb.Length > 0) sb.Append(";");
                    sb.AppendFormat("{0}{1}", code.Value, code.EndEffectiveDate.HasValue ? string.Format("*({0:yyyy-MM-dd})", code.EndEffectiveDate) : string.Empty);
                    previous = long.MinValue;
                }
            }
            if (insideRange && previous != long.MinValue)
            {
                sb.Append("-");
                sb.Append(previous);
            }
            return sb.ToString();
        }

        public static string GetCodeRangeForRateSheet(IEnumerable<Code> codes)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            if (!TABS.SystemParameter.IncludeCodeRangesInCustomPricelist.BooleanValue.Value)
            {
                foreach (Code code in codes)
                    sb.AppendFormat("{0},", code.Value);

                return sb.ToString().TrimEnd(',');
            }


            long previous = long.MinValue;
            bool insideRange = false;
            foreach (Code code in codes)
            {
                long current = 0;
                try
                {
                    current = long.Parse(code.Value);
                    if (current == (previous + 1))
                    {
                        if (!insideRange)
                        {
                            insideRange = true;
                        }
                    }
                    else
                    {
                        if (insideRange)
                        {
                            sb.Append("-");
                            sb.Append(previous);
                            sb.Append(";");
                            sb.Append(current);
                        }
                        else
                        {
                            if (sb.Length > 0) sb.Append(";");
                            sb.Append(current);
                        }
                        insideRange = false;
                    }
                    previous = current;
                }
                catch
                {
                    if (sb.Length > 0) sb.Append(";");
                    sb.Append(code.Value);
                    previous = long.MinValue;
                }
            }
            if (insideRange && previous != long.MinValue)
            {
                sb.Append("-");
                sb.Append(previous);
            }
            return sb.ToString().TrimEnd(';');
        }

        public static string GetCodeRangeForRateSheet(IEnumerable<string> codes)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            //if (!TABS.SystemParameter.IncludeCodeRangesInCustomPricelist.BooleanValue.Value)
            //{
            //    foreach (var code in codes)
            //        sb.AppendFormat("{0},", code);

            //    return sb.ToString().TrimEnd(',');
            //}


            long previous = long.MinValue;
            bool insideRange = false;
            foreach (var code in codes)
            {
                long current = 0;
                try
                {
                    current = long.Parse(code);
                    if (current == (previous + 1))
                    {
                        if (!insideRange)
                        {
                            insideRange = true;
                        }
                    }
                    else
                    {
                        if (insideRange)
                        {
                            sb.Append("-");
                            sb.Append(previous);
                            sb.Append(";");
                            sb.Append(current);
                        }
                        else
                        {
                            if (sb.Length > 0) sb.Append(";");
                            sb.Append(current);
                        }
                        insideRange = false;
                    }
                    previous = current;
                }
                catch
                {
                    if (sb.Length > 0) sb.Append(";");
                    sb.Append(code);
                    previous = long.MinValue;
                }
            }
            if (insideRange && previous != long.MinValue)
            {
                sb.Append("-");
                sb.Append(previous);
            }
            return sb.ToString().TrimEnd(';');
        }

        #region Comparing Codes for sorting

        /// <summary>
        /// The Default code comparer that compares two codes
        /// using their "values".
        /// </summary>
        public class DefaultComparer : IComparer
        {
            public DefaultComparer()
            {

            }

            #region IComparer Members

            public int Compare(object x, object y)
            {
                Code first = x as Code;
                Code second = y as Code;
                if (first == null || second == null)
                    return 0;
                else
                    return first.Value.CompareTo(second.Value);
            }

            /// <summary>
            /// Sort a given List of codes
            /// </summary>
            /// <param name="codes">The list to sort.</param>
            public static void Sort(IList<Code> codes)
            {
                List<Code> sorted = new List<Code>(codes);
                DefaultComparer comparer = new DefaultComparer();
                sorted.Sort();
                codes.Clear();
                foreach (Code code in sorted)
                    codes.Add(code);
            }

            #endregion
        }

        #endregion

        #region IComparable<Code> Members

        public int CompareTo(Code other)
        {
            return (this._Code == null ? "" : this._Code).CompareTo(other._Code == null ? "" : other._Code);
        }

        public override bool Equals(object obj)
        {
            Code other = obj as Code;
            if (other != null)
            {
                if (this.ID > 0 && other.ID > 0) return this.ID.Equals(other.ID);
                else
                    return (
                        this.Supplier.Equals(other.Supplier)
                        && this.Zone.Equals(other.Zone)
                        && this.Value.Equals(other.Value)
                        && this.BeginEffectiveDate.Equals(other.BeginEffectiveDate)
                        );
            }
            else
                return false;
        }
        #endregion
    }
}

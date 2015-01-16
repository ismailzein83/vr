using System;
using System.Collections.Generic;

namespace TABS
{
    public class CDR : TABS.Addons.Utilities.Extensibility.CDR
    {
        public virtual long CDRID { get; set; }
        public virtual Switch Switch { get; set; }

        public CDR()
        {

        }

        public virtual void ClipFieldsToBounds()
        {
            if (this.CDPN != null && this.CDPN.Length > 40) this.CDPN = this.CDPN.Substring(0, 40);
            if (this.CDPNOut != null && this.CDPNOut.Length > 40) this.CDPNOut = this.CDPNOut.Substring(0, 40);
            if (this.CGPN != null && this.CGPN.Length > 40) this.CGPN = this.CGPN.Substring(0, 40);
            if (this.Extra_Fields != null)
            {
                if (this.Extra_Fields.Length > 8000) this.Extra_Fields = this.Extra_Fields.Substring(0, 8000);
                else if (this.Extra_Fields.Length == 0) this.Extra_Fields = null;
            }
        }

        public CDR(Switch sourceSwitch, TABS.Addons.Utilities.Extensibility.CDR cdr)
        {
            this.Switch = sourceSwitch;
            // Copy base properties
            base.IDonSwitch = cdr.IDonSwitch;
            base.Tag = cdr.Tag;
            base.AttemptDateTime = cdr.AttemptDateTime;
            base.AlertDateTime = cdr.AlertDateTime;
            base.ConnectDateTime = cdr.ConnectDateTime;
            base.DisconnectDateTime = cdr.DisconnectDateTime;
            base.Duration = cdr.Duration;
            base.IN_TRUNK = cdr.IN_TRUNK;
            base.IN_CIRCUIT = cdr.IN_CIRCUIT;
            base.IN_CARRIER = cdr.IN_CARRIER;
            base.IN_IP = cdr.IN_IP;
            base.OUT_TRUNK = cdr.OUT_TRUNK;
            base.OUT_CIRCUIT = cdr.OUT_CIRCUIT;
            base.OUT_CARRIER = cdr.OUT_CARRIER;
            base.OUT_IP = cdr.OUT_IP;
            base.CGPN = cdr.CGPN;
            base.CDPN = cdr.CDPN;
            base.CDPNOut = cdr.CDPNOut;
            base.CAUSE_FROM = cdr.CAUSE_FROM;
            base.CAUSE_FROM_RELEASE_CODE = cdr.CAUSE_FROM_RELEASE_CODE;
            base.CAUSE_TO = cdr.CAUSE_TO;
            base.CAUSE_TO_RELEASE_CODE = cdr.CAUSE_TO_RELEASE_CODE;
            base.Extra_Fields = cdr.Extra_Fields;
            base.IsRerouted = cdr.IsRerouted;
        }

        public class CdrEnumerator : IDisposable, IEnumerable<CDR>, IEnumerator<CDR>
        {
            /// <summary>
            /// The required and necessary fields in the required order to enumerate CDRs.
            /// </summary>
            public static readonly string[] ReaderFields =
            {
                "CDRID",
                "SwitchId",
                "IDonSwitch",
                "Tag",
                "AttemptDateTime",
                "AlertDateTime",
                "ConnectDateTime",
                "DisconnectDateTime",
                "DurationInSeconds",
                "IN_TRUNK",
                "IN_CIRCUIT",
                "IN_CARRIER",
                "IN_IP",
                "OUT_TRUNK",
                "OUT_CIRCUIT", 
                "OUT_CARRIER",
                "OUT_IP",
                "CGPN",
                "CDPN",
                "CDPNOut",
                "CAUSE_FROM",
                "CAUSE_FROM_RELEASE_CODE",
                "CAUSE_TO",
                "CAUSE_TO_RELEASE_CODE",
                "Extra_Fields",
                "IsRerouted",
                "CDPNOut"
            };

            System.Data.IDataReader reader;
            CDR current;

            public CdrEnumerator(System.Data.IDbCommand readerCommand)
            {
                reader = readerCommand.ExecuteReader();
            }

            public CdrEnumerator(System.Data.IDataReader cdrReader)
            {
                this.reader = cdrReader;
            }

            public void Dispose()
            {
                try
                {
                    reader.Close();
                }
                catch
                {

                }
                finally
                {
                    reader.Dispose();
                }
            }

            public IEnumerator<CDR> GetEnumerator()
            {
                return this;
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return this;
            }

            public CDR Current
            {
                get { return current; }
            }

            object System.Collections.IEnumerator.Current
            {
                get { return this; }
            }

            public bool MoveNext()
            {
                bool success = reader.Read();
                if (success)
                {
                    this.current = new CDR();
                    int index = -1;
                    index++; current.CDRID = (long)reader[index];
                    index++; current.Switch = Switch.All[(byte)reader[index]];
                    index++; current.IDonSwitch = (long)reader[index];
                    index++; current.Tag = reader.IsDBNull(index) ? null : (string)reader[index];
                    index++; current.AttemptDateTime = reader.GetDateTime(index);
                    index++; current.AlertDateTime = reader.IsDBNull(index) ? null : (DateTime?)reader[index];
                    index++; current.ConnectDateTime = reader.IsDBNull(index) ? null : (DateTime?)reader[index];
                    index++; current.DisconnectDateTime = reader.IsDBNull(index) ? null : (DateTime?)reader[index];
                    index++; current.DurationInSeconds = reader.GetDecimal(index);
                    index++; current.IN_TRUNK = reader.IsDBNull(index) ? null : (string)reader[index];
                    index++; current.IN_CIRCUIT = reader.IsDBNull(index) ? 0 : short.Parse(reader[index].ToString());
                    index++; current.IN_CARRIER = reader.IsDBNull(index) ? null : (string)reader[index];
                    index++; current.IN_IP = reader.IsDBNull(index) ? null : (string)reader[index];
                    index++; current.OUT_TRUNK = reader.IsDBNull(index) ? null : (string)reader[index];
                    index++; current.OUT_CIRCUIT = reader.IsDBNull(index) ? 0 : short.Parse(reader[index].ToString());
                    index++; current.OUT_CARRIER = reader.IsDBNull(index) ? null : (string)reader[index];
                    index++; current.OUT_IP = reader.IsDBNull(index) ? null : (string)reader[index];
                    index++; current.CGPN = reader.IsDBNull(index) ? null : (string)reader[index];
                    index++; current.CDPN = reader.IsDBNull(index) ? null : (string)reader[index];
                    index++; current.CDPNOut = reader.IsDBNull(index) ? null : (string)reader[index];
                    index++; current.CAUSE_FROM = reader.IsDBNull(index) ? null : (string)reader[index];
                    index++; current.CAUSE_FROM_RELEASE_CODE = reader.IsDBNull(index) ? null : (string)reader[index];
                    index++; current.CAUSE_TO = reader.IsDBNull(index) ? null : (string)reader[index];
                    index++; current.CAUSE_TO_RELEASE_CODE = reader.IsDBNull(index) ? null : (string)reader[index];
                    index++; current.Extra_Fields = reader.IsDBNull(index) ? null : (string)reader[index];
                    index++; current.IsRerouted = reader.IsDBNull(index) ? false : "Y".Equals(reader[index]);
                    index++; current.CDPNOut = reader.IsDBNull(index) ? null : (string)reader[index];
                }
                return success;
            }

            public void Reset()
            {
                throw new NotSupportedException("CDR Enumerator Does not Support Resetting");
            }
        }

        public override string ToString()
        {
            return string.Concat(this.Switch.SwitchID, sep, base.ToString());
        }

        public static IEnumerable<CDR> Get(System.Data.IDbCommand readerCommand)
        {
            return new CdrEnumerator(readerCommand);
        }

        public static IEnumerable<CDR> Get(System.Data.IDataReader cdrReader)
        {
            return new CdrEnumerator(cdrReader);
        }

    }
}

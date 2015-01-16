using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;

namespace TABS.Addons.CDRStores
{
    public class CdrReaderEnumerator : IEnumerator<CDR>, IEnumerable<CDR>
    {
        CDR _current;
        IDataReader _reader;

        public CdrReaderEnumerator(IDataReader reader)
        {
            _reader = reader;
        }

        #region IEnumerator<CDR> Members

        public CDR Current
        {
            get { return _current; }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            _reader.Dispose();
        }

        #endregion

        #region IEnumerator Members

        object System.Collections.IEnumerator.Current
        {
            get { return _current; }
        }

        public bool MoveNext()
        {
            // If nothing more to read, we have to return false and set current to null
            if (!_reader.Read())
            {
                _current = null;
                return false;                
            }

            CDR cdr = new CDR();

            int index = -1;
            
            cdr.Switch = Switch.All[_reader.GetByte(++index)];
            cdr.IDonSwitch = _reader.GetInt64(++index);
            cdr.Tag = _reader[++index] == DBNull.Value ? null : (string)_reader[index];
            cdr.AttemptDateTime = _reader.GetDateTime(++index);
            cdr.AlertDateTime = _reader[++index] == DBNull.Value ? null : (DateTime?)_reader[index];
            cdr.ConnectDateTime = _reader[++index] == DBNull.Value ? null : (DateTime?)_reader[index];
            cdr.DisconnectDateTime = _reader[++index] == DBNull.Value ? null : (DateTime?)_reader[index];
            cdr.DurationInSeconds = _reader.GetDecimal(++index);
            cdr.IN_TRUNK = _reader[++index] == DBNull.Value ? null : (string)_reader[index];
            cdr.IN_CIRCUIT = _reader.GetInt16(++index);
            cdr.IN_CARRIER = _reader[++index] == DBNull.Value ? null : (string)_reader[index];
            cdr.IN_IP = _reader[++index] == DBNull.Value ? null : (string)_reader[index];
            cdr.OUT_TRUNK = _reader[++index] == DBNull.Value ? null : (string)_reader[index];
            cdr.OUT_CIRCUIT = _reader.GetInt16(++index);
            cdr.OUT_CARRIER = _reader[++index] == DBNull.Value ? null : (string)_reader[index];
            cdr.OUT_IP = _reader[++index] == DBNull.Value ? null : (string)_reader[index];
            cdr.CGPN = _reader[++index] == DBNull.Value ? null : (string)_reader[index];
            cdr.CDPN = _reader[++index] == DBNull.Value ? null : (string)_reader[index];
            cdr.CAUSE_FROM_RELEASE_CODE = _reader[++index] == DBNull.Value ? null : (string)_reader[index];
            cdr.CAUSE_FROM = _reader[++index] == DBNull.Value ? null : (string)_reader[index];
            cdr.CAUSE_TO_RELEASE_CODE = _reader[++index] == DBNull.Value ? null : (string)_reader[index];
            cdr.CAUSE_TO = _reader[++index] == DBNull.Value ? null : (string)_reader[index];

            _current = cdr;
            
            return true;
        }


        public void Reset()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable<CDR> Members

        public IEnumerator<CDR> GetEnumerator()
        {
            return this;
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this;
        }

        #endregion
    }
}

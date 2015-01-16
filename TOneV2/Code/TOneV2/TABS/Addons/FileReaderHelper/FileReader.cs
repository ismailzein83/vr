using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace TABS.Addons.FileReaderHelper
{
    public abstract class FileReader : IDataReader
    {
        private StreamReader _streamReader;
        protected IDataRecord _currentDataRecord;
        protected List<string> _FieldNames;
        private int _recordsAffected = -1;

        public List<string> FieldNames { get { return _FieldNames; } }

        protected FileReader(StreamReader reader)
        {
            _FieldNames = new List<string>();
            _streamReader = reader;
            _currentDataRecord = null;
        }

        public abstract IDataRecord GetNextDataRecord();

        public System.Type CurrentRecordType
        {
            get
            {
                return _currentDataRecord.GetType();
            }
        }

        protected StreamReader StreamReader
        {
            get
            {
                return _streamReader;
            }
        }

        #region IDataReader Members

        public int RecordsAffected
        {
            get
            {
                return _recordsAffected;
            }
        }

        public bool IsClosed
        {
            get
            {
                return _streamReader == null;
            }
        }


        public bool NextResult()
        {
            return false;
        }

        public void Close()
        {
            _currentDataRecord = null;
            if (_streamReader != null)
                _streamReader.Close();
        }

        public bool Read()
        {
            if (_streamReader == null)
                return false;

            _currentDataRecord = this.GetNextDataRecord();

            if (_currentDataRecord != null)
            {
                if (_recordsAffected == -1)
                    _recordsAffected = 1;
                else
                    _recordsAffected++;

                return true;
            }
            else
            {
                return false;
            }
        }

        public int Depth
        {
            get
            {
                return 0;
            }
        }

        public DataTable GetSchemaTable()
        {
            return null;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            this.Close();
            if (_streamReader != null)
                _streamReader.Dispose();
        }

        #endregion

        #region IDataRecord Members

        public int GetInt32(int i)
        {
            return _currentDataRecord.GetInt32(i);
        }

        public virtual object this[string name]
        {
            get
            {
                return _currentDataRecord[name];
            }
        }

        public virtual object this[int i]
        {
            get
            {
                return _currentDataRecord[i];
            }
        }

        public object GetValue(int i)
        {
            return _currentDataRecord.GetValue(i);
        }

        public bool IsDBNull(int i)
        {
            return _currentDataRecord.IsDBNull(i);
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            return _currentDataRecord.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
        }

        public byte GetByte(int i)
        {
            return _currentDataRecord.GetByte(i);
        }

        public Type GetFieldType(int i)
        {
            return _currentDataRecord.GetFieldType(i);
        }

        public decimal GetDecimal(int i)
        {
            return _currentDataRecord.GetDecimal(i);
        }

        public int GetValues(object[] values)
        {
            return _currentDataRecord.GetValues(values);
        }

        public string GetName(int i)
        {
            return _FieldNames[i];
        }

        public int FieldCount
        {
            get
            {
                return _currentDataRecord.FieldCount;
            }
        }

        public long GetInt64(int i)
        {
            return _currentDataRecord.GetInt64(i);
        }

        public double GetDouble(int i)
        {
            return _currentDataRecord.GetDouble(i);
        }

        public bool GetBoolean(int i)
        {
            return _currentDataRecord.GetBoolean(i);
        }

        public Guid GetGuid(int i)
        {
            return _currentDataRecord.GetGuid(i);
        }

        public DateTime GetDateTime(int i)
        {
            return _currentDataRecord.GetDateTime(i);
        }

        public int GetOrdinal(string name)
        {
            return _currentDataRecord.GetOrdinal(name);
        }

        public string GetDataTypeName(int i)
        {
            return _currentDataRecord.GetDataTypeName(i);
        }

        public float GetFloat(int i)
        {
            return _currentDataRecord.GetFloat(i);
        }

        public IDataReader GetData(int i)
        {
            return _currentDataRecord.GetData(i);
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            return _currentDataRecord.GetChars(i, fieldoffset, buffer, bufferoffset, length);
        }

        public string GetString(int i)
        {
            return _currentDataRecord.GetString(i);
        }

        public char GetChar(int i)
        {
            return _currentDataRecord.GetChar(i);
        }

        public short GetInt16(int i)
        {
            return _currentDataRecord.GetInt16(i);
        }
        #endregion IDataRecord Members

    }
}

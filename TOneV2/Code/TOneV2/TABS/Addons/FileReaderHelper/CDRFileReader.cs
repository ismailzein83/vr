using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Data;
using System.IO;

namespace TABS.Addons.FileReaderHelper
{
    public class CDRFileReader : FileReader
    {
        StreamReader _streamReader;
        public CDRFileReader(StreamReader streamReader)
            : base(streamReader)
        {
            _streamReader = streamReader;
        }
        /// <summary>
        /// regular expression to extract field name and field value of a specific cdr line 
        /// </summary>
        protected static Regex CdrParser = new Regex(@"(^|,)\s*(?<HEAD>[^=,]+)\s*=\s*(?<VALUE>[^,]*)(?=,|$)",
                                                System.Text.RegularExpressions.RegexOptions.Compiled
                                                | System.Text.RegularExpressions.RegexOptions.Singleline
                                                | System.Text.RegularExpressions.RegexOptions.ExplicitCapture);

        Dictionary<string, string> fields { get; set; }

        public override IDataRecord GetNextDataRecord()
        {
            string cdrLine = StreamReader.ReadLine();
            if (cdrLine == null) return null;
            _currentDataRecord = new FileDataRecord(this, cdrLine);
            return _currentDataRecord;
        }


        public class FileDataRecord : IDataRecord
        {
            Dictionary<int, string> _FieldValues;
            FileReader _FileReader;

            public FileDataRecord(FileReader reader, string cdrLine)
            {
                _FileReader = reader;
                _FieldValues = new Dictionary<int, string>();

                string fieldName, fieldValue;
                foreach (Match match in CdrParser.Matches(cdrLine))
                {
                    fieldName = match.Groups["HEAD"].Value.ToUpper();
                    fieldValue = match.Groups["VALUE"].Value;

                    bool fieldFound = false;
                    for (int i = 0; i < reader.FieldNames.Count; i++)
                    {
                        if (_FileReader.FieldNames[i].Equals(fieldName))
                        {
                            _FieldValues[i] = fieldValue;
                            fieldFound = true;
                            break;
                        }
                    }
                    if (!fieldFound)
                    {
                        _FileReader.FieldNames.Add(fieldName);
                        _FieldValues[_FileReader.FieldNames.Count - 1] = fieldValue;
                    }
                }
            }

            #region IDataRecord Members

            public int GetInt32(int i)
            {
                return Convert.ToInt32(_FieldValues[i]);
            }

            public object this[string name]
            {
                get
                {
                    int i = 0;
                    foreach (string fieldName in _FileReader.FieldNames)
                    {
                        if (fieldName.Equals(name))
                            return _FieldValues[i];
                        i++;
                    }
                    return DBNull.Value;
                }
            }

            object System.Data.IDataRecord.this[int i]
            {
                get
                {
                    return GetValue(i);
                }
            }

            public object GetValue(int i)
            {
                if (_FieldValues.ContainsKey(i))
                    return _FieldValues[i];
                else
                    return DBNull.Value;
            }

            public bool IsDBNull(int i)
            {
                return _FieldValues.ContainsKey(i);
            }

            public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
            {
                throw new NotImplementedException();
            }

            public byte GetByte(int i)
            {
                return Convert.ToByte(_FieldValues[i]);
            }

            public Type GetFieldType(int i)
            {
                return typeof(string);
            }

            public decimal GetDecimal(int i)
            {
                return Convert.ToDecimal(_FieldValues[i]);
            }

            public int GetValues(object[] values)
            {
                for (int i = 0; i < values.Length; i++)
                    values[i] = GetValue(i);
                return _FieldValues.Values.Count;
            }

            public string GetName(int i)
            {
                return _FileReader.GetName(i);
            }

            public int FieldCount
            {
                get
                {
                    return _FieldValues.Count;
                }
            }

            public long GetInt64(int i)
            {
                return Convert.ToInt64(_FieldValues[i]);
            }

            public double GetDouble(int i)
            {
                return Convert.ToDouble(_FieldValues[i]);
            }

            public bool GetBoolean(int i)
            {
                return Convert.ToBoolean(_FieldValues[i]);
            }

            public Guid GetGuid(int i)
            {
                return new Guid(_FieldValues[i]);
            }

            public DateTime GetDateTime(int i)
            {
                return Convert.ToDateTime(_FieldValues[i]);
            }

            public int GetOrdinal(string name)
            {
                int i = 0;
                foreach (string fieldName in _FileReader.FieldNames)
                {
                    if (fieldName == name)
                        return i;
                    i++;
                }
                return -1;
            }

            public string GetDataTypeName(int i)
            {
                throw new NotImplementedException();
            }

            public float GetFloat(int i)
            {
                return Convert.ToSingle(_FieldValues[i]);
            }

            public IDataReader GetData(int i)
            {
                throw new NotImplementedException();
            }

            public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
            {
                throw new NotImplementedException();
            }

            public string GetString(int i)
            {
                return (string)GetValue(i);
            }

            public char GetChar(int i)
            {
                return Convert.ToChar(_FieldValues[i]);
            }

            public short GetInt16(int i)
            {
                return Convert.ToInt16(_FieldValues[i]);
            }

            #endregion
        }
    }
}
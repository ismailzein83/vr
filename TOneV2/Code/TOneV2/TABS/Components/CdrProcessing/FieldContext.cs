using System;
using System.Collections.Generic;

namespace TABS.Components.CdrProcessing
{
    public class FieldContext
    {
        
        public Dictionary<string, FieldRecord> FieldRecords { get; set; }
        public System.IO.BinaryWriter BinaryWriter { get; set; }
        public System.IO.BinaryReader BinaryReader { get; set; }
        int _MaxWrittenLength = 0;
        public int MaxWrittenLength { get { return _MaxWrittenLength; } }

        internal System.IO.MemoryStream WorkerStream { set; get; }

        public FieldContext(Dictionary<string, FieldRecord> fieldRecords)
        {
            WorkerStream = new System.IO.MemoryStream(255); // a default of 255 bytes is more than ok
            BinaryWriter = new System.IO.BinaryWriter(WorkerStream);
            BinaryReader = new System.IO.BinaryReader(WorkerStream);
            FieldRecords = fieldRecords;
        }
        
        /// <summary>
        /// 
        /// </summary>
        public FieldContext()
            : this(new Dictionary<string, FieldRecord>())
        {
        }

        public long Write(System.IO.Stream targetStream, System.Data.IDataReader reader)
        {
            long count = 0;            
            while (reader.Read())
            {
                byte fields = 0;
                WorkerStream.SetLength(0);
                for (int i = 0; i < reader.FieldCount; i++)
                    if (!reader.IsDBNull(i))
                    {
                        string fieldName = reader.GetName(i);
                        FieldRecord fieldRecord = null;
                        if (!FieldRecords.TryGetValue(fieldName, out fieldRecord))
                        {
                            fieldRecord = new FieldRecord(reader, i);
                            fieldRecord.Index = (byte)FieldRecords.Count;
                            FieldRecords[fieldName] = fieldRecord; 
                        }
                        fieldRecord.Write(this, reader[i]);
                        fields++;
                    }
                if (fields > 0)
                {
                    targetStream.WriteByte(fields);
                    int length = (int)WorkerStream.Length;
                    targetStream.Write(WorkerStream.ToArray(), 0, length);
                    length++;
                    if (_MaxWrittenLength < length) _MaxWrittenLength = length;
                }
                count++;
            }
            return count;
        }

        public DynamicCDR Read(System.IO.Stream sourceStream)
        {
            if (FieldRecords == null || FieldRecords.Count == 0) 
                throw new Exception("Cannot Read CDRs with no Field Records Defined");

            BinaryReader = new System.IO.BinaryReader(sourceStream);

            DynamicCDR cdr = null;
            int fields = sourceStream.ReadByte();
            if (fields > 0)
            {
                cdr = new DynamicCDR();
                while (fields > 0)
                {
                    // Read the field index
                    byte index = (byte)sourceStream.ReadByte();
                    bool fieldFound = false;
                    foreach (FieldRecord field in FieldRecords.Values)
                    {
                        if (field.Index == index)
                        {
                            cdr[field] = field.Read(this, sourceStream);
                            fieldFound = true;
                            break;
                        }
                    }
                    if (!fieldFound) 
                        throw new Exception(string.Format("CDR has field #{0} which is not defined", index));
                    fields--;
                }
                return cdr;
            }
            else
                return null;
        }

    }
}

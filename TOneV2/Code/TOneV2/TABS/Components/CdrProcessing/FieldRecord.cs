using System;

namespace TABS.Components.CdrProcessing
{
    [Serializable]
    public class FieldRecord
    {
        byte _Index;
        string _Name;
        StoredType _StoredType;
        string _OriginalTypeName;

        public byte Index { get { return _Index; } set { _Index = value; } }
        public string Name { get { return _Name; } set { _Name = value; } }
        public StoredType StoredType { get { return _StoredType; } set { _StoredType = value; } }
        public string OriginalTypeName { get { return _OriginalTypeName; } set { _OriginalTypeName = value; } }

        public FieldRecord(System.Data.IDataReader reader, int index)
        {
            // Name and index
            this.Name = reader.GetName(index);
            
            // Type
            Type fieldType = reader.GetFieldType(index);
            this.OriginalTypeName = fieldType.FullName;            
            switch (fieldType.Name.ToLower())
            {
                case "byte":
                case "char":
                case "int16":
                case "int32":
                case "ubyte":
                case "uint16":
                case "uint32":                    
                    StoredType = StoredType.Integer;
                    break;
                case "int64":
                case "uint64":
                case "long":
                case "float":
                case "single":
                case "double":
                case "decimal":
                    StoredType = StoredType.Number;
                    break;
                case "datetime":
                    StoredType = StoredType.DateTime;
                    break;
                default:
                    StoredType = StoredType.Text;
                    break;
            }
        }

        public void Write(FieldContext context, object value)
        {
            context.BinaryWriter.Write(this.Index);
            switch (StoredType)
            {
                // Integer. int is the standard
                case StoredType.Integer:
                    context.BinaryWriter.Write(int.Parse(value.ToString()));
                    break;

                // Numeric. Double is our standard value since we won't need more precision
                case StoredType.Number:
                    context.BinaryWriter.Write(Double.Parse(value.ToString()));
                    break;

                // 8-bytes Datetime
                case StoredType.DateTime:
                    context.BinaryWriter.Write(((DateTime)value).ToBinary());
                    break;                

                // Zero-Trailed Text
                default:
                    byte [] bytes = System.Text.Encoding.UTF8.GetBytes(value.ToString());
                    context.BinaryWriter.Write(bytes);
                    context.BinaryWriter.Write((byte)0);
                    break;
            }
        }

        public object Read(FieldContext context, System.IO.Stream stream)
        {
            switch (StoredType)
            {
                // 4-bytes integer
                case StoredType.Integer:
                    return context.BinaryReader.ReadInt32();

                // 8-bytes Datetime
                case StoredType.DateTime:
                    return DateTime.FromBinary(context.BinaryReader.ReadInt64());

                // Numeric. Double is our standard value since we won't need more precision
                case StoredType.Number:
                    return context.BinaryReader.ReadDouble();

                // Zero-Trailed Text
                default:
                    context.WorkerStream.SetLength(0);
                    int read = 0;
                    while ((read = stream.ReadByte()) > 0)
                        context.WorkerStream.WriteByte((byte)read);
                    if (context.WorkerStream.Length > 0)
                        return System.Text.Encoding.UTF8.GetString(context.WorkerStream.ToArray());
                    else
                        return "";
            }
        }
    }
}
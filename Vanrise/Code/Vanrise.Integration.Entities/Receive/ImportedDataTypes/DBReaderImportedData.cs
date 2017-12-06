using System;
using System.Data.Common;
using System.Text;

namespace Vanrise.Integration.Entities
{
    public class DBReaderImportedData : IImportedData
    {
        public DbDataReader Reader { get; set; }

        public Object FirstImportedId { get; set; }

        public Object LastImportedId { get; set; }

        public string Description
        {
            get
            {
                StringBuilder descriptionBuilder = new StringBuilder();
                if (this.FirstImportedId != null)
                    descriptionBuilder.AppendFormat("First Id '{0}'", this.FirstImportedId);
                if (this.LastImportedId != null)
                {
                    if (descriptionBuilder.Length > 0)
                        descriptionBuilder.Append(" ");
                    descriptionBuilder.AppendFormat("Last Id '{0}'", this.LastImportedId);
                }
                return descriptionBuilder.ToString();
            }
        }

        public Object MapperStateObj { get; set; }

        public long? BatchSize
        {
            get { return null; }
        }


        public void OnDisposed()
        {
            if (this.Reader != null && !Reader.IsClosed)
            {
                Reader.Close();
                Reader.Dispose();
            }
        }


        public bool IsMultipleReadings
        {
            get { return true; }
        }

        public bool IsEmpty
        {
            get;
            set;
        }
        public bool IsFile
        {
            get
            {
                return false;
            }
        }
    }
}

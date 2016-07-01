using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Integration.Entities
{
    public class DBReaderImportedData : IImportedData
    {
        public DbDataReader Reader { get; set; }

        public Object LastImportedId { get; set; }

        public string Description
        {
            get { return null; }
        }

        public Object MapperStateObj { get; set; }

        public long? BatchSize
        {
            get { return null ; }
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
    }
}

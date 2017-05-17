﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Integration.Entities
{
    public class StreamReaderImportedData : IImportedData
    {
        public Stream Stream { get; set; }

        public DateTime Accessed { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        public string Name { get; set; }

        public long Size { get; set; }

        public object Type { get; set; }

        StreamReader _streamReader;
        public StreamReader StreamReader
        {
            get
            {
                if (_streamReader == null)
                    _streamReader = new StreamReader(this.Stream);
                return _streamReader;
            }
        }

        public string Description
        {
            get { return null; }
        }


        public long? BatchSize
        {
            get { return Size; }
        }


        public void OnDisposed()
        {
            if(this.StreamReader != null)
            {
                this.StreamReader.Close();
                this.StreamReader.Dispose();
            }
        }


        public bool IsMultipleReadings
        {
            get { return false; }
        }

        public bool IsEmpty
        {
            get;
            set;
        }
    }
}

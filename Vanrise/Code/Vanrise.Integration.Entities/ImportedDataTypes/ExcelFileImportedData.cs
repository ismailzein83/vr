﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Integration.Entities
{
    public class ExcelFileImportedData : IImportedData
    {
        public byte[] Content { get; set; }

        public long Size { get; set; }

        public string Description
        {
            get { return null; }
        }


        public long? BatchSize
        {
            get { return this.Size; }
        }


        public void OnDisposed()
        {
            
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

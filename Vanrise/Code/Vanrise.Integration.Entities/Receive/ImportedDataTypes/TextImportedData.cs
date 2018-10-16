﻿using System;

namespace Vanrise.Integration.Entities
{
    public class TextFileImportedData : IImportedData
    {
        public string FileName { get; set; }

        public string Content { get; set; }

        public long Size { get; set; }

        public string Description { get { return FileName; } }

        public long? BatchSize { get { return this.Size; } }

        public bool IsEmpty { get; set; }

        public bool IsFile { get { return true; } }

        public bool IsMultipleReadings { get { return false; } }

        public void OnDisposed()
        {

        }
    }
}
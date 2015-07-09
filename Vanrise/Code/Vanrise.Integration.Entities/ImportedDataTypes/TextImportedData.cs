﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Integration.Entities
{
    public class TextFileImportedData : IImportedData
    {
        public string FileName { get; set; }
        public string Content { get; set; }

        public string Description
        {
            get { return FileName; }
        }
    }
}

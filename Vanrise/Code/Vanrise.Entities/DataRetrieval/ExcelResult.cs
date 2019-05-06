﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class ExcelResult
    {
        public MemoryStream ExcelFileStream { get; set; }

        public byte[] ExcelFileContent { get; set; }

        public ExcelConversionResultType? ConversionResultType { get; set; }

    }


    public enum ExcelConversionResultType
    {
        [Description("The sheet contain cell(s) includes data having more than 32k.")]
        InvalidContentLenght = 1
    }

    public class ExcelResult<T> : ExcelResult, IDataRetrievalResult<T>
    {
    }


}

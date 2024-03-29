﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDRComparison.Entities
{
    public interface IReadFromFileContext
    {
        byte[] FileContent { get; }

        bool TryReadLine(out string line);

        bool IsCompressed { get; set; }
    }
}

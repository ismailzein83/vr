﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOM.Main.Entities
{
    public class CreateSOMRequestOutput
    {
        public Guid SOMRequestId { get; set; }

        public long SOMProcessInstanceId { get; set; }
    }
}

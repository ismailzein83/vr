﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.Sales.BP.Arguments.Tasks
{
	public class EmailTaskExecutionInformation : BPTaskExecutionInformation
	{
		public IEnumerable<int> CustomerIds { get; set; }

        public bool Decision { get; set; }
	}
}

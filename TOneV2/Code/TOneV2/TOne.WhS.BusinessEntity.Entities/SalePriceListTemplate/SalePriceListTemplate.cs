﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
	public class SalePriceListTemplate
	{
		public int SalePriceListTemplateId { get; set; }

		public string Name { get; set; }

		public SalePriceListTemplateSettings Settings { get; set; }
	}
}

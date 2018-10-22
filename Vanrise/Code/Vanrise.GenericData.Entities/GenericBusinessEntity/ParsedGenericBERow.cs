using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
	public class ParsedGenericBERow
	{
		public int RowIndex { get; set; }
		public Dictionary<string, object> ColumnValueByFieldName { get; set; }
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class GenericLKUPItem
    {
        public Guid GenericLKUPItemId { get; set; }
        public string Name { get; set; }
        public Guid BusinessEntityDefinitionId { get; set; }
        public GenericLKUPItemSettings Settings { get; set; }
    }

    public class GenericLKUPItemSettings
    {
        public GenericLKUPItemExtendedSettings ExtendedSettings { get; set; }
    }

    public abstract class GenericLKUPItemExtendedSettings
    {

    }
}

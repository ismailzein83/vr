using Retail.Teles.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Teles.Business
{
    public class TelesUserFilterContext : ITelesUserFilterContext
    {
        public string UserId { get; set; }
        public Guid AccountBEDefinitionId { get; set; }
    }
}

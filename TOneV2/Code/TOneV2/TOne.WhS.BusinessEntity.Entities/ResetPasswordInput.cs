using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class ResetPasswordInput
    {
        public int CarrierProfileId { get; set; }
        public string Password { get; set; }
        public int UserId { get; set; }
    }
}

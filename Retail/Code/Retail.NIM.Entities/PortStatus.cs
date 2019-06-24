using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.NIM.Entities
{
    public static class PortStatus
    {
        public static Guid Free = new Guid("d65f6900-7e1f-479d-be8c-c5ecbc45c7c5");

        public static Guid TemporaryReserved = new Guid("265b40e8-5621-4a94-83bc-9fd8713cb9e9");

        public static Guid Used = new Guid("e648730c-4a0c-4354-8c4e-5e0d8c34f855");
    }
}
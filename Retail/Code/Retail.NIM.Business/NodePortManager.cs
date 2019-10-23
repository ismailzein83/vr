using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.NIM.Business
{
    public  class NodePortManager
    {
        static Guid _connectionDefinitionId = new Guid("e74d9d5d-cc59-4b40-8626-048a755c054c");
        public ReserveConnectionOutput ReserveConnection(ReserveConnectionInput input)
        {
            return null;
        }

        public ReservePortOutput ReservePort(ReservePortInput input)
        {
            return null;
        }
        public ReservePortOutput ReservePort(int portId)
        {
            return null;
        }
    }
    public class ReserveConnectionOutput
    {
        public ReservePortOutput Port1 { get; set; }
        public ReservePortOutput Port2 { get; set; }
    }
    public class ReserveConnectionInput
    {
        public Guid ConnectionTypeId { get; set; }
        public long NodeId { get; set; }
        public Guid? PartTypeId { get; set; }
    }
    public class ReservePortInput
    {
        public long NodeId { get; set; }
        public Guid? PartTypeId { get; set; }
    }
    public class ReservePortOutput
    {
        public long NodeId { get; set; }
        public string Number { get; set; }
    }
}

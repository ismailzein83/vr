using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions
{
    public class SwitchDBReplicationPreInsert : DBReplicationPreInsert
    {
        public override Guid ConfigId { get { return new Guid("5f22d483-6913-4943-bbbb-804559bef664"); } }
        public override void Execute(IDBReplicationPreInsertExecuteContext context)
        {

        }
    }
}

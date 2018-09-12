using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess.MainExtensions.BPInstance
{
    //Obsolete: Only for Testing
    public class TestingBPInstanceBeforeInsertHandler : BPInstanceInsertHandler
    {
        public override Guid ConfigId { get { return new Guid("371F8454-DAF0-4001-86FE-057B6BCDDA31"); } }

        public override void ExecuteBeforeInsert(IBPInstanceHandlerBeforeExecuteInsertContext context)
        {
            context.StartProcessOutput.BeforeInsertId = 1;
        }

        public override void ExecuteAfterInsert(IBPInstanceHandlerAfterExecuteInsertContext context)
        {
            context.StartProcessOutput.AfterInsertId = 2;
        }

        public override void BeforeAPICompile(IBPInstanceHandlerBeforeAPICompileContext context)
        {
            string outputArgumentCode = @"
                public int BeforeInsertId { get; set; }

                public int AfterInsertId { get; set; }
            ";

            context.OutputArgumentCode = outputArgumentCode;
        }
    }
}
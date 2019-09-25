using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkProvision.Entities
{
    public class GenerateCommandHandlerCustomCode : GenerateCommandHandler
    {
        public override Guid ConfigId
        {
            get { return new Guid("6B4B63CC-5B1C-4012-BB2C-0A5A51E287B9"); }
        }

        public string MethodLogic { get; set; }

        public string ClassMembers { get; set; }

        public override string Execute(IGenerateCommandHandlerExecuteContect context)
        {
            throw new NotImplementedException();
        }

        public override string GetCode(IGenerateCommandHandlerGetCodeContect context)
        {
            throw new NotImplementedException();
        }
    }
}

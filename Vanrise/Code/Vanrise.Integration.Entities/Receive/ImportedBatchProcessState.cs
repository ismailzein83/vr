using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Integration.Entities.Receive
{
    public class ImportedBatchProcessState : ProcessState
    {
        public override Guid Id { get; }

    }
}

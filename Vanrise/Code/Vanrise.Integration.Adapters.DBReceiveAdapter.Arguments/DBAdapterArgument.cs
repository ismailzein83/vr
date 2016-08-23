using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.Adapters.DBReceiveAdapter.Arguments
{
    public class DBAdapterArgument : BaseAdapterArgument
    {
        public string ConnectionString { get; set; }

        public string Query { get; set; }

        public string IdentifierColumnName { get; set; }

        public int NumberOfParallelReader { get; set; }

        public int? NumberOffSet { get; set; }

        public TimeSpan? TimeOffset { get; set; }

        public override int MaxParallelRuntimeInstances
        {
            get
            {
                return this.NumberOfParallelReader;
            }
        }

        public int CommandTimeoutInSeconds { get; set; }
    }
}

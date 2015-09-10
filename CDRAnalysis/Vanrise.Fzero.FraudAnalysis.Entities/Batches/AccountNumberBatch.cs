using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Queueing.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class AccountNumberBatch
    {
        public List<String> AccountNumbers;

        public string AccountNumberBatchFilePath { get; set; }
    }
}

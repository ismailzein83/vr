using System;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{

    public class AccountStatusDetail
    {
        public AccountStatus Entity { get; set; }

        public string StatusName { get; set; }

        public string UserName { get; set; }

        public string SourceName { get; set; }
    }
}

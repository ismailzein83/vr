using QM.CLITester.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.CLITester.Business
{
    public class InitiateTestContext : IInitiateTestContext
    {
        public QM.BusinessEntity.Entities.Supplier Supplier
        {
            get;
            set;
        }

        public QM.BusinessEntity.Entities.Zone Zone
        {
            get;
            set;
        }

        public Vanrise.Entities.Country Country
        {
            get;
            set;
        }
    }
}

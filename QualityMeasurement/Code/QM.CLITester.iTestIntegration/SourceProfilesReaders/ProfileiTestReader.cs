﻿using QM.CLITester.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.CLITester.MainExtensions.SourceProfilesReaders
{
    public class ProfileiTestReader : SourceProfileReader 
    {
        public string Dummy { get; set; }

        public override bool UseSourceItemId
        {
            get
            {
                return true;
            }
        }

        public override IEnumerable<SourceProfile> GetChangedItems(ref object updatedHandle)
        {
            return new List<SourceProfile>();
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.BP.Activities
{

    public sealed class BuildCDRCorrelationFilterGroup : CodeActivity
    {
        [RequiredArgument]
        public InArgument<string> IdFieldName { get; set; }

        [RequiredArgument]
        public OutArgument<RecordFilterGroup> RecordFilterGroup { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            throw new NotImplementedException();
        }
    }
}
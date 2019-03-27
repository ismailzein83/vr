﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;

namespace Vanrise.BusinessProcess.Business
{
    public class StartBPProcessAction: GenericBEActionSettings
    {
        public override Guid ConfigId { get { return new Guid("0E9730F4-1197-456B-9261-E745FE4AFB3B"); } }
        public Guid BPDefinitionId { get; set; }
        public List<InputArgumentMapping> InputArgumentsMapping { get; set; }
        public override string ActionTypeName { get { return "StartBPProcess"; } }
        public override string ActionKind { get { return "StartBPProcessAction"; } }
    }

    public class InputArgumentMapping
    {
        public string InputArgumentName { get; set; }
        public string MappedFieldName { get; set; }
    }
}

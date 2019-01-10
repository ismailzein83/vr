﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.Security.Entities;

namespace Vanrise.GenericData.MainExtensions
{
    public class DeleteGenericBEAction : GenericBEActionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("6BC1FB84-F28D-476A-81FA-A11FC4E5CC06"); }
        }
        public override string ActionTypeName { get { return "DeleteGenericBEAction"; } }

		public override string ActionKind { get { return "Delete"; } }

	}
}

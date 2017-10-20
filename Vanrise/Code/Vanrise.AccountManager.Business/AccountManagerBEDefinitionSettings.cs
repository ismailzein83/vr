﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountManager.Entities;
using Vanrise.GenericData.Entities;

namespace Vanrise.AccountManager.Business
{
    public class AccountManagerBEDefinitionSettings : BusinessEntityDefinitionSettings
    {
        public static Guid s_configId = new Guid("924F08E3-27EE-4954-BFB8-CCE496B3AE5C");
        public override Guid ConfigId { get { return s_configId; } }
        public override string SelectorFilterEditor { get; set; }
        public override string DefinitionEditor
        {
            get { return "vr-accountmanager-accountmanagerdefinition-editor"; }
        }
        public override string IdType
        {
            get { return "System.Int64"; }
        }
        public override string ManagerFQTN
        {
            get { return "Vanrise.AccountManager.Business.AccountManagerBEDefinitionSettings, Vanrise.AccountManager.Business"; }
        }
       
        public List<AccountManagerAssignmentDefinition> AssignmentDefinitions { get; set; }

        public List<AccountManagerSubViewDefinition> SubViews { get; set; }
    }
}

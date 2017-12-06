using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountManager.Entities;

namespace Vanrise.AccountManager.Business
{
    public class AssignmentDefinitionTrackAndLogObject : IAssignmentDefinitionTrackAndLogObject
    {
        public AccountManagerAssignment AccountManagerAssignmentToTrack { get; set; }
    }
}

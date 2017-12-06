using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public class PasswordSettings
    {
        public int PasswordLength { get; set; }

        public int MaxPasswordLength { get; set; }
        public int MaximumUserLoginTries { get; set; }
        public int UserPasswordHistoryCount { get; set; }
        public int MinutesToLock { get; set; }
        public TimeSpan? FailedInterval { get; set; }
        public PasswordComplexity? PasswordComplexity { get; set; }
        public Guid? NotificationMailTemplateId { get; set; }
    }

    public enum PasswordComplexity
    {

        Medium = 1,
        Strong = 2
    }
}

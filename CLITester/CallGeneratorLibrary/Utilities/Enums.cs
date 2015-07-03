using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallGeneratorLibrary.Utilities
{
    public class Enums
    {
        public enum Service : int
        {
            SIMServer = 1,
            AndroidDevice = 2,
            Monty = 3
        }
        public enum PeriodRecharge : int
        {
            Daily = 1,
            Monthly = 2,
            yearly = 3
        }


        public enum ContractType : int
        {
            Regular = 1,
            Bonus = 2
        }

        public enum UserRole : int
        {
            Administrator = 1,
            SuperUser = 2,
            User = 3
        }

        public enum Lang : short
        {
            English = 1,
            Arabic = 2,
            French = 3
        }
        public enum Direction : short
        {
            ltr = 1,
            rtl = 2
        }
        public enum Align : short
        {
            left = 1,
            right = 2
        }

        public enum ActionType
        {
            Add = 1,
            Modify = 2,
            Delete = 3,
            Login = 4,
            Logout = 5,
            ScheduleDone = 6,
            ScheduleFailed = 7
        }

        public enum CallStatus
        {
            ErrorMessage = 0,
            [Description("Error")]
            CLIValid = 1,
            [Description("CLI Valid")]
            CLINotValid = 2,
            [Description("CLI Not Valid")]
            Waiting = 3,
            Expired = 4,
            Failed = 5,
            Phase = 6
        }

        public enum PhoneNumberStatus
        {
            Free = 0,
            Busy = 1
        }
    }
}

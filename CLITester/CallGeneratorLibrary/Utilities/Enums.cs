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
            Monty = 1,
            AndroidDevice = 2
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
        public enum Datatype
        {
            Text = 1,
            Decimal = 2,
            Integer = 3,
            Currency = 4,
            LongText = 5,
            ListOfDates = 6,
            ListOfString = 7,
            ListOfCurrency = 8,
            DateTime = 9,
            ListOfLookups = 10,
            Label = 11
        }

        public enum FieldType
        {
            InsuranceOrFees = 1,
            Rate = 3,
            AmountBorrowed = 4,
            FirstPaymentDate = 6,
            Period = 7
        }


        public enum LookupIdValues
        {
            JobCategory = 1,
            JobType = 2,
            JobGender = 3,
            JobDuration = 4,
            Answer = 5,
            Degree = 6,
            Nationality = 7,
            MaritalStatus = 8
        }


        public enum ActionModule
        {
            Lookup = 8,
            LookupValue = 9,
            Currency = 10,
            Exception = 99
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

    }
}

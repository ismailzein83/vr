using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace Vanrise.CommonLibrary
{
    public class PasswordValidator : BaseValidator
    {
        string propertyToValidate = string.Empty;
        public string PropertyToValidate { get { return propertyToValidate; } set { propertyToValidate = value; } }

        protected override bool EvaluateIsValid()
        {
            string value = this.GetControlValidationValue(this.ControlToValidate);
            if (string.IsNullOrWhiteSpace(value))
                return false;
            //at least 9 characters, 
            //with at least one character from each of the 4 character classes (alphabetic lower and upper case; numeric, symbols).
            //string passwordRegex = "(?=.{9,})(?=.*?[^\w\s])(?=.*?[0-9])(?=.*?[A-Z]).*?[a-z].*";
            //if(System.Text.RegularExpressions.Regex.IsMatch(passwordRegex))
            string error = Manager.IsValidPassword(value, value);
            if (string.IsNullOrWhiteSpace(error))
                return true;
            ErrorMessage = error;
            return false;
        }
    }
}
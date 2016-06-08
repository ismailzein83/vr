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
            string error = Manager.IsValidPassword(value, value);
            if (string.IsNullOrWhiteSpace(error))
                return true;
            ErrorMessage = error;
            return false;
        }
    }
}
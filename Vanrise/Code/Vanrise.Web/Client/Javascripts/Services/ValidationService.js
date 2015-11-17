(function (appControllers) {

    "use strict";
    VRValidationService.$inject = ['ValidationMessagesEnum'];

    function VRValidationService(ValidationMessagesEnum) {

        function validate(value, $scope, $attrs, validationOptions) {
            if ($attrs.isrequired != undefined && ($attrs.isrequired == "" || $scope.$parent.$eval($attrs.isrequired))) {
                if (value == undefined || value.length == 0)
                    return ValidationMessagesEnum.required;
            }
            if (value != undefined && value != null) {
                if (validationOptions != undefined) {
                    if (validationOptions.emailValidation) {
                        if (!validateEmail(value))
                            return ValidationMessagesEnum.invalidEmail;
                    }
                    if (validationOptions.numberValidation) {
                        if (!validateNumber(value, validationOptions))
                            return ValidationMessagesEnum.invalidNumber;
                    }
                }
                if ($attrs.customvalidate != undefined) {
                    var customerValidationError = $scope.$parent.$eval($attrs.customvalidate);
                    if (customerValidationError != null)
                        return customerValidationError;
                }
            }
            return null;
        }

        function validateEmail(value) {
            var isEmail_re = /^\s*[\w\-\+_]+(\.[\w\-\+_]+)*\@[\w\-\+_]+\.[\w\-\+_]+(\.[\w\-\+_]+)*\s*$/;
            return String(value).search(isEmail_re) != -1;
        }

        function validateNumber(value, validationOptions) {
            var decimalArray = String(value).split(".");
            var validmax = (validationOptions.maxNumber != undefined) ? parseFloat(value) <= validationOptions.maxNumber : true;
            var validmin = (validationOptions.minNumber != undefined) ? parseFloat(value) >= validationOptions.minNumber : true;
            var validmaxprec = (validationOptions.numberPrecision != undefined && decimalArray[1] != undefined) ? decimalArray[1].length <= validationOptions.numberPrecision : true;
            return validmin && validmax && validmaxprec;
        }

        function validateTimeRange(fromDate, toDate) {
            if (fromDate == undefined || toDate == undefined)
                return null;
            var from = new Date(fromDate);
            var to = new Date(toDate);
            if (from.getTime() > to.getTime())
                return "Start should be before end";
            else
                return null;
        }

        return ({
            validate: validate,
            validateTimeRange: validateTimeRange
        });
    }

    appControllers.service('VRValidationService', VRValidationService);
})(appControllers);
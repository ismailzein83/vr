(function (appControllers) {

    "use strict";
    VRValidationService.$inject = ['UtilsService', 'ValidationMessagesEnum'];

    function VRValidationService(UtilsService, ValidationMessagesEnum) {

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

        	var errorMessage = "Invalid time range";

        	var startDate = getDateObject(fromDate);
        	var endDate = getDateObject(toDate);

        	if (startDate != undefined && endDate != undefined) {
        		if (startDate.getTime() > endDate.getTime())
        			return errorMessage;
        	}
        	else {
        		if (fromDate instanceof Object && toDate instanceof Object) {
        			if (toDate.Hour == 0 && toDate.Minute == 0)
        				return null;
        			else if (fromDate.Hour > toDate.Hour || (fromDate.Hour == toDate.Hour && fromDate.Minute > toDate.Minute))
        				return errorMessage;
        		}
        	}
        	return null;
        }

        function getDateObject(date) {
        	if (date instanceof Date)
        		return date;
        	else if (typeof (date) == 'string')
        		return UtilsService.createDateFromString(date);
        	else
        		return undefined;
        }

        function validateTimeEqualorGreaterthanToday(currentDate) {
            var errorMessage = "Date cannot be in the past";
            var today = new Date();
            today.setHours(0, 0, 0, 0);

            if (currentDate instanceof Date) {
                if (currentDate == undefined)
                    return null;
                var from = new Date(currentDate);
                if (from.getTime() < today.getTime())
                    return errorMessage;
                else
                    return null;
            }
            else if (currentDate instanceof Object) {
                if (currentDate.Hour < today.Hour || (currentDate.Hour == today.Hour && currentDate.Minute < today.Minute))
                    return errorMessage;
                else
                    return null;
            }
            else if (typeof currentDate == 'string') {
                if (currentDate == undefined)
                    return null;
                var from = new Date(currentDate);
                if (from.getTime() < today.getTime())
                    return errorMessage;
                else
                    return null;
            }
        }

        return ({
            validate: validate,
            validateEmail:validateEmail,
            validateTimeRange: validateTimeRange,
            validateTimeEqualorGreaterthanToday: validateTimeEqualorGreaterthanToday
        });
    }

    appControllers.service('VRValidationService', VRValidationService);
})(appControllers);
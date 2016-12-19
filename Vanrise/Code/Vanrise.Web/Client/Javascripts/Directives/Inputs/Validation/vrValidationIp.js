'use strict';
app.directive('vrValidationIp', function () {

    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, elm, attrs, ctrlModel) {

            var validate = function (viewValue) {
                if (viewValue != null && viewValue != "" && viewValue != undefined) {

                    var isIp_re = /^([0-9]{1,3})[.]([0-9]{1,3})[.]([0-9]{1,3})[.]([0-9]{1,3})$/;
                    var ipParts = value.split(".");

                    if (String(value).search(isIp_re) == -1)
                        ctrlModel.$setValidity('invalidip', false);

                    if (parseInt(ipParts[0]) > 255 || parseInt(ipParts[1]) > 255 || parseInt(ipParts[2]) > 255 || parseInt(ipParts[3]) > 255)
                        ctrlModel.$setValidity('invalidip', false);
                    else
                        ctrlModel.$setValidity('invalidip', true);
                    
                    return viewValue;
                }
                else {
                    ctrlModel.$setValidity('invalidip', true);
                    return '';
                }
            };
            ctrlModel.$parsers.unshift(validate);
            ctrlModel.$formatters.push(validate);
        }
    };
});

'use strict';
app.directive('vrValidationEmail', function () {

    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, elm, attrs, ctrlModel) {

            var validate = function (viewValue) {
                if (viewValue != null && viewValue != "" && viewValue != undefined) {
                    var isEmail_re = /^\s*[\w\-\+_]+(\.[\w\-\+_]+)*\@[\w\-\+_]+\.[\w\-\+_]+(\.[\w\-\+_]+)*\s*$/;
                    var valid = String(viewValue).search(isEmail_re) != -1;
                    ctrlModel.$setValidity('invalidemail', valid);
                    return viewValue;
                }
                else {
                    ctrlModel.$setValidity('invalidemail', true);
                    return '';
                }
            };
            ctrlModel.$parsers.unshift(validate);
            ctrlModel.$formatters.push(validate);
        }
    };
});

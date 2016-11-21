'use strict';

app.directive('vrValidationArray', function () {

    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, elm, attrs, ctrlModel) {

            var validate = function (viewValue) {
                if (viewValue == undefined || viewValue == 0) ctrlModel.$setValidity('requiredarray', false);
                else ctrlModel.$setValidity('requiredarray', true);
                return viewValue;
            };
            ctrlModel.$parsers.unshift(validate);
            ctrlModel.$formatters.push(validate);
        }
    };
});
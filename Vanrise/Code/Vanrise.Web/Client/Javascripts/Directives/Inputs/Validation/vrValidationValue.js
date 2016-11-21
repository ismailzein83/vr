'use strict';

app.directive('vrValidationValue', function () {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, elm, attrs, ctrlModel) {

            var validate = function (viewValue) {
                if (viewValue == undefined || viewValue === '') ctrlModel.$setValidity('requiredvalue', false);
                else ctrlModel.$setValidity('requiredvalue', true);
                return viewValue;
            };
            ctrlModel.$parsers.unshift(validate);
            ctrlModel.$formatters.push(validate);

        }
    };
});
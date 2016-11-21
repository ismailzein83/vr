'use strict';

app.directive('vrValidationCustom', ['ValidationMessagesEnum', function (ValidationMessagesEnum) {
    return {
        restrict: 'A',
        require: ['ngModel', '^form'],
        link: function (scope, elm, attrs, controllers) {
            var ctrlModel = controllers[0];
            scope.ctrl.customMessage = '';


            var validate = function (viewValue) {
                var isvalid = true;
                scope.ctrl.customMessage = scope.ctrl.customvalidate()(viewValue);
                if (scope.ctrl.customMessage == undefined || scope.ctrl.customMessage == null || scope.ctrl.customMessage == '') isvalid = true;
                else isvalid = false;
                ctrlModel.$setValidity('customvalidation', isvalid);
                //controllers[1].validateForm();//.$checkValidity()
                return viewValue;
            };
            ctrlModel.$parsers.unshift(validate);
            ctrlModel.$formatters.push(validate);

        }
    };
}]);
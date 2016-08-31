'use strict';
app.directive('vrValidationNumber', function () {

    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, elm, attrs, ctrlModel) {

            var validate = function (viewValue) {

                if (viewValue != null && viewValue != "" && viewValue != undefined) {
                    var decimalArray = String(viewValue).split(".");
                    var negativeArray = String(viewValue).split("-");
                    var validmax = (scope.ctrl.maxvalue != undefined) ? parseFloat(viewValue) <= scope.ctrl.maxvalue : true;
                    var validmin = (scope.ctrl.minvalue != undefined) ? parseFloat(viewValue) >= scope.ctrl.minvalue : true;
                    var validnegprec = viewValue.tirm() == '-' ? true : false;                 

                    ctrlModel.$setValidity('invalidnumber', validmin && validmax && validmaxprec && validnegprec);
                    return viewValue;
                }
                else {
                    ctrlModel.$setValidity('invalidnumber', true);
                    return '';
                }


            }
            ctrlModel.$parsers.unshift(validate);
            ctrlModel.$formatters.push(validate);
        }
    };
});

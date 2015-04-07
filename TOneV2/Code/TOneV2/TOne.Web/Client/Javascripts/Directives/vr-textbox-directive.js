'use strict';

var INTEGER_REGEXP = /^\-?\d+$/;
app.directive('integer', function () {
    return {
        require: 'ngModel',
        link: function (scope, elm, attrs, ctrl) {
            ctrl.$validators.integer = function (modelValue, viewValue) {
                if (ctrl.$isEmpty(modelValue)) {
                    // consider empty models to be valid
                    return true;
                }

                if (INTEGER_REGEXP.test(viewValue)) {
                    // it is valid
                    return true;
                }

                // it is invalid
                return false;
            };
        }
    };
});


app.directive('vrTextbox', ['TextBoxService', function (TextBoxService) {

    var directiveDefinitionObject = {

        require: '^form',
        restrict: 'E',
        scope: {
            placeholder: '@',
            label: '@',
            icon: '@',
            buttontext: '@',
            value: '=',
            btnClick: '&'
        },
        controller: function () {
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {
            TextBoxService.allDirective.forEach(function (item) { attrs = TextBoxService.setDefaultAttributes(attrs, item); });

            return {
                pre: function (scope, elem, attrs, formCtrl) {

                    scope.valelement = function () {
                        //console.log(formCtrl.inputValue.$valid);
                        //console.log(formCtrl.$valid);
                    };

                }
            }
        },
        templateUrl: function (element, attrs) {
            return TextBoxService.getTemplateUrl(attrs.type);
        }

    };

    return directiveDefinitionObject;

}]);
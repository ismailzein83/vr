'use strict';

app.directive('vrValidationGroup', [function () {

    var directiveDefinitionObject = {
        restrict: 'E',
        require: '?^^vrValidationGroup',
        scope: {
            validationcontext: '='
        },
        controller: function ($scope, $element, $attrs) {


            var validators = [];

            var ctrl = this;
            ctrl.addValidator = function (validationLogic) {
                var validator = {
                    validationLogic: validationLogic
                };
                validators.push(validator);
                return validator;
            };

            ctrl.removeValidator = function (validator) {
                var itemIndex = validators.indexOf(validator);
                validators.splice(itemIndex, 1);
            };

            if (ctrl.validationcontext == undefined)
                ctrl.validationcontext = {};
            ctrl.validationcontext.validate = function () {
                for (var i = 0; i < validators.length; i++) {
                    var validationError = validators[i].validationLogic();
                    if (validationError != null)
                        return validationError;
                }
                return null;
            };
        },
        controllerAs: 'ValidationGroupCtrl',
        bindToController: true,
        compile: function (tElement, tAttrs) {

            return {
                pre: function ($scope, iElem, iAttrs, parentValidationGroupCtrl) {
                    if (parentValidationGroupCtrl != null) {
                        var validator = parentValidationGroupCtrl.addValidator($scope.ValidationGroupCtrl.validationcontext.validate);
                        $scope.$on('$destroy', function () {
                            parentValidationGroupCtrl.removeValidator(validator);
                        });
                    }
                }
            };
        }
    };

    return directiveDefinitionObject;

}]);
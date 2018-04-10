'use strict';

app.directive('vrValidator', ['$compile', 'UtilsService', function ($compile, UtilsService) {
    return {
        restrict: 'E',
        require: '?^^vrValidationGroup',
        scope: false,
        controller: function ($scope, $element, $attrs) {

        },
        compile: function (tElement, tAttrs) {
            var showTooltipVariableName = UtilsService.generateJSVariableName();
            var validationMessage = '<div  class="disable-animations tooltip-error" ng-style="(' + showTooltipVariableName + ' && ' + tAttrs.validate + ' != null) ? {\'display\':\'block\'} : {\'display\':\'none\'} ">';
            validationMessage += '<div style="white-space: pre-line;">{{' + tAttrs.validate + '}}</div></div>';

            var newElement = '<div validator-section id="validator-container"  ng-class="{\'required-inpute\' : ' + tAttrs.validate + ' != null }" ng-mouseenter="' + showTooltipVariableName + '=true;$root.onValidationMessageShown($event)" ng-mouseleave="' + showTooltipVariableName + '=false" >'
                + '<div  >' + tElement.html() + '</div>'
            + validationMessage
            + '</div>';
            tElement.html(newElement);
            return {
                pre: function ($scope, iElem, iAttrs, parentValidationGroupCtrl) {
                    if (parentValidationGroupCtrl != null) {
                        var validate = function () {
                            return $scope.$eval(iAttrs.validate);
                        };
                        var validator = parentValidationGroupCtrl.addValidator(validate);
                       
                        $scope.$on('$destroy', function () {
                            iElem.off();
                            parentValidationGroupCtrl.removeValidator(validator);
                        });
                    }
                }
            };
        }
    };
}]);
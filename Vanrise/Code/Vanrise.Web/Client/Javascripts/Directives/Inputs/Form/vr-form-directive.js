'use strict';

app.directive('vrForm', [function () {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope:false,
        controller: function ($scope, $element, $attrs) {
            $scope.onKeyUp = function ($event) {
                if ($attrs.submitname != undefined) {
                    if ($event.which == 13) {
                        $scope.$broadcast('submit' + $attrs.submitname);                        
                    }
                }
            }
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (tElement, tAttrs) {
            var validationContext = tAttrs.validationcontext != undefined ? tAttrs.validationcontext : tAttrs.name;
            var validateOutputAttribute = validationContext != undefined ? (' validationcontext="' + validationContext + '" ') : '';
            var newElement = '<div ng-keyup="onKeyUp($event)" ><vr-validation-group' + validateOutputAttribute + '>' + tElement.html() + '</vr-validation-group></div>';
            tElement.html(newElement);
        }
    };

    return directiveDefinitionObject;

}]);







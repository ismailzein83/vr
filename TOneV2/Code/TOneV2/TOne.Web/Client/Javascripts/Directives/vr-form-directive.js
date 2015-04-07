'use strict';


app.directive('vrForm', [function () {

    var directiveDefinitionObject = {
        transclude: true,
        restrict: 'E',
        scope: {
        
        },
        controller: function ($scope, $element) {

        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {


                }
            }
        },
        template: function (element, attrs) {
            return '<form name="' + attrs.name + '"  novalidate ng-transclude></form>';
        }

    };

    return directiveDefinitionObject;

}]);
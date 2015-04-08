'use strict';


app.directive('vrForm', [function () {

    var directiveDefinitionObject = {
        transclude: true,
        restrict: 'E',
        //scope: {
        
        //},
        controller: function ($scope, $element) {

        },
        controllerAs: 'ctrl',
        bindToController: true,
        //link: function (scope, element, attrs, ctrl, transclude) {
        //    transclude(scope.$parent, function (clone, scope) {
        //        //element.append(scope);
        //    });
        //},
        template: function (element, attrs) {
            return '<form name="' + attrs.name + '"  novalidate ng-transclude></form>';
        }

    };

    return directiveDefinitionObject;

}]);
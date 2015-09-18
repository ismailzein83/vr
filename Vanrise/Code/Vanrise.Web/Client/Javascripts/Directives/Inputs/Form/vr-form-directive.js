'use strict';


app.directive('vrForm', [function () {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope:false,
        controller: function ($scope, $element, $attrs) {
            $scope.submitForm = function ($event) {
                if ($event.which == 13) {                   
                     $scope.$broadcast('submit' + $attrs.name);
                }
            }
        },
        controllerAs: 'ctrl',
        compile: function (tElement, tAttrs) {
            var newElement = '<form ng-keyup="submitForm($event)" name="' + tAttrs.name + '"  novalidate >' + tElement.context.innerHTML + '</form>';
            tElement.html(newElement);            
        }
    };

    return directiveDefinitionObject;

}]);
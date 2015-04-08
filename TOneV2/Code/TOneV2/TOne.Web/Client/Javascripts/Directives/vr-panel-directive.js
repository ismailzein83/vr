'use strict';

app.directive('vrPanel', [function () {

    var directiveDefinitionObject = {
        transclude: true,
        restrict: 'E',
        controller: function ($scope, $element) {

        },
        controllerAs: 'ctrl',
        bindToController: true,
        template: function (element, attrs) {
            return '<div class="panel panel-primary panel-over-color"><div class="panel-body" ng-transclude></div></div>';
        }

    };

    return directiveDefinitionObject;

}]);
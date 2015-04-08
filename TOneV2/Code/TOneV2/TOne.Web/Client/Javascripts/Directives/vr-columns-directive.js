'use strict';

app.directive('vrColumns', [function () {

    var directiveDefinitionObject = {
        transclude: true,
        restrict: 'E',
        //replace: true,
        scope: {},
        link: function(scope, element, attrs, transclude) {
            transclude(scope.$parent, function(content) {
                element.append(content);
            });
        },
        controllerAs: 'ctrl',
        bindToController: true,
        template: function (element, attrs) {
            var numberOfColumns = attrs.colnum;
            if (numberOfColumns == undefined)
                numberOfColumns = 1;
            return '<div class="col-lg-' + numberOfColumns + ' style-col" ng-transclude></div>';
        }

    };

    return directiveDefinitionObject;

}]);
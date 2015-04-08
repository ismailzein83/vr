'use strict';

app.directive('vrRow', [function () {

    var directiveDefinitionObject = {
        transclude: true,
        restrict: 'E',
        //replace: true,
        scope: {},
        link: function (scope, element, attrs, transclude) {
            
            transclude(scope.$parent, function(content) {
                element.append(content);
            });
        },
        template: function (element, attrs) {
            var removeline = attrs.removeline;
            return '<div class="row' + (removeline == 'true' ? '' : ' style-row') + '" ng-transclude></div>';
        }

    };

    return directiveDefinitionObject;

}]);
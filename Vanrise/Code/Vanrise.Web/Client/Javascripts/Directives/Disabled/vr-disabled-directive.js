'use strict';


app.directive('vrDisabled', ['$compile', function ($compile) {

    var directiveDefinitionObject = {
        restrict: 'A',
        terminal: true,
        priority: 2000,
        link: function preLink(scope, iElement, iAttrs) { 
            iElement.removeAttr("vr-disabled");
            iElement.attr('ng-class', '{\'divDisabled\': ' + iAttrs.vrDisabled + '}');
            var compiled = $compile(iElement)(scope);

            return function (scope) {
                compiled(scope);
            };
        }
 
    };
    return directiveDefinitionObject;

}]);


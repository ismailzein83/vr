'use strict';


app.directive('vrDisabled', ['$compile', function ($compile) {

    var directiveDefinitionObject = {
        restrict: 'A',
        scope: false,
        compile: function (tElement, tAttrs) {
            tElement.attr('ng-class', '{\'divDisabled\': ' + tAttrs.vrDisabled + '}');
            tElement.removeAttr("vr-disabled");
            return {
                post: function postLink(scope, iElement, iAttrs, controller) {
                    $compile(iElement)(scope);
                }
            };
        }

    };
    return directiveDefinitionObject;

}]);


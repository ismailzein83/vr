'use strict';


app.directive('vrDisabled', ['$compile', function ($compile) {

    var directiveDefinitionObject = {
        restrict: 'A',
        scope: false,
        terminal: true,
        link: function preLink(scope, iElement, iAttrs) {
            iElement.removeAttr("vr-disabled");
            iElement.attr('ng-class', '{\'divDisabled\': ' + iAttrs.vrDisabled + '}');
            $compile(iElement)(scope);
        }

    };
    return directiveDefinitionObject;

}]);


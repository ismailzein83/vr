'use strict';


app.directive('vrActionbar', ['ActionBarDirService', function (ActionBarDirService) {

    var directiveDefinitionObject = {
        transclude: true,
        restrict: 'E',
        scope: {
            showcollapsebutton: '=',
            issectioncollapsed: '='
        },
        controller: function ($scope, $element) {
            
        },
        controllerAs: 'ctrl',
        bindToController: true,
        //link: function (scope, element, attrs, ctrl, transclude) {
        //    transclude(scope.$parent, function (clone, scope) {
        //        element.append(scope);
        //    });
        //},
        templateUrl: function (element, attrs) {
            return ActionBarDirService.dTemplate;
        }

    };

    return directiveDefinitionObject;

}]);
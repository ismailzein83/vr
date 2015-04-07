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
        compile: function (element, attrs) {

            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {


                }
            }
        },
        templateUrl: function (element, attrs) {
            return ActionBarDirService.dTemplate;
        }

    };

    return directiveDefinitionObject;

}]);
'use strict';

app.directive('vrActionbar', ['ActionBarDirService', 'MultiTranscludeService', function (ActionBarDirService, MultiTranscludeService) {

    var directiveDefinitionObject = {
        transclude: true,
        restrict: 'E',
        scope: {
            showcollapsebutton: '=',
            issectioncollapsed: '='
        },
        controller: function ($scope, $element) {

        },
        link: function (scope, elem, attr, ctrl, transcludeFn) {
            MultiTranscludeService.transclude(elem, transcludeFn);
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: function (element, attrs) {
            return ActionBarDirService.dTemplate;
        }
    };

    return directiveDefinitionObject;

}]);
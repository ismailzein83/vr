'use strict';

app.directive('vrDirectivetabs', ['UtilsService', function (UtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            tabitems: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;           
            
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {

                }
            };
        },
        templateUrl: '/Client/Javascripts/Directives/Tabs/Templates/DirectiveTabsTemplate.html'

    };

    return directiveDefinitionObject;
}]);


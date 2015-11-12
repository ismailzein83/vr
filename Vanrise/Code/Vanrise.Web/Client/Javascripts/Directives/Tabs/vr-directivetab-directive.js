'use strict';

app.directive('vrDirectivetab', ['UtilsService', function (UtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            tabitem: '='
        },
        require: '^vrTabs',
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var tabItem = ctrl.tabitem;
            if (tabItem.dontLoad == undefined)
                tabItem.dontLoad = true;
            tabItem.isLoading = true;
            tabItem.onDirectiveReady = function (directiveAPI) {
                if (tabItem.loadDirective != undefined) {
                    UtilsService.convertToPromiseIfUndefined(tabItem.loadDirective(directiveAPI)).finally(function () {
                        tabItem.isLoading = false;
                    });
                }
                else
                    tabItem.isLoading = false;
            };

        },
        controllerAs: 'vrDirectiveTabCtrl',
        bindToController: true,
        templateUrl: '/Client/Javascripts/Directives/Tabs/Templates/DirectiveTabTemplate.html'
    };

    return directiveDefinitionObject;



}]);


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

            setExtensionObject(ctrl.tabitem);

            function setExtensionObject(tabItem) {                
                if (tabItem.extensionObject != undefined)
                    return;
                if (tabItem.dontLoad == undefined)
                    tabItem.dontLoad = true;
                var extensionObject = {};
                extensionObject.isLoading = true;
                extensionObject.onDirectiveReady = function (directiveAPI) {
                    if (tabItem.loadDirective != undefined) {
                        UtilsService.convertToPromiseIfUndefined(tabItem.loadDirective(directiveAPI)).finally(function () {
                            extensionObject.isLoading = false;
                        });
                    }
                    else
                        extensionObject.isLoading = false;
                };
                tabItem.extensionObject = extensionObject;
            }

        },
        controllerAs: 'vrDirectiveTabCtrl',
        bindToController: true,
        templateUrl: '/Client/Javascripts/Directives/Tabs/Templates/DirectiveTabTemplate.html'
    };

    return directiveDefinitionObject;



}]);


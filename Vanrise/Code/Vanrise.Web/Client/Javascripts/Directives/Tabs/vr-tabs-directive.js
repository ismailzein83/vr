'use strict';


app.directive('vrTabs', ['MultiTranscludeService', function (MultiTranscludeService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            ctrl.tabs = [];
            ctrl.addTab = function (tab) {
                if (ctrl.tabs.length == 0)
                    tab.isLoaded = true;
                ctrl.tabs.push(tab);
            };

            ctrl.tabSelectionChanged = function () {
                ctrl.tabs[ctrl.selectedTabIndex].isLoaded = true;
            }

            ctrl.removeTab = function(tab)
            {
                var index = ctrl.tabs.indexOf(tab);
                ctrl.tabs.splice(index, 1);
            }
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {
            element.prepend('<vr-tabs-header></vr-tabs-header>');

            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {

                }
            }
        }

    };

    return directiveDefinitionObject;
}]);


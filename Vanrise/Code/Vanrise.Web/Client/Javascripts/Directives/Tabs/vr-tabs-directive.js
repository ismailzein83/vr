﻿'use strict';


app.directive('vrTabs', ['MultiTranscludeService', 'UtilsService', function (MultiTranscludeService, UtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            onselectionchanged: '&'
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            ctrl.tabs = [];
            ctrl.addTab = function (tab) {
                if (ctrl.tabs.length == 0)
                    tab.isLoaded = true;
                ctrl.tabs.push(tab);
            };
            ctrl.getMinHeight = function () {
                return { 'min-height': (ctrl.tabs.length * 26 + 1) + 'px' };
            };
            ctrl.tabSelectionChanged = function () {
                if (ctrl.tabs[ctrl.selectedTabIndex] != undefined)
                    ctrl.tabs[ctrl.selectedTabIndex].isLoaded = true;
                if (ctrl.onselectionchanged && typeof (ctrl.onselectionchanged) == 'function') {
                    ctrl.onselectionchanged();
                }
            };

            ctrl.removeTab = function (tab) {
                var index = ctrl.tabs.indexOf(tab);
                ctrl.tabs.splice(index, 1);
                if (typeof (tab.onremove) === 'function') {
                    tab.onremove(tab);
                }
        
                $("#" + tab.guid).remove();
                setTimeout(function () {
                    UtilsService.safeApply($scope);
                }, 1)
                if (ctrl.tabs[0] != undefined && tab.onremove!=undefined)
                     ctrl.tabs[0].isSelected = true;

            };
            var api = {};
            api.removeAllTabs = function () {
                ctrl.tabs.length = 0;
            };
            api.setTabSelected = function (index) {
                setTimeout(function () {
                    var tab = ctrl.tabs[index];
                    if (tab != undefined)
                        tab.isSelected = true;
                },1);
            };
            api.setLastTabSelected = function () {
                setTimeout(function () {
                    ctrl.tabs[ctrl.tabs.length - 1].isSelected = true;
                });
            };
            if (ctrl.onReady != null)
                ctrl.onReady(api);
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {
            element.prepend('<vr-tabs-header></vr-tabs-header>');

            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {

                }
            };
        }

    };

    return directiveDefinitionObject;
}]);


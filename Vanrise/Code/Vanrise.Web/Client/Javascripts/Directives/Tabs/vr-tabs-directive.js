﻿'use strict';

app.directive('vrTabs', ['MultiTranscludeService', 'UtilsService', 'VRNotificationService', function (MultiTranscludeService, UtilsService, VRNotificationService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            onselectionchanged: '&'
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            ctrl.selectedTabIndex = 0;
            ctrl.tabs = [];
            var isLock = false;
            var tempTabs = [];

            ctrl.addTab = function (tab) {
                if (!isLock) {
                    if (tab.haspermission != undefined) {
                        isLock = true;
                        tab.haspermission(tab).then(function (isAllowed) {
                            if (isAllowed) {
                                addTabObject(tab);
                            }
                            addRemainingTabs();
                        });
                    }
                    else {
                        addTabObject(tab);
                        addRemainingTabs();
                    }
                }
                else {
                    tempTabs.push(tab);
                }
            };

            function addRemainingTabs()
            {
                isLock = false;
                if (tempTabs.length > 0) {
                    var nextTab = tempTabs[0];
                    tempTabs.splice(0, 1);
                    ctrl.addTab(nextTab);
                }
            }
            function addTabObject(tab) {
                if (ctrl.tabs.length == 0) {
                    tab.isLoaded = true;
                    tab.isSelected = true;
                }
                ctrl.tabs.push(tab);
            }

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
                }, 1);
                tab.removed = true;
                if (ctrl.tabs[0] != undefined)
                    ctrl.tabs[0].isSelected = true;

            };
            ctrl.removeTabAndHeader = function (tab) {
                if (tab != undefined && !tab.removed) {
                    var index = ctrl.tabs.indexOf(tab);
                    ctrl.tabs.splice(index, 1);
                    $("#header-" + tab.guid).remove();
                    setTimeout(function () {
                        UtilsService.safeApply($scope);
                    }, 1);
                    if (ctrl.tabs[ctrl.tabs.length - 1] != undefined)
                        ctrl.tabs[ctrl.tabs.length - 1].isSelected = true;
                }
            };
            var api = {};
            api.removeAllTabs = function () {
                ctrl.tabs.length = 0;
            };
            api.setTabSelected = function (index) {
                var deferred = UtilsService.createPromiseDeferred();
                setTimeout(function () {
                    var tab = ctrl.tabs[index];
                    if (tab != undefined)
                        tab.isSelected = true;
                    deferred.resolve();
                }, 1);
                return deferred.promise;
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
            element.prepend('<vr-tabs-header ></vr-tabs-header>');

            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {

                }
            };
        }

    };

    return directiveDefinitionObject;
}]);


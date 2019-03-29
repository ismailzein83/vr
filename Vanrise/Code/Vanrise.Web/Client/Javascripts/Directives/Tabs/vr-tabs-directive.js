'use strict';

app.directive('vrTabs', ['MultiTranscludeService', 'UtilsService', 'VRNotificationService', 'MobileService', function (MultiTranscludeService, UtilsService, VRNotificationService, MobileService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            onselectionchanged: '&',
            hidepaginationcontrols: '='

        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            ctrl.selectedTabIndex = 0;
            ctrl.tabs = [];
            var isLock = false;
            var tempTabs = [];
            var isMobile = MobileService.isMobile();

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
            ctrl.pageSize = 5;
            ctrl.tabsCountStart = ctrl.selectedTabIndex;
            ctrl.tabsCountLimit = ctrl.tabsCountStart + ctrl.pageSize;         

            ctrl.isBackwardPaginationVisible = function () {
                return (ctrl.tabsCountLimit > ctrl.pageSize && ctrl.tabs.length > ctrl.pageSize) || (ctrl.tabs.length < ctrl.tabsCountLimit && ctrl.tabsCountStart > 0);
            };

            ctrl.isForwardPaginationVisible = function () {
                return ctrl.tabs.length > ctrl.pageSize && ctrl.tabsCountLimit < ctrl.tabs.length;
            };

            function addRemainingTabs() {
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
                return { 'min-height': (ctrl.pageSize * 26 + 1) + 'px' };
            };
            ctrl.tabSelectionChanged = function () {
                if (ctrl.tabs[ctrl.selectedTabIndex] != undefined)
                    ctrl.tabs[ctrl.selectedTabIndex].isLoaded = true;
                if (ctrl.onselectionchanged && typeof (ctrl.onselectionchanged) == 'function') {
                    ctrl.onselectionchanged();
                }
            };
            ctrl.hideTab = function ($index) {
                if (ctrl.hidepaginationcontrols != undefined) return false;
                return ($index >= ctrl.tabsCountLimit || $index < ctrl.tabsCountStart) && !($index == 0 && ctrl.tabs.length == 1);

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
                if (isMobile) {
                    if (ctrl.tabs[0] != undefined) {
                        ctrl.tabs[index].isSelected = true;
                    }
                }
                else {
                    if (ctrl.tabs[0] != undefined && tab.onremove == undefined) {
                        ctrl.selectedTabIndex = 0;
                    }
                    else if (tab.onremove != undefined) {
                        if (tab.isSelected == false) {
                            return;
                        }
                        if (ctrl.tabs[index] != undefined) {
                            ctrl.tabs[index].isSelected = true;
                            ctrl.selectedTabIndex = index;
                            ctrl.tabsCountStart = index;
                            ctrl.tabsCountLimit = ctrl.selectedTabIndex + ctrl.pageSize;
                            return;
                        }
                        if (ctrl.tabs.length == index && ctrl.tabs[index - 1] != undefined) {
                            ctrl.tabs[index - 1].isSelected = true;
                            ctrl.selectedTabIndex = index - 1;
                            ctrl.tabsCountStart = index - 1;
                            ctrl.tabsCountLimit = ctrl.selectedTabIndex + ctrl.pageSize;
                            return;
                        }
                        if (ctrl.tabs[0] != undefined) {
                            ctrl.tabs[0].isSelected = true;
                            ctrl.selectedTabIndex = 0;
                        }
                    }
                }

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
            ctrl.setTabSelectedIndex = function (index) {
                ctrl.tabsCountStart = index;
                ctrl.tabsCountLimit = ctrl.tabsCountStart + ctrl.pageSize;
                ctrl.selectedTabIndex = ctrl.tabsCountStart;
                ctrl.tabs[ctrl.selectedTabIndex].isLoaded = true;
            };

            ctrl.setlastTabSelectedIndex = function (index) {
                ctrl.tabsCountLimit = index + 1;
                ctrl.tabsCountStart = ctrl.tabsCountLimit - ctrl.pageSize;
                ctrl.selectedTabIndex = index;
                ctrl.tabs[ctrl.selectedTabIndex].isLoaded = true;
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
                    ctrl.setlastTabSelectedIndex(ctrl.tabs.length - 1);
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


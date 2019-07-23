'use strict';

app.directive('vrTabs', ['MultiTranscludeService', 'UtilsService', 'VRNotificationService', 'MobileService', function (MultiTranscludeService, UtilsService, VRNotificationService, MobileService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
            onselectionchanged: '&',
            hidepaginationcontrols: '=',
            settings: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            ctrl.pageSize = 5;
            if (ctrl.settings != undefined) {
                ctrl.pageSize = ctrl.settings.pagesize || ctrl.pageSize;
                ctrl.datasource = ctrl.settings.datasource != undefined ? ctrl.settings.datasource : undefined;
                ctrl.oneditclicked = ctrl.settings.oneditclicked != undefined ? ctrl.settings.oneditclicked : undefined;
                ctrl.sortable = ctrl.settings.sortable != undefined ? ctrl.settings.sortable : false;
                ctrl.datatitlefield = ctrl.settings.datatitlefield != undefined ? ctrl.settings.datatitlefield : undefined;
            }
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
                tab.orderedIndex = ctrl.tabs.length;
                if (ctrl.tabs.length == 0) {
                    tab.isLoaded = true;
                    tab.isSelected = true;
                }
                if (ctrl.datasource != undefined) {
                    tab.tabItem = ctrl.datasource[ctrl.tabs.length];
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

            ctrl.getPreviousReversedTabs = function () {
                var items = [];
                for (var i = 0 ; i < ctrl.tabs.length ; i++) {
                    if (ctrl.tabs[i].orderedIndex < ctrl.tabsCountStart)
                        items.push(ctrl.tabs[i]);
                }
                return items.slice().reverse();
            };

            ctrl.headerSortableListener = {
                handle: '.handeldrag',
                onSort: function (/**Event*/evt) {
                    for (var i = 0 ; i < ctrl.tabs.length ; i++) {
                        var newindex = i;
                        ctrl.tabs[i].orderedIndex = newindex;
                    }
                    if (ctrl.datasource && ctrl.datasource.length)
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            ctrl.datasource[i] = ctrl.tabs[i].tabItem;
                        }
                }
            };
            ctrl.hideTab = function ($index) {
                if (ctrl.hidepaginationcontrols != undefined) return false;
                return ($index >= ctrl.tabsCountLimit || $index < ctrl.tabsCountStart) && !($index == 0 && ctrl.tabs.length == 1);

            };
            ctrl.editClick = function (tab) {
                if (ctrl.oneditclicked != undefined && typeof ctrl.oneditclicked == 'function')
                    ctrl.oneditclicked(tab.data);
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
                        if (tab.isSelected == false || tab.isSelected == undefined) {
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
                            if (ctrl.tabs.length > ctrl.pageSize) {
                                ctrl.selectedTabIndex = index - 1;
                                ctrl.tabsCountLimit = ctrl.selectedTabIndex + ctrl.pageSize;
                            }
                            else {
                                ctrl.tabsCountStart = 0;
                                ctrl.tabsCountLimit = ctrl.pageSize;
                            }
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
            ctrl.setTabSelectedIndex = function (index, tab) {

                ctrl.tabsCountStart = index;
                ctrl.tabsCountLimit = ctrl.tabsCountStart + ctrl.pageSize;
                ctrl.selectedTabIndex = ctrl.tabsCountStart;
                setTimeout(function () {
                    if (tab) tab.isSelected = true;
                    $scope.$apply();
                }, 1);
                ctrl.tabs[ctrl.selectedTabIndex].isLoaded = true;
                $($element).find(".vr-tabs-expander").find(".dropdown-menu").css({ display: "none" });
            };

            ctrl.setlastTabSelectedIndex = function (index, tab) {
                ctrl.tabsCountLimit = index + 1;
                ctrl.tabsCountStart = ctrl.tabsCountLimit - ctrl.pageSize;
                ctrl.selectedTabIndex = index;
                setTimeout(function () {
                    if (tab) tab.isSelected = true;
                    $scope.$apply();
                }, 1);
                ctrl.tabs[ctrl.selectedTabIndex].isLoaded = true;
                $($element).find(".vr-tabs-expander").find(".dropdown-menu").css({ display: "none" });
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


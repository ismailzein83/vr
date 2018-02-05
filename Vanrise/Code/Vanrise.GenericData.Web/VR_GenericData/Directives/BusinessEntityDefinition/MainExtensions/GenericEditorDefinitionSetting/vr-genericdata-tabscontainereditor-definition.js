"use strict";
app.directive("vrGenericdataTabscontainereditorDefinition", ["UtilsService", "VRNotificationService", "VR_GenericData_GenericBEDefinitionService",
    function (UtilsService, VRNotificationService, VR_GenericData_GenericBEDefinitionService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new TabsContainer($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericEditorDefinitionSetting/Templates/TabsContainerDefinitionSettingTemplate.html"

        };

        function TabsContainer($scope, ctrl, $attrs) {

            var gridAPI;
            var context;

            this.initializeController = initializeController;
            function initializeController() {
                ctrl.datasource = [];
                ctrl.isValid = function () {
                    if (ctrl.datasource == undefined || ctrl.datasource.length == 0)
                        return "You Should add at least one tab.";
                    if (ctrl.datasource.length > 0 && checkDuplicateName())
                        return "Title in each tab should be unique.";

                    return null;
                };

                ctrl.addTabContainer = function () {
                    var onTabContainerAdded = function (addedItem) {
                        ctrl.datasource.push(addedItem);
                    };

                    VR_GenericData_GenericBEDefinitionService.addGenericBETabContainer(onTabContainerAdded, getContext());
                };
                ctrl.disableAddTabContainer = function () {
                    if (context == undefined) return true;
                    return context.getDataRecordTypeId() == undefined;
                };
                ctrl.removeTabContainer = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    ctrl.datasource.splice(index, 1);
                };



                defineMenuActions();
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.getData = function () {
                    var tabs;
                    if (ctrl.datasource != undefined && ctrl.datasource != undefined) {
                        tabs = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            tabs.push({
                                TabTitle: currentItem.TabTitle,
                                ShowTab: currentItem.ShowTab,
                                TabSettings: currentItem.TabSettings
                            });
                        }
                    }
                    return {
                        $type: "Vanrise.GenericData.MainExtensions.TabsContainerEditorDefinitionSetting, Vanrise.GenericData.MainExtensions",
                        TabContainers: tabs
                    };
                };

                api.load = function (payload) {
                    if (payload != undefined) {
                        context = payload.context;
                        api.clearDataSource();
                        if (payload.settings != undefined && payload.settings.TabContainers != undefined) {
                            var tabContainers = payload.settings.TabContainers;
                            for (var i = 0; i < tabContainers.length; i++) {
                                var item = tabContainers[i];
                                ctrl.datasource.push(item);
                            }
                        }
                    }
                };


                api.clearDataSource = function () {
                    ctrl.datasource.length = 0;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }



            function defineMenuActions() {
                var defaultMenuActions = [
                {
                    name: "Edit",
                    clicked: editTabContainer,
                }];

                $scope.gridMenuActions = function (dataItem) {
                    return defaultMenuActions;
                };
            }

            function editTabContainer(tabObj) {
                var onTabContainerUpdated = function (tab) {
                    var index = ctrl.datasource.indexOf(tabObj);
                    ctrl.datasource[index] = tab;
                };

                VR_GenericData_GenericBEDefinitionService.editGenericBETabContainer(onTabContainerUpdated, tabObj, getContext());
            }
            function getContext() {

                var currentContext = {
                    getFilteredFields: function () {
                        return context.getRecordTypeFields();
                    },
                    getRecordTypeFields: function () {
                        return context.getRecordTypeFields();
                    },
                    getDataRecordTypeId: function () {
                        return context.getDataRecordTypeId();
                    }
                }
                return currentContext;
            }

            function checkDuplicateName() {
                for (var i = 0; i < ctrl.datasource.length; i++) {
                    var currentItem = ctrl.datasource[i];
                    for (var j = 0; j < ctrl.datasource.length; j++) {
                        if (i != j && ctrl.datasource[j].TabTitle == currentItem.TabTitle)
                            return true;
                    }
                }
                return false;
            }
        }

        return directiveDefinitionObject;

    }
]);
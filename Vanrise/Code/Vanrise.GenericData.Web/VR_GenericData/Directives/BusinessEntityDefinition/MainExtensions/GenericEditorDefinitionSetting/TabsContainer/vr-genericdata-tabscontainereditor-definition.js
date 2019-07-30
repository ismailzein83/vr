"use strict";
app.directive("vrGenericdataTabscontainereditorDefinition", ["UtilsService", "VRUIUtilsService", "VRLocalizationService", "VR_GenericData_GenericBEDefinitionService", "VRNotificationService",
    function (UtilsService, VRUIUtilsService, VRLocalizationService, VR_GenericData_GenericBEDefinitionService, VRNotificationService) {

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
            templateUrl: "/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericEditorDefinitionSetting/TabsContainer/Templates/TabsContainerDefinitionSettingTemplate.html"

        };

        function TabsContainer($scope, ctrl, $attrs) {

            var context;
            var indexTab = 1;
            var tabsAPI;

            this.initializeController = initializeController;
            function initializeController() {
                $scope.scopeModel = {};
                ctrl.datasource = [];

                $scope.scopeModel.dragsettings = {
                    handle: '.handeldrag'
                };

                $scope.scopeModel.onTabsReady = function (api) {
                    tabsAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.addTabContainer = function () {
                    var dataItem = {
                        ShowTab: true,
                        TabTitle: "Tab " + (indexTab++)
                    };

                    dataItem.onTextResourceSelectorReady = function (api) {
                        dataItem.textResourceSeletorAPI = api;
                        var setLoader = function (value) { dataItem.isFieldTextResourceSelectorLoading = value; };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.textResourceSeletorAPI, undefined, setLoader);
                    };

                    dataItem.onEditorDirectiveReady = function (api) {
                        dataItem.editorDirectiveAPI = api;
                        var setLoader = function (value) { dataItem.isEditorDirectiveLoading = value; };
                        var payload = {
                            context: getContext(),
                        };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.editorDirectiveAPI, payload, setLoader);
                    };

                    ctrl.datasource.push(dataItem);

                    var index = ctrl.datasource.indexOf(dataItem);
                    tabsAPI.setTabSelected(index);
                };

                $scope.scopeModel.openEditTab = function (item) {
                    var currentItem = UtilsService.getItemByVal(ctrl.datasource, item.TabTitle, "TabTitle");

                    var payload = {
                        TabTitle: currentItem.TabTitle,
                        ShowTab: currentItem.ShowTab,
                    };

                    var onTabSettingsChanged = function (tabItem) {
                        currentItem.ShowTab = tabItem.ShowTab;
                    };
                    VR_GenericData_GenericBEDefinitionService.openGenericBETabContainerEditor(onTabSettingsChanged, payload, getContext());
                };

                $scope.scopeModel.onRemoveTab = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    ctrl.datasource.splice(index, 1);
                };

                $scope.scopeModel.tabsSettings = {
                    datasource: ctrl.datasource,
                    datatitlefield: "TabTitle",
                    sortable: true,
                    oneditclicked: $scope.scopeModel.openEditTab,
                    pagesize: 5
                };

                $scope.scopeModel.isValid = function () {
                    if (ctrl.datasource == undefined || ctrl.datasource.length == 0)
                        return "You Should add at least one tab.";
                    if (ctrl.datasource.length > 0 && checkDuplicateName())
                        return "Title in each tab should be unique.";

                    return null;
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    ctrl.datasource.length = 0;

                    $scope.scopeModel.isLocalizationEnabled = VRLocalizationService.isLocalizationEnabled();

                    if (payload != undefined) {
                        context = payload.context;

                        if (payload.settings != undefined && payload.settings.TabContainers != undefined) {
                            var tabContainers = payload.settings.TabContainers;
                            for (var i = 0; i < tabContainers.length; i++) {
                                var item = tabContainers[i];

                                var tabObject = {
                                    payload: item,
                                    textResourceReadyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                    textResourceLoadPromiseDeferred: UtilsService.createPromiseDeferred(),
                                    editorDirectiveReadyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                    editorDirectiveLoadPromiseDeferred: UtilsService.createPromiseDeferred()
                                };
                                if ($scope.scopeModel.isLocalizationEnabled)
                                    promises.push(tabObject.textResourceLoadPromiseDeferred.promise);
                                promises.push(tabObject.editorDirectiveLoadPromiseDeferred.promise);
                                prepareTab(tabObject);
                            }
                        }
                    }
                    return UtilsService.waitPromiseNode({ promises: promises });
                };

                api.getData = function () {
                    var tabs;
                    if (ctrl.datasource != undefined && ctrl.datasource.length > 0) {
                        tabs = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            tabs.push({
                                TabTitle: currentItem.TabTitle,
                                ShowTab: currentItem.ShowTab,
                                TabSettings: currentItem.editorDirectiveAPI != undefined ? currentItem.editorDirectiveAPI.getData() : currentItem.oldSettings,
                                TextResourceKey: currentItem.textResourceSeletorAPI != undefined ? currentItem.textResourceSeletorAPI.getSelectedValues() : currentItem.oldTextResourceKey
                            });
                        }
                    }
                    return {
                        $type: "Vanrise.GenericData.MainExtensions.TabsContainerEditorDefinitionSetting, Vanrise.GenericData.MainExtensions",
                        TabContainers: tabs
                    };
                };

                api.clearDataSource = function () {
                    ctrl.datasource.length = 0;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function prepareTab(tabObject) {
                var dataItem = {
                    TabTitle: tabObject.payload.TabTitle,
                    ShowTab: tabObject.payload.ShowTab,
                    oldSettings: tabObject.payload.TabSettings,
                    oldTextResourceKey: tabObject.payload.TextResourceKey
                };

                dataItem.onTextResourceSelectorReady = function (api) {
                    dataItem.textResourceSeletorAPI = api;
                    tabObject.textResourceReadyPromiseDeferred.resolve();
                };

                tabObject.textResourceReadyPromiseDeferred.promise.then(function () {
                    var textResourcePayload = { selectedValue: tabObject.payload.TextResourceKey };
                    VRUIUtilsService.callDirectiveLoad(dataItem.textResourceSeletorAPI, textResourcePayload, tabObject.textResourceLoadPromiseDeferred);
                });

                dataItem.onEditorDirectiveReady = function (api) {
                    dataItem.editorDirectiveAPI = api;
                    tabObject.editorDirectiveReadyPromiseDeferred.resolve();
                };

                tabObject.editorDirectiveReadyPromiseDeferred.promise.then(function () {
                    var tabPayload = {
                        settings: tabObject.payload.TabSettings,
                        context: getContext(),
                    };
                    VRUIUtilsService.callDirectiveLoad(dataItem.editorDirectiveAPI, tabPayload, tabObject.editorDirectiveLoadPromiseDeferred);
                });
                ctrl.datasource.push(dataItem);
            }

            function getContext() {

                var currentContext = {
                    getFields: function () {
                        return context.getFields();
                    },
                    getFilteredFields: function () {
                        return context.getRecordTypeFields();
                    },
                    getRecordTypeFields: function () {
                        return context.getRecordTypeFields();
                    },
                    getDataRecordTypeId: function () {
                        return context.getDataRecordTypeId();
                    },
                    getFieldType: function (fieldName) {
                        return context.getFieldType(fieldName);
                    }
                };
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
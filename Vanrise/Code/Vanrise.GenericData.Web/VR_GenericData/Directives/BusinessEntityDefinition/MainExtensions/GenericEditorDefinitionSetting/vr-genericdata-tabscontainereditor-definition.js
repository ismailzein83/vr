"use strict";
app.directive("vrGenericdataTabscontainereditorDefinition", ["UtilsService", "VRUIUtilsService","VRLocalizationService",
    function (UtilsService, VRUIUtilsService, VRLocalizationService) {

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
                $scope.scopeModel = {};
                ctrl.datasource = [];
                $scope.scopeModel.isValid = function () {
                    if (ctrl.datasource == undefined || ctrl.datasource.length == 0)
                        return "You Should add at least one tab.";
                    if (ctrl.datasource.length > 0 && checkDuplicateName())
                        return "Title in each tab should be unique.";

                    return null;
                };
                $scope.scopeModel.isLocalizationEnabled = VRLocalizationService.isLocalizationEnabled();

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();

                };
                $scope.scopeModel.addTabContainer = function () {
                    var dataItem = {
                        entity: {  }
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

                    gridAPI.expandRow(dataItem);

                    ctrl.datasource.push(dataItem);
                };

                $scope.scopeModel.removeTabContainer = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    ctrl.datasource.splice(index, 1);
                };
                $scope.scopeModel.disableAddTabContainer = function () {
                    if (context == undefined) return true;
                    var recordTypeFields = context.getRecordTypeFields();
                    return context.getDataRecordTypeId() == undefined && (recordTypeFields == undefined || recordTypeFields.length == 0);
                };
            }
            function prepareTab(tabObject) {
                var dataItem = {
                    entity: {
                        TabTitle: tabObject.payload.TabTitle,
                        ShowTab: tabObject.payload.ShowTab,
                    },
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
                    var setLoader = function (value) { dataItem.isEditorDirectiveLoading = value; };

                    var tabPayload = {
                        settings: tabObject.payload.TabSettings,
                        context: getContext(),
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.editorDirectiveAPI, tabPayload, setLoader);
                };
                ctrl.datasource.push(dataItem);
            }
 
            function defineAPI() {
                var api = {};

                api.getData = function () {
                    var tabs;
                    if (ctrl.datasource != undefined && ctrl.datasource.length>0) {
                        tabs = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            tabs.push({
                                TabTitle: currentItem.entity.TabTitle,
                                ShowTab: currentItem.entity.ShowTab,
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

                api.load = function (payload) {
                    var promises = [];
                    ctrl.datasource = [];
                    if (payload != undefined) {
                        context = payload.context;
                        api.clearDataSource();
                        if (payload.settings != undefined && payload.settings.TabContainers != undefined) {
                            var tabContainers = payload.settings.TabContainers;
                            for (var i = 0; i < tabContainers.length; i++) {
                                var item = tabContainers[i];

                                var tabObject = {
                                    payload: item,
                                    textResourceReadyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                    textResourceLoadPromiseDeferred: UtilsService.createPromiseDeferred(),
                                };
                                if ($scope.scopeModel.isLocalizationEnabled)
                                    promises.push(tabObject.textResourceLoadPromiseDeferred.promise);
                                prepareTab(tabObject);
                            }
                        }
                    }
                    return UtilsService.waitPromiseNode({ promises: promises });
                };


                api.clearDataSource = function () {
                    ctrl.datasource.length = 0;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
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
                        if (i != j && ctrl.datasource[j].entity.TabTitle == currentItem.entity.TabTitle)
                            return true;
                    }
                }
                return false;
            }
        }

        return directiveDefinitionObject;

    }
]);
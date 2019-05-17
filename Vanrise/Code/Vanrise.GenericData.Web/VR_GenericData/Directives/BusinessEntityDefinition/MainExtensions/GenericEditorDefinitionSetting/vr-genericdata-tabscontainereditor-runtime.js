"use strict";

app.directive("vrGenericdataTabscontainereditorRuntime", ["UtilsService", "VRUIUtilsService",
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new GenericEditorDefinitionSetting($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericEditorDefinitionSetting/Templates/TabsContainerRuntimeSettingTemplate.html"
        };

        function GenericEditorDefinitionSetting($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectedValues;
            var fieldValuesByName;
            var definitionSettings;
            var dataRecordTypeId;
            var historyId;
            var parentFieldValues;
            var genericContext;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.tabContainers = [];

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    if (payload != undefined) {
                        selectedValues = payload.selectedValues;
                        //fieldValuesByName = payload.fieldValuesByName;
                        definitionSettings = payload.definitionSettings;
                        dataRecordTypeId = payload.dataRecordTypeId;
                        historyId = payload.historyId;
                        parentFieldValues = payload.parentFieldValues;
                        //genericContext = payload.genericContext;
                    }

                    if (definitionSettings != undefined) {
                        if (definitionSettings.TabContainers != undefined) {
                            for (var i = 0; i < definitionSettings.TabContainers.length; i++) {
                                var tabContainer = definitionSettings.TabContainers[i];
                                var tabContainerDef = {
                                    payload: tabContainer,
                                    directiveReadyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                    directiveLoadPromiseDeferred: UtilsService.createPromiseDeferred(),
                                };
                                addTabContainer(tabContainerDef);
                                promises.push(tabContainerDef.directiveLoadPromiseDeferred.promise);
                            }
                        }
                    }

                    function addTabContainer(tabContainerDef) {
                        var tabContainer = {
                            showTab: tabContainerDef.payload.ShowTab,
                            tabTitle: tabContainerDef.payload.TabTitle,
                            runtimeEditor: tabContainerDef.payload.TabSettings.RuntimeEditor
                        };
                        tabContainer.onEditorRuntimeDirectiveReady = function (api) {
                            tabContainer.editorRuntimeAPI = api;
                            tabContainerDef.directiveReadyPromiseDeferred.resolve();
                        };
                        tabContainerDef.directiveReadyPromiseDeferred.promise.then(function () {
                            var directivePayload = {
                                dataRecordTypeId: dataRecordTypeId,
                                definitionSettings: tabContainerDef.payload.TabSettings,
                                selectedValues: selectedValues,
                                //fieldValuesByName: fieldValuesByName,
                                historyId: historyId,
                                parentFieldValues: parentFieldValues,
                               // genericContext: genericContext
                            };
                            VRUIUtilsService.callDirectiveLoad(tabContainer.editorRuntimeAPI, directivePayload, tabContainerDef.directiveLoadPromiseDeferred);
                        });
                        $scope.scopeModel.tabContainers.push(tabContainer);
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.setData = function (dicData) {
                    for (var i = 0; i < $scope.scopeModel.tabContainers.length; i++) {
                        var tabContainer = $scope.scopeModel.tabContainers[i];
                        tabContainer.editorRuntimeAPI.setData(dicData);
                    }
                };

                //api.onFieldValueChanged = function (allFieldValuesByFieldNames) {
                //    if ($scope.scopeModel.tabContainers == undefined)
                //        return null;

                //    var _promises = [];

                //    for (var i = 0; i < $scope.scopeModel.tabContainers.length; i++) {
                //        var currentTabContainers = $scope.scopeModel.tabContainers[i];
                //        if (currentTabContainers.editorRuntimeAPI.onFieldValueChanged != undefined && typeof (currentTabContainers.editorRuntimeAPI.onFieldValueChanged) == "function") {
                //            var onFieldValueChangedPromise = currentTabContainers.editorRuntimeAPI.onFieldValueChanged(allFieldValuesByFieldNames);
                //            if (onFieldValueChangedPromise != undefined)
                //                _promises.push(onFieldValueChangedPromise);
                //        }
                //    }

                //    return UtilsService.waitMultiplePromises(_promises);
                //};

                //api.setFieldValues = function (fieldValuesByNames) {
                //    if ($scope.scopeModel.tabContainers == undefined)
                //        return null;

                //    var _promises = [];

                //    for (var i = 0; i < $scope.scopeModel.tabContainers.length; i++) {
                //        var currentTabContainers = $scope.scopeModel.tabContainers[i];
                //        if (currentTabContainers.editorRuntimeAPI.setFieldValues != undefined && typeof (currentTabContainers.editorRuntimeAPI.setFieldValues) == "function") {
                //            var onFieldValueSettedPromise = currentTabContainers.editorRuntimeAPI.setFieldValues(fieldValuesByNames);
                //            if (onFieldValueSettedPromise != undefined)
                //                _promises.push(onFieldValueSettedPromise);
                //        }
                //    }

                //    return UtilsService.waitMultiplePromises(_promises);
                //};

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }
]);
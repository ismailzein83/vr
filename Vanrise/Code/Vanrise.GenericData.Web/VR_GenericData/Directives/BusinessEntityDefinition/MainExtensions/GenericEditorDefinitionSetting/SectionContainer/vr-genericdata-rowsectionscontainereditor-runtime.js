"use strict";

app.directive("vrGenericdataRowsectionscontainereditorRuntime", ["UtilsService", "VRUIUtilsService",
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
            templateUrl: "/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericEditorDefinitionSetting/SectionContainer/Templates/RowSectionsContainerRuntimeSettingTemplate.html"
        };

        function GenericEditorDefinitionSetting($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectedValues;
            var allFieldValuesByName;
            var definitionSettings;
            var dataRecordTypeId;
            var historyId;
            var parentFieldValues;
            var genericContext;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.sectionContainers = [];

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    $scope.scopeModel.sectionContainers.length = 0;

                    if (payload != undefined) {
                        selectedValues = payload.selectedValues;
                        allFieldValuesByName = payload.allFieldValuesByName;
                        definitionSettings = payload.definitionSettings;
                        dataRecordTypeId = payload.dataRecordTypeId;
                        historyId = payload.historyId;
                        parentFieldValues = payload.parentFieldValues;
                        genericContext = payload.genericContext;
                    }

                    if (definitionSettings != undefined) {
                        if (definitionSettings.RowSectionContainers != undefined) {
                            for (var i = 0; i < definitionSettings.RowSectionContainers.length; i++) {
                                var sectionContainer = definitionSettings.RowSectionContainers[i];
                                var sectionContainerDef = {
                                    payload: sectionContainer,
                                    directiveReadyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                    directiveLoadPromiseDeferred: UtilsService.createPromiseDeferred()
                                };
                                addSectionContainer(sectionContainerDef);
                                promises.push(sectionContainerDef.directiveLoadPromiseDeferred.promise);
                            }
                        }
                    }

                    function addSectionContainer(sectionContainerDef) {
                        var sectionContainer = {
                            colNum: sectionContainerDef.payload.ColNum,
                            sectionTitle: sectionContainerDef.payload.SectionTitle,
                            runtimeEditor: sectionContainerDef.payload.SectionSettings.RuntimeEditor
                        };
                        sectionContainer.onEditorRuntimeDirectiveReady = function (api) {
                            sectionContainer.editorRuntimeAPI = api;
                            sectionContainerDef.directiveReadyPromiseDeferred.resolve();
                        };
                        sectionContainerDef.directiveReadyPromiseDeferred.promise.then(function () {
                            var directivePayload = {
                                dataRecordTypeId: dataRecordTypeId,
                                definitionSettings: sectionContainerDef.payload.SectionSettings,
                                selectedValues: selectedValues,
                                allFieldValuesByName: allFieldValuesByName,
                                historyId: historyId,
                                parentFieldValues: parentFieldValues,
                                genericContext: genericContext
                            };
                            VRUIUtilsService.callDirectiveLoad(sectionContainer.editorRuntimeAPI, directivePayload, sectionContainerDef.directiveLoadPromiseDeferred);
                        });
                        $scope.scopeModel.sectionContainers.push(sectionContainer);
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.setData = function (dicData) {
                    for (var i = 0; i < $scope.scopeModel.sectionContainers.length; i++) {
                        var sectionContainer = $scope.scopeModel.sectionContainers[i];
                        if (sectionContainer.editorRuntimeAPI != undefined) {
                            sectionContainer.editorRuntimeAPI.setData(dicData);
                        }
                    }
                };

                api.onFieldValueChanged = function (allFieldValuesByFieldNames, changedField) {
                    if ($scope.scopeModel.sectionContainers == undefined)
                        return null;

                    var _promises = [];

                    for (var i = 0; i < $scope.scopeModel.sectionContainers.length; i++) {
                        var currentSectionContainers = $scope.scopeModel.sectionContainers[i];
                        if (currentSectionContainers.editorRuntimeAPI != undefined && currentSectionContainers.editorRuntimeAPI.onFieldValueChanged != undefined && typeof (currentSectionContainers.editorRuntimeAPI.onFieldValueChanged) == "function") {
                            var onFieldValueChangedPromise = currentSectionContainers.editorRuntimeAPI.onFieldValueChanged(allFieldValuesByFieldNames, changedField);
                            if (onFieldValueChangedPromise != undefined)
                                _promises.push(onFieldValueChangedPromise);
                        }
                    }

                    return UtilsService.waitMultiplePromises(_promises);
                };

                api.setFieldValues = function (fieldValuesByNames) {
                    if ($scope.scopeModel.sectionContainers == undefined)
                        return null;

                    var _promises = [];

                    for (var i = 0; i < $scope.scopeModel.sectionContainers.length; i++) {
                        var currentSectionContainers = $scope.scopeModel.sectionContainers[i];
                        if (currentSectionContainers.editorRuntimeAPI.setFieldValues != undefined && typeof (currentSectionContainers.editorRuntimeAPI.setFieldValues) == "function") {
                            var onFieldValueSettedPromise = currentSectionContainers.editorRuntimeAPI.setFieldValues(fieldValuesByNames);
                            if (onFieldValueSettedPromise != undefined)
                                _promises.push(onFieldValueSettedPromise);
                        }
                    }

                    return UtilsService.waitMultiplePromises(_promises);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }
]);
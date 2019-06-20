(function (app) {

    'use strict';

    RowsContainerEditorRuntimeDirective.$inject = ['UtilsService', 'VRUIUtilsService'];

    function RowsContainerEditorRuntimeDirective(UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new RowsContainerEditorRuntimeCtor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericEditorDefinitionSetting/Templates/RowsContainerEditorRuntimeSettingTemplate.html'
        };

        function RowsContainerEditorRuntimeCtor($scope, ctrl) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.rowContainers = [];
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var selectedValues;
                    var allFieldValuesByName;
                    var definitionSettings;
                    var dataRecordTypeId;
                    var historyId;
                    var parentFieldValues;
                    var genericContext;

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
                        if (definitionSettings.RowContainers != undefined) {
                            for (var i = 0; i < definitionSettings.RowContainers.length; i++) {
                                var rowContainer = definitionSettings.RowContainers[i];
                                var rowContainerDef = {
                                    payload: rowContainer,
                                    rowContainerLoadPromiseDeferred: UtilsService.createPromiseDeferred()
                                };
                                promises.push(rowContainerDef.rowContainerLoadPromiseDeferred.promise);
                                addRowContainer(rowContainerDef);
                            }
                        }
                    }

                    function addRowContainer(rowContainerDef) {
                        var rowSettings = [];
                        var fieldsPromises = [];

                        for (var i = 0; i < rowContainerDef.payload.RowSettings.length; i++) {
                            var rowSetting = {
                                payload: rowContainerDef.payload.RowSettings[i],
                                directiveReadyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                directiveLoadPromiseDeferred: UtilsService.createPromiseDeferred()
                            };
                            fieldsPromises.push(rowSetting.directiveLoadPromiseDeferred.promise);
                            addFieldAPI(rowSetting);
                        }

                        function addFieldAPI(rowSetting) {
                            var rowItem = {
                                runtimeEditor: rowSetting.payload.RuntimeEditor
                            };
                            rowItem.onEditorRuntimeDirectiveReady = function (api) {
                                rowItem.editorRuntimeAPI = api;
                                rowSetting.directiveReadyPromiseDeferred.resolve();
                            };

                            rowSetting.directiveReadyPromiseDeferred.promise.then(function () {
                                var directivePayload = {
                                    dataRecordTypeId: dataRecordTypeId,
                                    definitionSettings: rowSetting.payload,
                                    selectedValues: selectedValues,
                                    allFieldValuesByName: allFieldValuesByName,
                                    historyId: historyId,
                                    parentFieldValues: parentFieldValues,
                                    genericContext: genericContext
                                };
                                VRUIUtilsService.callDirectiveLoad(rowItem.editorRuntimeAPI, directivePayload, rowSetting.directiveLoadPromiseDeferred);
                            });
                            rowSettings.push(rowItem);
                        }

                        UtilsService.waitMultiplePromises(fieldsPromises).then(function () {
                            rowContainerDef.rowContainerLoadPromiseDeferred.resolve();
                        });

                        $scope.scopeModel.rowContainers.push({ rowSettings: rowSettings });
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.setData = function (dicData) {
                    for (var i = 0; i < $scope.scopeModel.rowContainers.length; i++) {
                        var rowContainer = $scope.scopeModel.rowContainers[i];
                        for (var j = 0; j < rowContainer.rowSettings.length; j++) {
                            var rowSetting = rowContainer.rowSettings[j];
                            if (rowSetting.editorRuntimeAPI != undefined) {
                                rowSetting.editorRuntimeAPI.setData(dicData);
                            }
                        }
                    }
                };

                api.onFieldValueChanged = function (allFieldValuesByFieldNames) {
                    if ($scope.scopeModel.rowContainers == undefined)
                        return null;

                    var _promises = [];

                    for (var i = 0; i < $scope.scopeModel.rowContainers.length; i++) {
                        var rowContainer = $scope.scopeModel.rowContainers[i];
                        for (var j = 0; j < rowContainer.rowSettings.length; j++) {
                            var rowSetting = rowContainer.rowSettings[j];
                            if (rowSetting.editorRuntimeAPI != undefined && rowSetting.editorRuntimeAPI.onFieldValueChanged != undefined && typeof (rowSetting.editorRuntimeAPI.onFieldValueChanged) == "function") {
                                var onFieldValueChangedPromise = rowSetting.editorRuntimeAPI.onFieldValueChanged(allFieldValuesByFieldNames);
                                if (onFieldValueChangedPromise != undefined)
                                    _promises.push(onFieldValueChangedPromise);
                            }
                        }
                    }

                    return UtilsService.waitMultiplePromises(_promises);
                };

                api.setFieldValues = function (fieldValuesByNames) {
                    if ($scope.scopeModel.rowContainers == undefined)
                        return null;

                    var _promises = [];

                    for (var i = 0; i < $scope.scopeModel.rowContainers.length; i++) {
                        var rowContainer = $scope.scopeModel.rowContainers[i];
                        for (var j = 0; j < rowContainer.rowSettings.length; j++) {
                            var rowSetting = rowContainer.rowSettings[j];
                            if (rowSetting.editorRuntimeAPI.setFieldValues != undefined && typeof (rowSetting.editorRuntimeAPI.setFieldValues) == "function") {
                                var onSetFieldValuesPromise = rowSetting.editorRuntimeAPI.setFieldValues(fieldValuesByNames);
                                if (onSetFieldValuesPromise != undefined)
                                    _promises.push(onSetFieldValuesPromise);
                            }
                        }
                    }

                    return UtilsService.waitMultiplePromises(_promises);
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrGenericdataRowscontainereditorRuntime', RowsContainerEditorRuntimeDirective);

})(app);
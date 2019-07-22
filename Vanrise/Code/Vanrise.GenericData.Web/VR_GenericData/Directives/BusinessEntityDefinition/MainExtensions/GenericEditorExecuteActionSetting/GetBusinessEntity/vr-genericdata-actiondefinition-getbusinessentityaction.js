(function (app) {

    'use strict';

    CallRestAPIActionDefinitionDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_GenericBEDefinitionAPIService'];

    function CallRestAPIActionDefinitionDirective(UtilsService, VRUIUtilsService, VR_GenericData_GenericBEDefinitionAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new CallRestAPIActionDefinitionCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericEditorExecuteActionSetting/GetBusinessEntity/Templates/GetBusinessEntityExecuteActionDefinition.html'
        };

        function CallRestAPIActionDefinitionCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var beDefinitionSelectorAPI;
            var beDefinitionSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var beDefinitionSelectionChangedPromiseDeferred;

            var beFieldIdAPI;
            var beFieldIdSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.beDataRecordFields = [];
                $scope.scopeModel.outputFields = [];

                $scope.scopeModel.onBEDefinitionSelectorReady = function (api) {
                    beDefinitionSelectorAPI = api;
                    beDefinitionSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onBEFieldIdSelectorReady = function (api) {
                    beFieldIdAPI = api;
                    beFieldIdSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onBEDefinitionSelectionChanged = function (selectedValue) {
                    if (selectedValue != undefined) {
                        if (beDefinitionSelectionChangedPromiseDeferred != undefined) {
                            beDefinitionSelectionChangedPromiseDeferred.resolve();
                        }
                        else {
                            var setLoader = function (value) {
                                $scope.scopeModel.isLoadingSelectorFilter = value;
                            };
                            var selectorFilterEditorPayload = {
                                beDefinitionId: beDefinitionSelectorAPI.getSelectedIds()
                            };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, selectorFilterEditorAPI, selectorFilterEditorPayload, setLoader);
                        }
                    }
                };

                $scope.scopeModel.addOutputField = function () {
                    extendAndAddOutputFieldMappingToGrid();
                };

                $scope.scopeModel.isInputValid = function () {
                    var inputItems = inputItemsGridAPI != undefined ? inputItemsGridAPI.getData() : undefined;

                    if (inputItems != undefined && inputItems.length > 0) {
                        for (var i = 0; i < inputItems.length; i++) {
                            var firstInputItem = inputItems[i];
                            if (firstInputItem.PropertyName == undefined)
                                continue;

                            for (var j = i + 1; j < inputItems.length; j++) {
                                var secondInputItem = inputItems[j];
                                if (secondInputItem.PropertyName == undefined)
                                    continue;

                                if (firstInputItem.PropertyName == secondInputItem.PropertyName)
                                    return "Property Name should be Unique!";
                            }
                        }
                    }

                    return null;
                };

                $scope.scopeModel.isOutputValid = function () {
                    var outputItems = outputItemsGridAPI != undefined ? outputItemsGridAPI.getData() : undefined;

                    if (outputItems != undefined && outputItems.length > 0) {
                        for (var i = 0; i < outputItems.length; i++) {
                            var firstOutputItem = outputItems[i];
                            if (firstOutputItem.FieldName == undefined)
                                continue;

                            for (var j = i + 1; j < outputItems.length; j++) {
                                var secondOutputItem = outputItems[j];
                                if (secondOutputItem.FieldName == undefined)
                                    continue;

                                if (firstOutputItem.FieldName == secondOutputItem.FieldName)
                                    return "Field Name should be Unique!";
                            }
                        }
                    }

                    return null;
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var context;

                    var businessEntityDefinitionId;
                    var beFieldId;
                    var outputFieldsMapping;

                    if (payload != undefined) {
                        context = payload.context;

                        var settings = payload.settings;
                        if (settings != undefined) {
                            businessEntityDefinitionId = settings.BusinessEntityDefinitionId;
                            beFieldId = settings.BEFieldId;
                            outputFieldsMapping = settings.OutputFieldsMapping;
                        }
                    }

                    if (context != undefined) {
                        $scope.scopeModel.allDataRecordTypeFields = context.getFields();
                    }

                    function loadBEDefinitionSelector() {
                        var beDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                        beDefinitionSelectorReadyPromiseDeferred.promise.then(function () {

                            var beDefinitionSelectorPayload = {
                                filter: {
                                    Filters: [{
                                        $type: "Vanrise.GenericData.Business.GenericBEDefinitionViewFilter, Vanrise.GenericData.Business"
                                    }]
                                }
                            };

                            if (businessEntityDefinitionId != undefined) {
                                beDefinitionSelectorPayload.selectedIds = businessEntityDefinitionId;
                            }

                            VRUIUtilsService.callDirectiveLoad(beDefinitionSelectorAPI, beDefinitionSelectorPayload, beDefinitionSelectorLoadDeferred);
                        });

                        return beDefinitionSelectorLoadDeferred.promise;
                    }

                    function getOutputFieldsMappingLoadPromise() {
                        var loadOutputFieldsMappingPromiseDeferred = UtilsService.createPromiseDeferred();

                        gridPromiseDeferred.promise.then(function () {
                            var _promises = [];
                            for (var i = 0; i < outputFieldsMapping.length; i++) {
                                var outputFieldMapping = outputFieldsMapping[i];
                                _promises.push(extendAndAddOutputFieldMappingToGrid(outputFieldMapping));
                            }

                            UtilsService.waitMultiplePromises(_promises).then(function () {
                                loadOutputFieldsMappingPromiseDeferred.resolve();
                            });
                        });

                        return loadOutputFieldsMappingPromiseDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    var definitionData = {
                        $type: "Vanrise.GenericData.MainExtensions.GetBusinessEntityExecuteActionType, Vanrise.GenericData.MainExtensions",
                        BusinessEntityDefinitionId: beDefinitionSelectorAPI.getSelectedIds(),
                        BEFieldId: ctrl.selectedvalues != undefined ? ctrl.selectedvalues.value : undefined,
                        OutputFieldsMapping: getOutputFieldsMapping()
                    };

                    function getOutputFieldsMapping() {
                        return [];
                    }

                    return definitionData;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function loadBEDataRecordFields(businessEntityDefinitionId) {
                var beDataRecordFieldsLoadDeferred = UtilsService.createPromiseDeferred();
                $scope.scopeModel.beDataRecordFields.length = 0;

                VR_GenericData_GenericBEDefinitionAPIService.GetDataRecordTypeFieldsListByBEDefinitionId(businessEntityDefinitionId).then(function (recordFields) {
                    if (recordFields) {
                        for (var i = 0; i < recordFields.length; i++) {
                            $scope.scopeModel.beDataRecordFields.push({
                                fieldName: recordFields[i].Name,
                                fieldTitle: recordFields[i].Title,
                                fieldType: recordFields[i].Type
                            });
                        }
                    }
                    beDataRecordFieldsLoadDeferred.resolve();
                });

                return beDataRecordFieldsLoadDeferred.promise;
            }

            function extendAndAddOutputFieldMappingToGrid(outputFieldMapping) {

                var extendOptionPromises = [];

                var outputFieldMappingDataItem = { tempId: UtilsService.guid() };

                if (outputFieldMapping != undefined) {
                    outputFieldMappingDataItem.compatibleOutputMappedFieldsSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    outputFieldMappingDataItem.outputMappedFieldSelectionChangedPromiseDeferred = UtilsService.createPromiseDeferred();

                    extendOptionPromises.push(outputFieldMappingDataItem.compatibleOutputMappedFieldsSelectorLoadDeferred.promise);
                }

                //Loading OutputFieldsSelector
                outputFieldMappingDataItem.outputMappedFieldsSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                outputFieldMappingDataItem.onOutputFieldsSelectorReady = function (api) {
                    outputFieldMappingDataItem.outputMappedFieldsSelectorAPI = api;
                    outputFieldMappingDataItem.outputMappedFieldsSelectorReadyPromiseDeferred.resolve();

                    if (outputFieldMapping != undefined && outputFieldMapping.OutputFieldName != undefined) {
                        var selectedValue = UtilsService.getItemByVal($scope.scopeModel.allDataRecordTypeFields, outputFieldMapping.OutputFieldName, "FieldName");
                        if (selectedValue != null)
                            outputFieldMappingDataItem.selectedField = selectedValue;
                    }
                };

                outputFieldMappingDataItem.onDataRecordFieldChanged = function (selectedDataRecordField) {
                    if (selectedDataRecordField == undefined)
                        return;

                    if (outputFieldMappingDataItem.outputMappedFieldSelectionChangedPromiseDeferred != undefined) {
                        outputFieldMappingDataItem.outputMappedFieldSelectionChangedPromiseDeferred.resolve();
                        return;
                    }

                    var compatibleOutputMappedFieldsSelectorPayload = {
                        entityDefinitionId: businessEntityDefinitionId,
                        dataRecordFieldType: selectedDataRecordField.fieldType
                    };
                    var setLoader = function (value) {
                        outputFieldMappingDataItem.isLoadingCompatibleOutputMappedFieldsSelector = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, outputFieldMappingDataItem.compatibleOutputMappedFieldsSelectorAPI, compatibleOutputMappedFieldsSelectorPayload, setLoader);
                };

                //Loading CompatibleOutputMappedFieldsSelector
                outputFieldMappingDataItem.onCompatibleOutputMappedFieldsSelectorReady = function (api) {
                    outputFieldMappingDataItem.compatibleOutputMappedFieldsSelectorAPI = api;

                    if (outputFieldMapping == undefined)
                        return;

                    outputFieldMappingDataItem.outputMappedFieldSelectionChangedPromiseDeferred.promise.then(function () {
                        outputFieldMappingDataItem.outputMappedFieldSelectionChangedPromiseDeferred = undefined;

                        var compatibleOutputMappedFieldsSelectorPayload = {
                            entityDefinitionId: beDefinitionId,
                            dataRecordFieldType: outputFieldMappingDataItem.selectedField.fieldType
                        };
                        if (outputFieldMapping.OutputMappedFieldName != undefined) {
                            compatibleOutputMappedFieldsSelectorPayload.selectedIds = outputFieldMapping.OutputMappedFieldName;
                        }
                        VRUIUtilsService.callDirectiveLoad(outputFieldMappingDataItem.compatibleOutputMappedFieldsSelectorAPI, compatibleOutputMappedFieldsSelectorPayload, outputFieldMappingDataItem.compatibleOutputMappedFieldsSelectorLoadDeferred);
                    });
                };

                $scope.scopeModel.outputMappedFields.push(outputFieldMappingDataItem);

                return UtilsService.waitMultiplePromises(extendOptionPromises);
            }
        }
    }

    app.directive('vrGenericdataExecuteactioneditorsettingActiondefinitionGetbusinessentityaction', CallRestAPIActionDefinitionDirective);

})(app);
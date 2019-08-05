﻿'use strict';

app.directive('vrGenericdataBusinessentitydefinitionDependantfieldGrid', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new DependantFieldGridCtor(ctrl, $scope);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: function (element, attrs) {
                return '/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/Templates/BEDependantFieldGridTemplate.html';
            }
        };

        function DependantFieldGridCtor(ctrl, $scope) {
            this.initializeController = initializeController;

            var beDependantFields;
            var beDefinitionId;
            var context;

            var gridAPI;
            var gridPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.dependantFields = [];
                $scope.scopeModel.allDataRecordTypeFields = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    gridPromiseDeferred.resolve();
                };

                $scope.scopeModel.onAddDependantFieldClicked = function () {
                    extendAndAddDependantFieldToGrid();
                };

                $scope.scopeModel.onDeleteRow = function (deletedItem) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.dependantFields, deletedItem.tempId, 'tempId');
                    $scope.scopeModel.dependantFields.splice(index, 1);
                };

                $scope.scopeModel.isValid = function () {
                    var dependantFields = $scope.scopeModel.dependantFields;

                    for (var i = 0; i < dependantFields.length; i++) {
                        var currentDependentField = dependantFields[i];
                        if (currentDependentField.selectedField == undefined)
                            continue;

                        for (var j = 0; j < dependantFields.length; j++) {
                            var dependantField = dependantFields[j];
                            if (dependantField.selectedField == undefined || i == j)
                                continue;

                            if (currentDependentField.selectedField.fieldName == dependantField.selectedField.fieldName)
                                return "Same Field Exist!";
                        }
                    }

                    return null;
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    $scope.scopeModel.dependantFields.length = 0;
                    $scope.scopeModel.allDataRecordTypeFields.length = 0;

                    var promises = [];

                    if (payload != undefined) {
                        beDependantFields = payload.beDependantFields;
                        beDefinitionId = payload.beDefinitionId;
                        context = payload.context;
                    }

                    if (context != undefined) {
                        $scope.scopeModel.allDataRecordTypeFields = context.getFields();
                    }

                    if (beDependantFields != undefined && beDependantFields.length > 0) {
                        var loadDependantFieldGridPromise = getDependantFieldsLoadPromise();
                        promises.push(loadDependantFieldGridPromise);
                    }

                    function getDependantFieldsLoadPromise() {
                        var loadDependantFieldsPromiseDeferred = UtilsService.createPromiseDeferred();

                        gridPromiseDeferred.promise.then(function () {
                            var _promises = [];
                            for (var i = 0; i < beDependantFields.length; i++) {
                                var currentDependantField = beDependantFields[i];
                                _promises.push(extendAndAddDependantFieldToGrid(currentDependantField));
                            }

                            UtilsService.waitMultiplePromises(_promises).then(function () {
                                loadDependantFieldsPromiseDeferred.resolve();
                            });
                        });

                        return loadDependantFieldsPromiseDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var fieldBusinessEntityTypeDependantFields = [];

                    for (var i = 0; i < $scope.scopeModel.dependantFields.length; i++)
                        fieldBusinessEntityTypeDependantFields.push(buildFieldBusinessEntityTypeDependantField($scope.scopeModel.dependantFields[i]));

                    return fieldBusinessEntityTypeDependantFields;
                };

                if (ctrl.onReady != null && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(api);
            }

            function extendAndAddDependantFieldToGrid(dependantField) {

                var extendOptionPromises = [];

                var dependantFieldDataItem = { tempId: UtilsService.guid() };

                if (dependantField != undefined) {
                    dependantFieldDataItem.IsRequired = dependantField.IsRequired;
                    dependantFieldDataItem.compatibleFieldsSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    dependantFieldDataItem.dependantFieldSelectionChangedPromiseDeferred = UtilsService.createPromiseDeferred();

                    //extendOptionPromises.push(dependantFieldDataItem.compatibleFieldsSelectorLoadDeferred.promise);
                }


                //Loading DependantFieldsSelector
                dependantFieldDataItem.dependantFieldsSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                dependantFieldDataItem.onDependantFieldsSelectorReady = function (api) {
                    dependantFieldDataItem.dependantFieldsSelectorAPI = api;
                    dependantFieldDataItem.dependantFieldsSelectorReadyPromiseDeferred.resolve();
                    var dependantFieldSelectorPayload = {
                        context: context,
                    };
                    if (dependantField != undefined) {
                        dependantFieldSelectorPayload.selectedIds = dependantField.FieldName;
                    }
                    VRUIUtilsService.callDirectiveLoad(dependantFieldDataItem.dependantFieldsSelectorAPI, dependantFieldSelectorPayload, undefined);
                    //if (dependantField != undefined && dependantField.FieldName != undefined) {
                    //    var selectedValue = UtilsService.getItemByVal(dependantFieldDataItem.dependantFieldsSelectorAPI.getData(), dependantField.FieldName, "fieldName");
                    //    if (selectedValue != null)
                    //dependantFieldDataItem.selectedField = dependantFieldDataItem.dependantFieldsSelectorAPI.getSelectedIds();
                    //}
                };

                dependantFieldDataItem.onDataRecordFieldChanged = function (selectedDataRecordField) {
                    if (selectedDataRecordField == undefined) {
                        if (dependantFieldDataItem.compatibleFieldsSelectorAPI != undefined && dependantFieldDataItem.compatibleFieldsSelectorAPI.getSelectedValues()!=undefined)
                            dependantFieldDataItem.compatibleFieldsSelectorAPI.clearData();
                        return;
                    }
                if (dependantFieldDataItem.dependantFieldSelectionChangedPromiseDeferred != undefined) {
                    dependantFieldDataItem.dependantFieldSelectionChangedPromiseDeferred.resolve();
                    return;
                    }
                var compatibleFieldsSelectorPayload = {
                    entityDefinitionId: beDefinitionId,
                    dataRecordFieldType: selectedDataRecordField.fieldType,
                    selectedIds: dependantFieldDataItem.compatibleFieldsSelectorAPI.getSelectedIds()
                };
                var setLoader = function (value) {
                        dependantFieldDataItem.isLoadingCompatibleFieldsSelector = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dependantFieldDataItem.compatibleFieldsSelectorAPI, compatibleFieldsSelectorPayload, setLoader);
                };

                //Loading CompatibleFieldsSelector 
                dependantFieldDataItem.onCompatibleFieldsSelectorReady = function (api) {
                    dependantFieldDataItem.compatibleFieldsSelectorAPI = api;

                    if (dependantField == undefined)
                        return;

                    dependantFieldDataItem.dependantFieldSelectionChangedPromiseDeferred.promise.then(function () {
                        dependantFieldDataItem.dependantFieldSelectionChangedPromiseDeferred = undefined;
                        var compatibleFieldsSelectorPayload = {
                            entityDefinitionId: beDefinitionId,
                            dataRecordFieldType: dependantFieldDataItem.dependantFieldsSelectorAPI.getSelectedValues().fieldType
                        };
                        if (dependantField.MappedFieldName != undefined) {
                            compatibleFieldsSelectorPayload.selectedIds = dependantField.MappedFieldName;
                        }
                        VRUIUtilsService.callDirectiveLoad(dependantFieldDataItem.compatibleFieldsSelectorAPI, compatibleFieldsSelectorPayload, dependantFieldDataItem.compatibleFieldsSelectorLoadDeferred);
                    });
                };

                $scope.scopeModel.dependantFields.push(dependantFieldDataItem);

                return UtilsService.waitMultiplePromises(extendOptionPromises);
            }

            function buildFieldBusinessEntityTypeDependantField(dependantFieldDataItem) {
                return {
                    FieldName: dependantFieldDataItem.dependantFieldsSelectorAPI != undefined ? dependantFieldDataItem.dependantFieldsSelectorAPI.getSelectedIds() : null,
                    MappedFieldName: (dependantFieldDataItem.compatibleFieldsSelectorAPI != undefined) ? dependantFieldDataItem.compatibleFieldsSelectorAPI.getSelectedIds():null,
                    IsRequired: dependantFieldDataItem.IsRequired
                };
            }
        }

        return directiveDefinitionObject;
    }]);
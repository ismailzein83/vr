"use strict";

app.directive("vrGenericdataGenericbeBulkactiondefinitionUpdate", ["UtilsService", "VRUIUtilsService", "VRNotificationService", "VR_GenericData_GenericBEDefinitionService",
    function (UtilsService, VRUIUtilsService, VRNotificationService, VR_GenericData_GenericBEDefinitionService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new GenericBEBulkActionUpdate($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: '/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Definition/GenericBEBulkActionDefinition/MainExtensions/Templates/GenericBEBulkActionUpdateTemplate.html'

        };

        function GenericBEBulkActionUpdate($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var genericBEDefinitionId;
            var gridAPI;
            var fieldSelectedPromiseDeferred;
            var dataRecordTypeFieldsSelectorAPI;
            var dataRecordTypeFieldsSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var loadDataRecordTypeFieldsSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
            var context;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.selectedFields = [];
                ctrl.datasource = [];
                $scope.scopeModel.onDataRecordTypeFieldsSelectorDirectiveReady = function (api) {
                    dataRecordTypeFieldsSelectorAPI = api;
                    dataRecordTypeFieldsSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onFieldSelected = function (item) {
                    var entity = {
                        data: { FieldName: item.Name },
                        runtimeEditor: item.Type.RuntimeEditor
                    };
                    entity.onFieldStateDirectiveReady = function (api) {
                        entity.fieldStateDirectiveAPI = api;
                        var setLoader = function (value) { $scope.scopeModel.isFieldStateDirectiveloading = value; };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, api, undefined, setLoader);
                    };
                    entity.onRunTimeEditorDirectiveReady = function (api) {
                        entity.runtimeDirectiveAPI = api;
                        var payload = { fieldTitle: item.Title, fieldType: item.Type };
                        var setLoader = function (value) { $scope.scopeModel.isRuntimeEditorDirectiveloading = value; };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, api, payload, setLoader);
                    };
                    ctrl.datasource.push(entity);
                };

                $scope.scopeModel.onFieldDeselected = function (item) {
                    var index = UtilsService.getItemIndexByVal(ctrl.datasource, item.Name, "data.FieldName");
                    ctrl.datasource.splice(index, 1);
                };

                $scope.scopeModel.onDeleteField = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    var selectedFieldIndex = UtilsService.getItemIndexByVal($scope.scopeModel.selectedFields, dataItem.data.FieldName, "Name");
                    $scope.scopeModel.selectedFields.splice(selectedFieldIndex, 1);
                    ctrl.datasource.splice(index, 1);
                };

                $scope.scopeModel.deselectAllFields = function () {
                    ctrl.datasource.length = 0;
                };

                $scope.scopeModel.onFieldSelectionChange = function (value) {
                    if (value) {
                        if (fieldSelectedPromiseDeferred != undefined) {
                            fieldSelectedPromiseDeferred.resolve();
                        }
                    }
                };
                defineAPI();
            }


            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var selectedIds;
                    context = payload.context;
                    if (payload != undefined && payload.bulkActionSettings && payload.bulkActionSettings.GenericBEFields != undefined) {
                        fieldSelectedPromiseDeferred = UtilsService.createPromiseDeferred();
                        selectedIds = [];
                        for (var i = 0; i < payload.bulkActionSettings.GenericBEFields.length; i++) {
                            var field = payload.bulkActionSettings.GenericBEFields[i];
                            selectedIds.push(field.FieldName);
                        }
                    }
                    dataRecordTypeFieldsSelectorReadyPromiseDeferred.promise.then(function () {
                        var fieldsPayload = {
                            dataRecordTypeId: context.getDataRecordTypeId(),
                            selectedIds: selectedIds
                        }; 
                        VRUIUtilsService.callDirectiveLoad(dataRecordTypeFieldsSelectorAPI, fieldsPayload, loadDataRecordTypeFieldsSelectorPromiseDeferred);
                    });

                    promises.push(loadDataRecordTypeFieldsSelectorPromiseDeferred.promise);
                    if (payload != undefined && payload.bulkActionSettings && payload.bulkActionSettings.GenericBEFields != undefined) {
                        for (var i = 0; i < payload.bulkActionSettings.GenericBEFields.length; i++) {
                            var genericBEField = payload.bulkActionSettings.GenericBEFields[i];
                            var genericBEFieldObject = {
                                payload: genericBEField,
                                fieldStateReadyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                fieldStateLoadPromiseDeferred: UtilsService.createPromiseDeferred(),
                                runtimeFieldReadyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                runtimeFieldLoadPromiseDeferred: UtilsService.createPromiseDeferred()
                            };
                            promises.push(genericBEFieldObject.fieldStateLoadPromiseDeferred.promise);
                            promises.push(genericBEFieldObject.runtimeFieldLoadPromiseDeferred.promise);
                            prepareDataItem(genericBEFieldObject);
                        }
                    }


                    return UtilsService.waitMultiplePromises(promises);
                };

                function prepareDataItem(genericBEFieldObject) {

                    var entity = {
                        data: genericBEFieldObject.payload
                    };
                    entity.onFieldStateDirectiveReady = function (api) {
                        entity.fieldStateDirectiveAPI = api;
                        genericBEFieldObject.fieldStateReadyPromiseDeferred.resolve();
                    };
                    entity.onRunTimeEditorDirectiveReady = function (api) {
                        entity.runtimeDirectiveAPI = api;
                        genericBEFieldObject.runtimeFieldReadyPromiseDeferred.resolve();
                    };

                    genericBEFieldObject.fieldStateReadyPromiseDeferred.promise.then(function () {
                        var payloadFieldState = { selectedIds: genericBEFieldObject.payload.FieldState }; 
                        VRUIUtilsService.callDirectiveLoad(entity.fieldStateDirectiveAPI, payloadFieldState, genericBEFieldObject.fieldStateLoadPromiseDeferred);
                    });
                    fieldSelectedPromiseDeferred.promise.then(function () {
                        var item = UtilsService.getItemByVal($scope.scopeModel.selectedFields, genericBEFieldObject.payload.FieldName, "Name"); 
                        entity.runtimeEditor = item.Type.RuntimeEditor;
                        genericBEFieldObject.runtimeFieldReadyPromiseDeferred.promise.then(function () {

                            var payloadRuntimeEditor = {
                                fieldTitle: item.Title,
                                fieldValue: genericBEFieldObject.payload.DefaultValue,
                                fieldType: item.Type
                            };
                            VRUIUtilsService.callDirectiveLoad(entity.runtimeDirectiveAPI, payloadRuntimeEditor, genericBEFieldObject.runtimeFieldLoadPromiseDeferred);
                        });
                    });
                    ctrl.datasource.push(entity);
                }

                api.getData = function () {
                    var genericBEFields;
                    if (ctrl.datasource != undefined) {
                        genericBEFields = [];
                        for (var i = 0; i < ctrl.datasource.length; i++) {
                            var currentItem = ctrl.datasource[i];
                            currentItem.data.FieldState = currentItem.fieldStateDirectiveAPI.getSelectedIds();
                            currentItem.data.DefaultValue = currentItem.runtimeDirectiveAPI.getData();
                            genericBEFields.push(currentItem.data);
                        }
                    }
                    return {
                        $type: "Vanrise.GenericData.MainExtensions.UpdateGenericBEBulkAction,Vanrise.GenericData.MainExtensions",
                        GenericBEFields: genericBEFields
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

        }

        return directiveDefinitionObject;

    }
]);
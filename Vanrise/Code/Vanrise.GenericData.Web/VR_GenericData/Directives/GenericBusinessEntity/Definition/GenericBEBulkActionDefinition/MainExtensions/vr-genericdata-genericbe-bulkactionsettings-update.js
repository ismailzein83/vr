"use strict";

app.directive("vrGenericdataGenericbeBulkactionsettingsUpdate", ["UtilsService", "VRUIUtilsService", "VRNotificationService", "VR_GenericData_GenericBEDefinitionService", "VR_GenericData_GenericBEDefinitionAPIService","VR_GenericData_GenericBEBulkActionUpdateFieldStateEnum",
    function (UtilsService, VRUIUtilsService, VRNotificationService, VR_GenericData_GenericBEDefinitionService, VR_GenericData_GenericBEDefinitionAPIService, VR_GenericData_GenericBEBulkActionUpdateFieldStateEnum) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new GenericBEBulkActionSettingsUpdate($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: '/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Definition/GenericBEBulkActionDefinition/MainExtensions/Templates/GenericBEBulkActionSettingsUpdateTemplate.html'

        };

        function GenericBEBulkActionSettingsUpdate($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
          
            var recordFieldTypes;
            $scope.scopeModel = {};
            function initializeController() {
                ctrl.datasource = [];
                $scope.scopeModel.fields = [];
                $scope.scopeModel.fieldStates = UtilsService.getArrayEnum(VR_GenericData_GenericBEBulkActionUpdateFieldStateEnum);
                $scope.scopeModel.hideField = function (fieldState) {
                    for (var i = 0; i < $scope.scopeModel.fieldStates.length; i++) {
                        var state = $scope.scopeModel.fieldStates[i];
                        if (fieldState == VR_GenericData_GenericBEBulkActionUpdateFieldStateEnum.NotVisible.value) {
                            return true;
                        }
                    }
                    return false;
                };
                $scope.scopeModel.disableField = function (fieldState) {
                    for (var i = 0; i < $scope.scopeModel.fieldStates.length; i++) {
                        var state = $scope.scopeModel.fieldStates[i];
                        if (fieldState == VR_GenericData_GenericBEBulkActionUpdateFieldStateEnum.ReadOnly.value) {
                            return true;
                        }
                    }
                    return false;
                };
                defineAPI();
            }
            function defineAPI() {
                var api = {};
                api.load = function (payload) {
                    var loadFieldsPromise = UtilsService.createPromiseDeferred();
                    if (payload != undefined && payload.dataRecordTypeId && payload.bulkAction != undefined && payload.bulkAction.Settings != undefined && payload.bulkAction.Settings.GenericBEFields != undefined && payload.bulkAction.Settings.GenericBEFields.length != 0) {
                        VR_GenericData_GenericBEDefinitionAPIService.GetUpdateBulkActionGenericBEFieldsRuntime({
                            DataRecordTypeId: payload.dataRecordTypeId,
                            Fields: payload.bulkAction.Settings.GenericBEFields
                        }).then(function (response) {
                            var promises = [];
                            for (var j = 0; j < response.length; j++) {
                                var field = response[j];
                                var fieldObject = {
                                    payload: field,
                                    readyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                    loadPromiseDeferred: UtilsService.createPromiseDeferred()
                                };
                                promises.push(fieldObject.loadPromiseDeferred.promise);
                                prepareField(fieldObject);
                            }
                            UtilsService.waitMultiplePromises(promises).then(function () {
                                loadFieldsPromise.resolve();
                            });
                            })
                            .catch(function (error) {
                                loadFieldsPromise.reject();
                                VRNotificationService.notifyException(error, $scope);
                            });
                    }
                    else loadFieldsPromise.resolve();
                    return loadFieldsPromise.promise;
                };

                function prepareField(fieldObject) {
                    var field = {
                        IsRequired: fieldObject.payload.IsRequired,
                        RuntimeEditor: fieldObject.payload.RuntimeEditor,
                        FieldState: fieldObject.payload.FieldState
                    };
                    field.onRunTimeEditorDirectiveReady = function (api) {
                        field.directiveAPI = api;
                        fieldObject.readyPromiseDeferred.resolve();
                    };
                    fieldObject.readyPromiseDeferred.promise.then(function () {
                        var payload = {
                            fieldTitle: fieldObject.payload.Title,
                            fieldValue: fieldObject.payload.DefaultValue,
                            fieldType: fieldObject.payload.Type
                        };
                        VRUIUtilsService.callDirectiveLoad(field.directiveAPI, payload, fieldObject.loadPromiseDeferred);
                    });
                    $scope.scopeModel.fields.push(field);
                }

                api.getData = function () {
                    var fieldValues = {};
                    for (var i = 0; i < $scope.scopeModel.fields.length; i++) {
                        var field = $scope.scopeModel.fields[i];
                        fieldValues[field.Name] = field.directiveAPI.getData();
                    }
                    return {
                        $type:"Vanrise.GenericData.MainExtensions.UpdateGenericBEBulkActionRuntimeSettings,Vanrise.GenericData.MainExtensions",
                        FieldValues: fieldValues
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

        }

        return directiveDefinitionObject;

    }
]);
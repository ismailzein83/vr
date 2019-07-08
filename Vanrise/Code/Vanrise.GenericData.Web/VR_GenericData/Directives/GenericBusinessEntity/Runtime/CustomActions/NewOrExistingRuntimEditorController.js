(function (appControllers) {
    "use strict";
    NewOrExistingRuntimEditorController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'VR_GenericData_GenericBusinessEntityAPIService', 'VR_GenericData_GenericBusinessEntityService'];

    function NewOrExistingRuntimEditorController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, VR_GenericData_GenericBusinessEntityAPIService, VR_GenericData_GenericBusinessEntityService) {

        var runtimeEditorAPI;
        var runtimeEditorReadyDeferred = UtilsService.createPromiseDeferred();

        $scope.scopeModel = {};

        var customAction;
        var businessEntityDefinitionId;
        var parentFieldValues;
        var context;
        var dataRecordTypeId;
        var customActionSettings;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                dataRecordTypeId = parameters.dataRecordTypeId;
                parentFieldValues = parameters.parentFieldValues;
                customAction = parameters.customAction;
                context = parameters.context;
                businessEntityDefinitionId = parameters.businessEntityDefinitionId;
            }
            customActionSettings = customAction.Settings != undefined ? customAction.Settings : {};
        }

        function defineScope() {

            $scope.scopeModel.onEditorRuntimeDirectiveReady = function (api) {
                runtimeEditorAPI = api;
                runtimeEditorReadyDeferred.resolve();
            };
            $scope.scopeModel.getBEentity = function () {
                var fieldValues = {};
                runtimeEditorAPI.setData(fieldValues);

                $scope.scopeModel.isLoading = true;

                var filter = buildFilter(fieldValues);
                var promiseDeferred = UtilsService.createPromiseDeferred();
                var message;
                VR_GenericData_GenericBusinessEntityAPIService.GetGenericBusinessEntityInfo(businessEntityDefinitionId, UtilsService.serializetoJson(filter))
                    .then(function (response) {
                        if (response != undefined && response.length != 0) {
                            message = "A customer with same name exist. Do you want to open the existing customer?";
                            var filter = {};
                            if (response.length == 1) {
                                var firstEntityId = response[0].GenericBusinessEntityId;
                                filter.ID = [firstEntityId];
                            }
                            else {
                                message = response.length + " customers with same name exist. Do you want to select from the existing customers?";
                                var ids = [];
                                for (var i = 0; i < response.length; i++) {
                                    var entityId = response[i].GenericBusinessEntityId;
                                    ids.push(entityId);
                                }
                                filter.ID = ids;

                            }
                            context.trigerSearch(filter);

                            VRNotificationService.showConfirmation(message).then(function (confirmed) {
                                $scope.scopeModel.isLoading = false;
                                if (confirmed) {
                                    if (response.length == 1) {
                                        var firstEntityId = response[0].GenericBusinessEntityId;
                                        context.expendRow(firstEntityId);
                                    }
                                    else {
                                        promiseDeferred.resolve();
                                    }
                                    $scope.modalContext.closeModal();
                                }
                                else {
                                    promiseDeferred.resolve();
                                    $scope.modalContext.closeModal();
                                    showAdd(fieldValues);
                                }
                            });
                        }
                        else {
                            $scope.modalContext.closeModal();
                            promiseDeferred.resolve();
                            showAdd(fieldValues);
                        }
                    }).catch(function (error) {
                        $scope.scopeModel.isLoading = false;
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                        promiseDeferred.reject();
                    });
                return promiseDeferred.promise;
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function loadEditorRuntimeDirective() {
            var loadEditorRuntimeDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

            runtimeEditorReadyDeferred.promise.then(function () {
                var runtimeEditorPayload = {
                    parentFieldValues: parentFieldValues,
                    dataRecordTypeId: dataRecordTypeId,
                    definitionSettings: customActionSettings.EditorDefinitionSetting,
                    runtimeEditor: customActionSettings.EditorDefinitionSetting != undefined ? customActionSettings.EditorDefinitionSetting.RuntimeEditor : undefined,
                    isEditMode: false
                };
                VRUIUtilsService.callDirectiveLoad(runtimeEditorAPI, runtimeEditorPayload, loadEditorRuntimeDirectivePromiseDeferred);
            });
            return loadEditorRuntimeDirectivePromiseDeferred.promise;
        }
        function showAdd(runntimeEditorFieldValues) {
            var fieldValues = buildFieldValues(runntimeEditorFieldValues);
            VR_GenericData_GenericBusinessEntityService.addGenericBusinessEntity(context.onGenericBEAdded, businessEntityDefinitionId, "large", fieldValues);
        }
        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }
        function loadAllControls() {
            $scope.title = "Add";
            return UtilsService.waitPromiseNode({ promises: [loadEditorRuntimeDirective()] }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function buildFieldValues(runntimeEditorFieldValues) {

            var fieldValue = {};
            var keys = Object.keys(runntimeEditorFieldValues);

            for (var i = 0; i < keys.length; i++) {
                var key = keys[i];
                fieldValue[key] = {};
                fieldValue[key].value = runntimeEditorFieldValues[key];
            }
            return fieldValue;
        }

        function buildFilter(runntimeEditorFieldValues) {

            var filters = [];
            var keys = Object.keys(runntimeEditorFieldValues);
            for (var i = 0; i < keys.length; i++) {
                var key = keys[i];
                var object =
                {
                    FieldName: key,
                    FilterValues: [runntimeEditorFieldValues[key]]
                };
                filters.push(object);
            }
            var filter =
            {
                FieldFilters: filters
            };
            return filter;
        }
    }
    appControllers.controller('VR_GenericData_NewOrExistingRuntimeEditorController', NewOrExistingRuntimEditorController);
})(appControllers);
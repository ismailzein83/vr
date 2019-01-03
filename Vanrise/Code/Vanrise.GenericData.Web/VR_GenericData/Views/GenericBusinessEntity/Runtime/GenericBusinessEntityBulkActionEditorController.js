(function (appControllers) {

    'use strict';

    GenericBusinessEntityBulkActionsController.$inject = ['$scope', 'VR_GenericData_GenericBusinessEntityService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService', 'VR_GenericData_GenericBEDefinitionAPIService', 'VR_GenericData_RecordQueryLogicalOperatorEnum', 'VR_GenericData_GenericBusinessEntityAPIService','BusinessProcess_BPInstanceService'];

    function GenericBusinessEntityBulkActionsController($scope, VR_GenericData_GenericBusinessEntityService, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, VR_GenericData_GenericBEDefinitionAPIService, VR_GenericData_RecordQueryLogicalOperatorEnum, VR_GenericData_GenericBusinessEntityAPIService, BusinessProcess_BPInstanceService) {

        var viewId;
        var businessEntityDefinitionId;
        var bulkAction;
        var bulkActionDraftFinalState;
        var dataRecordTypeId;
        var genericBERuntimeManagementDirectiveAPI;
        var genericBERuntimeManagementDirectiveReadyDeferred = UtilsService.createPromiseDeferred();
        var bulkActionRuntimeDirectiveAPI;
        var bulkActionRuntimeDirectiveReadyDeferred = UtilsService.createPromiseDeferred();
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                viewId = parameters.viewId;
                businessEntityDefinitionId = parameters.businessEntityDefinitionId;
                bulkAction = parameters.bulkAction;
                dataRecordTypeId = parameters.dataRecordTypeId;
            }
        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.onGenericBERuntimeManagementDirectiveReady = function (api) {
                genericBERuntimeManagementDirectiveAPI = api;
                genericBERuntimeManagementDirectiveReadyDeferred.resolve();
            };
            $scope.scopeModel.onBulkActionRuntimeDirectiveReady = function (api) {
                bulkActionRuntimeDirectiveAPI = api;
                bulkActionRuntimeDirectiveReadyDeferred.resolve();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

            $scope.scopeModel.execute = function () {

                var bulkActionDraftFinalStatePromise = genericBERuntimeManagementDirectiveAPI.finalizeBulkActionDraft();

                bulkActionDraftFinalStatePromise.then(function (bulkActionDraftFinalState) {

                    var executeGenericBEBulkActionProcessInput = {
                        GenericBEDefinitionId: businessEntityDefinitionId,
                        GenericBEBulkActions: [{
                            GenericBEBulkActionId: bulkAction.GenericBEBulkActionId,
                            Settings: bulkActionRuntimeDirectiveAPI.getData()
                        }],
                        HandlingErrorOption: undefined,
                        BulkActionFinalState: bulkActionDraftFinalState
                    };
                    return VR_GenericData_GenericBusinessEntityAPIService.ExecuteGenericBEBulkActions(executeGenericBEBulkActionProcessInput).then(function (response) {
                        if (response.Succeed) {
                            $scope.modalContext.closeModal();
                            BusinessProcess_BPInstanceService.openProcessTracking(response.ProcessInstanceId);
                        }
                        else {
                            VRNotificationService.showError(response.OutputMessage, $scope);
                        }
                    });
                });
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
            function loadAllControls() {

                function loadGenericBERuntimeManagementDirective() {
                    var genericBERuntimeManagementDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                    genericBERuntimeManagementDirectiveReadyDeferred.promise.then(function () {
                        var payload = {
                            viewId: viewId,
                            businessEntityDefinitionId: businessEntityDefinitionId,
                            bulkAction: bulkAction
                        };

                        VRUIUtilsService.callDirectiveLoad(genericBERuntimeManagementDirectiveAPI, payload, genericBERuntimeManagementDirectiveLoadDeferred);
                    });
                    return genericBERuntimeManagementDirectiveLoadDeferred.promise;
                }

                function setTitle() {
                    $scope.title = 'Bulk Action: ' + bulkAction.Title;
                }


                function loadBulkActionRuntimeDirective() {
                    var bulkActionRuntimeDirectiveLoadDeferred = UtilsService.createPromiseDeferred();
                    bulkActionRuntimeDirectiveReadyDeferred.promise.then(function () {
                        var payload = {
                            dataRecordTypeId: dataRecordTypeId,
                            bulkAction: bulkAction
                        };
                        VRUIUtilsService.callDirectiveLoad(bulkActionRuntimeDirectiveAPI, payload, bulkActionRuntimeDirectiveLoadDeferred);
                    });
                    return bulkActionRuntimeDirectiveLoadDeferred.promise;
                }

                return UtilsService.waitMultipleAsyncOperations([loadGenericBERuntimeManagementDirective, loadBulkActionRuntimeDirective, setTitle]).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
            }
        }

    }

    appControllers.controller('VR_GenericData_BulkActionsEditorController', GenericBusinessEntityBulkActionsController);

})(appControllers);
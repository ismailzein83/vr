(function (appControllers) {

    "use strict";

    CodePreparationPreviewController.$inject = ['$scope', 'BusinessProcess_BPTaskAPIService', 'WhS_CP_PreviewChangeTypeEnum', 'WhS_CP_PreviewGroupedBy', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService','WhS_BE_SalePriceListChangeService'];

    function CodePreparationPreviewController($scope, BusinessProcess_BPTaskAPIService, WhS_CP_PreviewChangeTypeEnum, WhS_CP_PreviewGroupedBy, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, WhS_BE_SalePriceListChangeService) {
        var bpTaskId;
        var processInstanceId;
        var changeType = true;

        var validationMessageHistoryGridAPI;
        var validationMessageHistoryReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var directiveWrapperAPI;
        var directiveWrapperReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var viewChangeTypeSelectorAPI;
        var viewChangeTypeReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var groupedBySelectorAPI;
        var groupedByReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();

        defineScope();

        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters !== undefined && parameters !== null) {
                bpTaskId = parameters.TaskId;
            }
        }

        function defineScope() {

            $scope.scopeModal = {};

            $scope.scopeModal.continueTask = function () {
                return executeTask(true);
            };
            $scope.scopeModal.openCheckPriceListPreview = function () {
                WhS_BE_SalePriceListChangeService.openCheckPriceListPreview(processInstanceId);
            };
            $scope.scopeModal.stopTask = function () {
                return executeTask(false);
            };

            $scope.onViewChangeTypeSelectItem = function (dataItem) {
                if (dataItem != undefined) {
                    directiveWrapperReadyPromiseDeferred = UtilsService.createPromiseDeferred();
                    directiveWrapperReadyPromiseDeferred.resolve();
                    changeType = dataItem.value;
                    return loadPreviewDataSection();
                }
            };

            $scope.scopeModal.onValidationMessageHistoryGridReady = function (api) {
                validationMessageHistoryGridAPI = api;
                validationMessageHistoryReadyPromiseDeferred.resolve();
            };


            $scope.onViewChangeTypeSelectorReady = function (api) {
                viewChangeTypeSelectorAPI = api;
                viewChangeTypeReadyPromiseDeferred.resolve();
            };

            $scope.onGroupedBySelectorReady = function (api) {
                groupedBySelectorAPI = api;
                groupedByReadyPromiseDeferred.resolve();
            };

            $scope.onDirectiveWrapperReady = function (api) {
                directiveWrapperAPI = api;

                var setLoader = function (value) {
                    $scope.scopeModal.isLoadingPreviewDataSection = value;
                };

                var previewDataPayload = {
                    ProcessInstanceId: processInstanceId,
                    OnlyModified: viewChangeTypeSelectorAPI.getSelectedIds(),
                };


                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope.scopeModal, directiveWrapperAPI, previewDataPayload, setLoader, directiveWrapperReadyPromiseDeferred);
            };

        }

        function executeTask(taskAction) {
            var executionInformation = {
                $type: "TOne.WhS.CodePreparation.BP.Arguments.Tasks.PreviewTaskExecutionInformation, TOne.WhS.CodePreparation.BP.Arguments",
                Decision: taskAction
            };

            var input = {
                $type: "Vanrise.BusinessProcess.Entities.ExecuteBPTaskInput, Vanrise.BusinessProcess.Entities",
                TaskId: bpTaskId,
                ExecutionInformation: executionInformation
            };

            return BusinessProcess_BPTaskAPIService.ExecuteTask(input).then(function (response) {
                $scope.modalContext.closeModal();
            }).catch(function (error) {
                VRNotificationService.notifyException(error);
            });
        }

        function load() {
            $scope.scopeModal.isLoading = true;
            BusinessProcess_BPTaskAPIService.GetTask(bpTaskId).then(function (response) {
                processInstanceId = response.ProcessInstanceId;
                loadAllControls();
            })
        }

        function loadAllControls() {

            return UtilsService.waitMultipleAsyncOperations([loadViewChangeTypeSelector, loadGroupedBySelector, loadPreviewDataSection, loadValidationMessageHistory])
                          .catch(function (error) {
                              VRNotificationService.notifyException(error);
                          })
                          .finally(function () {
                              $scope.scopeModal.isLoading = false;
                          });

        }

        function loadViewChangeTypeSelector() {
            var loadViewChangeTypeSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            viewChangeTypeReadyPromiseDeferred.promise.then(function () {
                var viewChangeTypeSelectorPayload = {
                    selectedIds: WhS_CP_PreviewChangeTypeEnum.OnlyModifiedEntities.value
                };

                VRUIUtilsService.callDirectiveLoad(viewChangeTypeSelectorAPI, viewChangeTypeSelectorPayload, loadViewChangeTypeSelectorPromiseDeferred);

            });

            return loadViewChangeTypeSelectorPromiseDeferred.promise;
        }

        function loadGroupedBySelector() {
            var loadGroupedBySelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            groupedByReadyPromiseDeferred.promise.then(function () {
                var viewChangeTypeSelectorPayload = {
                    selectedIds: WhS_CP_PreviewGroupedBy.Countries.description
                };

                VRUIUtilsService.callDirectiveLoad(groupedBySelectorAPI, viewChangeTypeSelectorPayload, loadGroupedBySelectorPromiseDeferred);

            });

            return loadGroupedBySelectorPromiseDeferred.promise;
        }

        function loadPreviewDataSection() {
            var loadPreviewDataPromiseDeferred = UtilsService.createPromiseDeferred();
            directiveWrapperReadyPromiseDeferred.promise.then(function () {
                directiveWrapperReadyPromiseDeferred = undefined;
                var payload = {
                    ProcessInstanceId: processInstanceId,
                    OnlyModified: changeType
                };
                VRUIUtilsService.callDirectiveLoad(directiveWrapperAPI, payload, loadPreviewDataPromiseDeferred)
            });
            return loadPreviewDataPromiseDeferred.promise;
        }

        function loadValidationMessageHistory() {
            var loadValidationMessageHistoryPromiseDeferred = UtilsService.createPromiseDeferred();

            validationMessageHistoryReadyPromiseDeferred.promise.then(function () {
                var payload = {
                    BPInstanceID: processInstanceId,
                };

                VRUIUtilsService.callDirectiveLoad(validationMessageHistoryGridAPI, payload, loadValidationMessageHistoryPromiseDeferred)
            });

            return loadValidationMessageHistoryPromiseDeferred.promise;
        }

    }

    appControllers.controller('WhS_CP_CodePreparationPreviewController', CodePreparationPreviewController);
})(appControllers);

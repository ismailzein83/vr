(function (appControllers) {

    "use strict";

    jazzReportsPreviewTaskController.$inject = ['$scope', 'BusinessProcess_BPTaskAPIService', 'WhS_SupPL_PreviewChangeTypeEnum', 'WhS_SupPL_PreviewGroupedBy', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService','WhS_Jazz_TransactionsReportAPIService'];

    function jazzReportsPreviewTaskController($scope, BusinessProcess_BPTaskAPIService, WhS_SupPL_PreviewChangeTypeEnum, WhS_SupPL_PreviewGroupedBy, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, WhS_Jazz_TransactionsReportAPIService) {

        var bpTaskId;
        var processInstanceId;

        var taskData;

        var fileDownloadDirectiveApi;
        var fileDownloadDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        $scope.scopeModal = {};
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

            

            $scope.scopeModal.continueTask = function () {
                return executeTask(true);
            };

            $scope.scopeModal.stopTask = function () {
                return executeTask(false);
            };

            $scope.onFileDownloadDirectiveReady = function (api) {
                fileDownloadDirectiveApi = api;
                fileDownloadDirectiveReadyPromiseDeferred.resolve();
             
            };



        }

        function executeTask(taskAction) {
            var executionInformation = {
                $type: "TOne.WhS.Jazz.BP.Arguments.PreviewTaskExecutionInformation, TOne.WhS.Jazz.BP.Arguments",
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
                taskData = response.TaskData; 
                $scope.scopeModal.value = {
                    fileId: taskData.FileId
                };
                $scope.scopeModal.downloadFile = {
                    fileName: "Jazz Transaction Report",
                    downloadFileCallBackAPI: function () {
                        return WhS_Jazz_TransactionsReportAPIService.DownloadTransactionsReports(processInstanceId);
                    }
                };

                loadAllControls();
            });
        }

        function loadAllControls() {

            return UtilsService.waitMultipleAsyncOperations([setTitle])
                          .catch(function (error) {
                              VRNotificationService.notifyException(error);
                          })
                          .finally(function () {
                              $scope.scopeModal.isLoading = false;
                          });

        }
        function setTitle() {
            $scope.title = 'File Preview';
        }
        //function loadFileDownloadDirective() {
        //    var fileDownloadDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();


        //    fileDownloadDirectiveReadyPromiseDeferred.promise.then(function () {
        //        var fileDownloadPayload = {
        //            processInstanceId: processInstanceId,
        //            fileId: taskData.FileId,
        //            requireWarningConfirmation: true
        //        };
        //        VRUIUtilsService.callDirectiveLoad(fileDownloadDirectiveApi, fileDownloadPayload, fileDownloadDirectiveLoadPromiseDeferred);

        //    });
        //    return fileDownloadDirectiveLoadPromiseDeferred.promise;
        //}
    }
    appControllers.controller('WhS_Jazz_ReportsPreviewTaskController', jazzReportsPreviewTaskController);
})(appControllers);
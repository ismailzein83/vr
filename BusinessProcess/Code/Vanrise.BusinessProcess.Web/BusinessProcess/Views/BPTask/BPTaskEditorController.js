BPTaskEditorController.$inject = ['$scope', 'VRNavigationService', 'BusinessProcess_BPTaskAPIService', 'UtilsService','VRNotificationService','VRUIUtilsService',];

function BPTaskEditorController($scope, VRNavigationService, BusinessProcess_BPTaskAPIService, UtilsService, VRNotificationService,VRUIUtilsService) {
    var bpTaskId;

    var bpTaskDirectiveApi;
    var bpTaskDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    defineScope();
    loadParameters();
    load();

    function defineScope() {

        $scope.createProcessInput = {};
        $scope.close = function () {
            $scope.modalContext.closeModal()
        };

        $scope.onBPTaskDirectiveReady = function (api) {
            bpTaskDirectiveApi = api;
            bpTaskDirectiveReadyPromiseDeferred.resolve();
        }

        $scope.executeTask = function () {
            var taskData = buildInstanceObjFromScope();
            if (taskData != null) {
                BusinessProcess_BPTaskAPIService.ExecuteTask(taskData).then(function (response) {
                        $scope.modalContext.closeModal();
                }).catch(function (error) {
                    VRNotificationService.notifyException(error);
                });
            }
        };
    }

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);
        if (parameters !== undefined && parameters !== null) {
            bpTaskId = parameters.TaskId;
            bpTaskTypeId = parameters.TaskTypeId;
        }
    }

    function load() {
        $scope.isLoading = true;
        getBPTaskType().then(function () {
            if ($scope.bpTaskType && $scope.bpTaskType.Settings && $scope.bpTaskType.Settings.Editor) {
                loadAllControls().finally(function () {
                    $scope.isLoading = false;
                });
            }
            else
                $scope.isLoading = false;

        }).catch(function (error) {
            VRNotificationService.notifyExceptionWithClose(error, $scope);
            $scope.isLoading = false;
        })
    }



    function getBPTaskType() {

        return BusinessProcess_BPTaskAPIService.GetBPTaskType(bpTaskTypeId)
           .then(function (response) {
               $scope.bpTaskType = response;
           }).catch(function (error) {
               VRNotificationService.notifyExceptionWithClose(error, $scope);
           }).finally(function () {

           });;
    }


    function loadAllControls() {
        var loadBPTaskTypePromiseDeferred = UtilsService.createPromiseDeferred();
        bpTaskDirectiveReadyPromiseDeferred.promise.then(function () {
            VRUIUtilsService.callDirectiveLoad(bpTaskDirectiveApi, undefined, loadBPTaskTypePromiseDeferred);
        });
        return loadBPTaskTypePromiseDeferred.promise;
    }

    function buildInstanceObjFromScope() {
        if (bpTaskDirectiveApi != undefined) {
            return bpTaskDirectiveApi.getData(bpTaskId);
        }
        else return null;
    }
}
appControllers.controller('BusinessProcess_BPTaskEditorController', BPTaskEditorController);

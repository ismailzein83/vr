CustomCodeTaskCompilationResultController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRNotificationService'];

function CustomCodeTaskCompilationResultController($scope, VRNavigationService, UtilsService, VRNotificationService) {
    var mainGridAPI;
    loadParameters();
    defineScope();
    load();

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);
        if (parameters != null && parameters.errorMessages != undefined) {
            $scope.dataSrouce = [];
            for (var i = 0; i < parameters.errorMessages.length; i++)
            {
                $scope.dataSrouce.push({ Error: parameters.errorMessages[i] })
            }
        }
    }

    function defineScope() {

        $scope.close = function () {
            $scope.modalContext.closeModal()
        };
      
        $scope.title = 'Custom Code Task Compilation Result';
        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
        };
    }

   
    function load() {
    }

}
appControllers.controller('BusinessProcess_BP_CustomCodeTaskCompilationResultController', CustomCodeTaskCompilationResultController);

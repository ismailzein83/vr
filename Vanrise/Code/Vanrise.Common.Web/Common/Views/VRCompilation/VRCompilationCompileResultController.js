//VRCompilationCompileResultController.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRNotificationService', 'VRCommon_CompilationAPIService'];

//function VRCompilationCompileResultController($scope, VRNavigationService, UtilsService, VRNotificationService, VRCommon_CompilationAPIService) {

//    var mainGridAPI;
//    var errorMessages;
//    var title;
//    loadParameters();
//    defineScope();
//    load();

//    function loadParameters() {
//        var parameters = VRNavigationService.getParameters($scope);

//        if (parameters != null && parameters.errorMessages != undefined) {
//            $scope.datasource = [];
//            title = parameters.title;
//            errorMessages = parameters.errorMessages;
//            for (var i = 0; i < errorMessages.length; i++) {
//                $scope.datasource.push({ Error: errorMessages[i] });
//            }
//        }
//    }

//    function defineScope() {
//        $scope.close = function () {
//            $scope.modalContext.closeModal();
//        };
//        $scope.export = function () {
//            return VRCommon_CompilationAPIService.ExportCompilationResult(errorMessages).then(function (response) {
//                UtilsService.downloadFile(response.data, response.headers);
//            });
//        };
//        $scope.title = title;
//        $scope.onMainGridReady = function (api) {
//            mainGridAPI = api;
//        };
//    }

//    function load() {
//    }
//}
//appControllers.controller('VRCommon_VRCompilationCompileResultController', VRCompilationCompileResultController);

VRNamespaceItemCompilationResult.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRNotificationService', 'VRCommon_VRNamespaceItemAPIService'];

function VRNamespaceItemCompilationResult($scope, VRNavigationService, UtilsService, VRNotificationService, VRCommon_VRNamespaceItemAPIService) {

    var mainGridAPI;
    var namespaceObj;

    loadParameters();
    defineScope();
    load();

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);

        if (parameters != null && parameters.errorMessages != undefined) {
            namespaceObj = parameters.namespaceObj;
            $scope.datasource = [];

            for (var i = 0; i < parameters.errorMessages.length; i++) {
                $scope.datasource.push({ Error: parameters.errorMessages[i] });
            }
        }
    }

    function defineScope() {
        $scope.close = function () {
            $scope.modalContext.closeModal();
        };
        $scope.export = function () {

            return VRCommon_VRNamespaceItemAPIService.ExportCompilationResult(namespaceObj).then(function (response) {
                UtilsService.downloadFile(response.data, response.headers);
            });
        };
        $scope.title = 'Namespace Compilation Result';
        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
        };
    }

    function load() {
    }
}
appControllers.controller('Common_VRNamespaceItemCompilationResultController', VRNamespaceItemCompilationResult);

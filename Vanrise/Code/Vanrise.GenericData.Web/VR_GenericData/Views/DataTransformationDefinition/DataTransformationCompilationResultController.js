DataTransformationCompilationResult.$inject = ['$scope', 'VRNavigationService', 'UtilsService', 'VRNotificationService', 'VR_GenericData_DataTransformationDefinitionAPIService'];

function DataTransformationCompilationResult($scope, VRNavigationService, UtilsService,VRNotificationService, VR_GenericData_DataTransformationDefinitionAPIService) {
    var mainGridAPI;
    loadParameters();
    defineScope();
    load();
    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);
        if (parameters != null && parameters.errorMessages != undefined) {
            $scope.dataTransformationObj = parameters.dataTransformationObj;
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
        $scope.export = function () {

            return VR_GenericData_DataTransformationDefinitionAPIService.ExportCompilationResult($scope.dataTransformationObj).then(function (response) {
                UtilsService.downloadFile(response.data, response.headers);
            });
        };
        $scope.title = 'Data Transformation Compilation Result';
        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
        };
    }

   
    function load() {
    }





}
appControllers.controller('VR_GenericData_DataTransformationCompilationResult', DataTransformationCompilationResult);

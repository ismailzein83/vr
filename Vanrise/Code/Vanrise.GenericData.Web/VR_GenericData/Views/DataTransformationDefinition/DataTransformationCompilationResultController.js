DataTransformationCompilationResult.$inject = ['$scope', 'VRNavigationService', 'VR_GenericData_DataTransformationDefinitionAPIService', 'UtilsService', 'VRNotificationService'];

function DataTransformationCompilationResult($scope, VRNavigationService, VR_GenericData_DataTransformationDefinitionAPIService, VRNotificationService) {
    var mainGridAPI;
    loadParameters();
    defineScope();
    load();
    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);
        if (parameters != null) {
            $scope.dataTransformationObj = parameters.dataTransformationDefinition;
        }
    }

    function defineScope() {
        $scope.dataSrouce = [];
        $scope.close = function () {
            $scope.modalContext.closeModal()
        };
        $scope.title = 'Data Transformation Compilation Result';
        $scope.onMainGridReady = function (api) {
            mainGridAPI = api;
            if ($scope.dataTransformationObj != undefined) {
                $scope.isLoading = true;
                tryCompile().catch(function (error) {
                    $scope.isLoading = false;
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
                .finally(function () {
                    $scope.isLoading = false;
                });
            }
        };
    }

   
    function load() {
        if($scope.dataTransformationObj != undefined)
        {
            $scope.isLoading = true;
            tryCompile().catch(function (error) {
                $scope.isLoading = false;
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            })
            .finally(function () {
                $scope.isLoading = false;
            });
        }
            
    }
    function tryCompile() {
        return VR_GenericData_DataTransformationDefinitionAPIService.TryCompileSteps($scope.dataTransformationObj)
        .then(function (response) {
            if (response) {
                if (response.Result) {
                    $scope.showMessage = "All steps compiled successfully.";
                }else
                {
                    for (var i = 0; i < response.ErrorMessages.length; i++)
                    {
                        $scope.dataSrouce.push({Error:response.ErrorMessages[i]})
                    }
                }
            }
        });
    }




}
appControllers.controller('VR_GenericData_DataTransformationCompilationResult', DataTransformationCompilationResult);

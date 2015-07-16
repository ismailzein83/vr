InstanceEditorController.$inject = ['$scope', 'BusinessProcessAPIService', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'UtilsService'];

function InstanceEditorController($scope, BusinessProcessAPIService, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService, UtilsService) {
    defineScope();
    loadParameters();
    load();

    function defineScope() {


        $scope.createProcessInput = {};
        $scope.close = function () {
            $scope.modalContext.closeModal()
        };


        $scope.createNewProcess = function () {

            $scope.issaving = true;
            var createProcessInputs = buildInstanceObjFromScope();

            if (angular.isArray(createProcessInputs))
            {
                angular.forEach(createProcessInputs, function (itm) {
                    BusinessProcessAPIService.CreateNewProcess(itm).then().catch(function (error) {
                        VRNotificationService.notifyException(error);
                    });
                });

                if ($scope.onProcessInputsCreated != undefined)
                    $scope.onProcessInputsCreated();

                $scope.modalContext.closeModal();
            }
            else {
                BusinessProcessAPIService.CreateNewProcess(createProcessInputs).then(function (response) {
                    if (VRNotificationService.notifyOnItemAdded("Bussiness Instance", response)) {
                        if ($scope.onProcessInputCreated != undefined)
                            $scope.onProcessInputCreated(response.ProcessInstanceId);
                        $scope.modalContext.closeModal();
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error);
                });
            }
        };
    }



    function buildInstanceObjFromScope() {
        var createProcessInputs = $scope.createProcessInput.getData();
        return createProcessInputs;
    }



    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);
        $scope.BPDefinitionID = undefined;

        if (parameters != undefined && parameters != null)
            $scope.BPDefinitionID = parameters.BPDefinitionID;
    }

    function load()
    {
        getBPDefinition().finally(function () {
            $scope.isGettingData = false;
        });
    }



    function getBPDefinition() {

        return BusinessProcessAPIService.GetDefinition($scope.BPDefinitionID)
           .then(function (response) {
               fillScopeFromBPDefinitionObj(response);
           })
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
    }

    function fillScopeFromBPDefinitionObj(bpDefinitionObject) {
        $scope.bpDefinitionObj = bpDefinitionObject
        $scope.bpDefinitionObj.configuration = bpDefinitionObject.Configuration;
        $scope.bpDefinitionObj.configuration.url = bpDefinitionObject.Configuration.Url;

    }



}
appControllers.controller('FraudAnalysis_InstanceEditorController', InstanceEditorController);

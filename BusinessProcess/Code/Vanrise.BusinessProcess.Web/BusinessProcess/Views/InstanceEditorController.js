InstanceEditorController.$inject = ['$scope', 'BusinessProcessAPIService', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'UtilsService'];

function InstanceEditorController($scope, BusinessProcessAPIService, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService, UtilsService) {
    defineScope();
    loadParameters();
    load();

    function defineScope() {
        $scope.subViewExecuteStrategyProcessInput = {};
        $scope.close = function () {
            $scope.modalContext.closeModal()
        };
        $scope.startInstance = function () {

            $scope.issaving = true;
            var createProcessInput = buildInstanceObjFromScope();
            BusinessProcessAPIService.CreateNewProcess(createProcessInput).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Bussiness Instance", response)) {
                    if ($scope.onProcessInputCreated != undefined)
                        $scope.onProcessInputCreated(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error);
            });


        };
    }









    function buildInstanceObjFromScope() {
        var inputArguments = $scope.subViewExecuteStrategyProcessInput.getData();
        var createProcessInputObject = {
            InputArguments: inputArguments
        };

        return createProcessInputObject;
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

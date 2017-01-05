NewInstanceEditorController.$inject = ['$scope', 'BusinessProcess_BPInstanceAPIService','BusinessProcess_BPDefinitionAPIService', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

function NewInstanceEditorController($scope, BusinessProcess_BPInstanceAPIService,BusinessProcess_BPDefinitionAPIService, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {
    var bpDefinitionDirectiveApi;

    var bpDefinitionDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

    defineScope();
    loadParameters();
    load();
    function defineScope() {


        $scope.createProcessInput = {};
        $scope.close = function () {
            $scope.modalContext.closeModal()
        };

        $scope.onBPDefinitionManualDirectiveReady = function (api) {
            bpDefinitionDirectiveApi = api;
            bpDefinitionDirectiveReadyPromiseDeferred.resolve();

        };
        $scope.createNewProcess = function () {

            $scope.issaving = true;
            var createProcessInputs = buildInstanceObjFromScope();
            if (createProcessInputs != null) {
                if (angular.isArray(createProcessInputs))
                {
                    angular.forEach(createProcessInputs, function (itm) {
                        BusinessProcess_BPInstanceAPIService.CreateNewProcess(itm).then().catch(function (error) {
                            VRNotificationService.notifyException(error);
                        });
                    });

                    if ($scope.onProcessInputsCreated != undefined)
                        $scope.onProcessInputsCreated();

                    $scope.modalContext.closeModal();
                }
                else {
                    BusinessProcess_BPInstanceAPIService.CreateNewProcess(createProcessInputs).then(function (response) {
                        if (VRNotificationService.notifyOnItemAdded("Business Instance", response)) {
                            if ($scope.onProcessInputCreated != undefined)
                                $scope.onProcessInputCreated(response.ProcessInstanceId);
                            $scope.modalContext.closeModal();
                        }
                    }).catch(function (error) {
                        VRNotificationService.notifyException(error);
                    });
                }
            }
           
        };
    }



    function buildInstanceObjFromScope() {
        if (bpDefinitionDirectiveApi != undefined) {
            return bpDefinitionDirectiveApi.getData()
        }
        else return null;
    }



    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);
        $scope.BPDefinitionID = undefined;

        if (parameters != undefined && parameters != null)
            $scope.BPDefinitionID = parameters.BPDefinitionID;
    }

    function load()
    {
        $scope.isLoading = true;
        getBPDefinition().then(function () {
            if ($scope.bpDefinitionObj.Configuration.ManualExecEditor)
            {
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



    function getBPDefinition() {

        return BusinessProcess_BPDefinitionAPIService.GetBPDefintion($scope.BPDefinitionID)
           .then(function (response) {
               $scope.bpDefinitionObj = response;
           }).catch(function(error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function() {
               
            });;
    }


    function loadAllControls() {
        var loadBPDefinitionPromiseDeferred = UtilsService.createPromiseDeferred();
        bpDefinitionDirectiveReadyPromiseDeferred.promise.then(function () {
            VRUIUtilsService.callDirectiveLoad(bpDefinitionDirectiveApi, undefined, loadBPDefinitionPromiseDeferred);
        });
        return loadBPDefinitionPromiseDeferred.promise ;
    }



}
appControllers.controller('FraudAnalysis_NewInstanceEditorController', NewInstanceEditorController);

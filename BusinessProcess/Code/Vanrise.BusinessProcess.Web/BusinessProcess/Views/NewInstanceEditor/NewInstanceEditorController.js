(function (appControllers) {

    "use strict";

    NewInstanceEditorController.$inject = ['$scope', 'BusinessProcess_BPInstanceAPIService', 'BusinessProcess_BPDefinitionAPIService', '$routeParams', 'notify', 'BusinessProcess_VRWorkflowAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'BusinessProcess_DynamicBusinessProcessAPIService'];

    function NewInstanceEditorController($scope, BusinessProcess_BPInstanceAPIService, BusinessProcess_BPDefinitionAPIService, $routeParams, notify, BusinessProcess_VRWorkflowAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, BusinessProcess_DynamicBusinessProcessAPIService) {

        var bpDefinitionId;

        var bpDefinitionManualDirectiveApi;
        var bpDefinitionManualDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();


        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null)
                bpDefinitionId = parameters.BPDefinitionID;
        }
        function defineScope() {
            $scope.createProcessInput = {};
            $scope.BPDefinitionID = bpDefinitionId;

            $scope.onBPDefinitionManualDirectiveReady = function (api) {
                bpDefinitionManualDirectiveApi = api;
                bpDefinitionManualDirectiveReadyPromiseDeferred.resolve();
            };

            $scope.createNewProcess = function () {
                var promises = [];

                var createProcessInputs = buildInstanceObjFromScope();
                if (createProcessInputs != null) {
                    if (angular.isArray(createProcessInputs)) {
                        angular.forEach(createProcessInputs, function (itm) {
                            var currentCreateNewProcessPromise = getCreateNewProcessPromise(itm);
                            promises.push(currentCreateNewProcessPromise);

                            currentCreateNewProcessPromise.then().catch(function (error) {
                                VRNotificationService.notifyException(error);
                            });
                        });

                        if ($scope.onProcessInputsCreated != undefined)
                            $scope.onProcessInputsCreated();
                        $scope.modalContext.closeModal();
                    }
                    else {
                        var currentCreateNewProcessPromise = getCreateNewProcessPromise(createProcessInputs);
                        promises.push(currentCreateNewProcessPromise);

                        currentCreateNewProcessPromise.then(function (response) {
                            if ($scope.bpDefinitionObj.VRWorkflowId == undefined) {
                                if (VRNotificationService.notifyOnItemAdded("Business Instance", response)) {
                                    if ($scope.onProcessInputCreated != undefined)
                                        $scope.onProcessInputCreated(response.ProcessInstanceId);
                                    $scope.modalContext.closeModal();
                                }
                            }
                            else {
                                if ($scope.onProcessInputCreated != undefined)
                                    $scope.onProcessInputCreated(response.ProcessId);
                                $scope.modalContext.closeModal();
                            }

                        }).catch(function (error) {
                            VRNotificationService.notifyException(error);
                        });
                    }
                }

                function getCreateNewProcessPromise(createProcessInput) {
                    if ($scope.bpDefinitionObj.VRWorkflowId == undefined)
                        return BusinessProcess_BPInstanceAPIService.CreateNewProcess(createProcessInput);
                    else
                        return BusinessProcess_DynamicBusinessProcessAPIService.StartProcess(UtilsService.replaceAll($scope.bpDefinitionObj.Title, ' ', ''), createProcessInput);
                }

                return UtilsService.waitMultiplePromises(promises);
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
        }
        function load() {
            $scope.isLoading = true;

            var loadPromiseDeferred = UtilsService.createPromiseDeferred();

            getBPDefinition().then(function () {
                loadAllControls().finally(function () {
                    $scope.isLoading = false;
                    loadPromiseDeferred.resolve();
                });
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.isLoading = false;
            });

            return loadPromiseDeferred.promise;
        }

        function getBPDefinition() {

            return BusinessProcess_BPDefinitionAPIService.GetBPDefintion($scope.BPDefinitionID)
                .then(function (response) {
                    $scope.bpDefinitionObj = response;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                }).finally(function () {

                });
        }

        function loadAllControls() {
            var initialPromises = [];

            if ($scope.bpDefinitionObj.Configuration.ManualExecEditor) {
                var loadBpDefinitionManualDirectivePromise = loadBpDefinitionManualDirective();
                initialPromises.push(loadBpDefinitionManualDirectivePromise);
            }

            var rootPromiseNode = {
                promises: initialPromises
            };

            function loadBpDefinitionManualDirective() {
                var loadBPManualDefinitionPromiseDeferred = UtilsService.createPromiseDeferred();

                bpDefinitionManualDirectiveReadyPromiseDeferred.promise.then(function () {
                    var bpDefinitionPayload = {
                        bpDefinitionId: bpDefinitionId
                    };
                    VRUIUtilsService.callDirectiveLoad(bpDefinitionManualDirectiveApi, bpDefinitionPayload, loadBPManualDefinitionPromiseDeferred);
                });

                return loadBPManualDefinitionPromiseDeferred.promise;
            }

            return UtilsService.waitPromiseNode(rootPromiseNode);;
        }



        function buildInstanceObjFromScope() {
            if (bpDefinitionManualDirectiveApi != undefined) {
                return bpDefinitionManualDirectiveApi.getData();
            }
            else return null;
        }
    }

    appControllers.controller('FraudAnalysis_NewInstanceEditorController', NewInstanceEditorController);

})(appControllers);

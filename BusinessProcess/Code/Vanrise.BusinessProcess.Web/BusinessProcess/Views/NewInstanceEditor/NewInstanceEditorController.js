(function (appControllers) {

    "use strict";

    NewInstanceEditorController.$inject = ['$scope', 'BusinessProcess_BPInstanceAPIService', 'BusinessProcess_BPDefinitionAPIService', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function NewInstanceEditorController($scope, BusinessProcess_BPInstanceAPIService, BusinessProcess_BPDefinitionAPIService, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var bpDefinitionId;

        var bpDefinitionDirectiveApi;
        var bpDefinitionDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

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
                bpDefinitionDirectiveApi = api;
                bpDefinitionDirectiveReadyPromiseDeferred.resolve();

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

                function getCreateNewProcessPromise(createProcessInput) {
                    return BusinessProcess_BPInstanceAPIService.CreateNewProcess(createProcessInput);
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
                if ($scope.bpDefinitionObj.Configuration.ManualExecEditor) {
                    loadAllControls().finally(function () {
                        $scope.isLoading = false;
                        loadPromiseDeferred.resolve();
                    });
                }
                else {
                    loadPromiseDeferred.resolve();
                    $scope.isLoading = false;
                }
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

               });;
        }

        function loadAllControls() {
            var loadBPDefinitionPromiseDeferred = UtilsService.createPromiseDeferred();
            bpDefinitionDirectiveReadyPromiseDeferred.promise.then(function () {
                var bpDefinitionPayload = { bpDefinitionId: bpDefinitionId };
                VRUIUtilsService.callDirectiveLoad(bpDefinitionDirectiveApi, bpDefinitionPayload, loadBPDefinitionPromiseDeferred);
            });
            return loadBPDefinitionPromiseDeferred.promise;
        }

        function buildInstanceObjFromScope() {
            if (bpDefinitionDirectiveApi != undefined) {
                return bpDefinitionDirectiveApi.getData();
            }
            else return null;
        }
    }

    appControllers.controller('FraudAnalysis_NewInstanceEditorController', NewInstanceEditorController);

})(appControllers);

(function (appControllers) {

    'use strict';

    StartBPProcessEditorController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService', 'Retail_BE_AccountBEAPIService', 'BusinessProcess_DynamicBusinessProcessAPIService', 'BusinessProcess_BPDefinitionAPIService','BusinessProcess_BPInstanceService'];

    function StartBPProcessEditorController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, Retail_BE_AccountBEAPIService,
        BusinessProcess_DynamicBusinessProcessAPIService, BusinessProcess_BPDefinitionAPIService, BusinessProcess_BPInstanceService) {

        var accountBEDefinitionId;
        var accountId;
        var actionDefinitionSettings;
        var accountEntity;

        var bpDefinitionDirectiveApi;
        var bpDefinitionDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                accountBEDefinitionId = parameters.accountBEDefinitionId;
                accountId = parameters.accountId;
                actionDefinitionSettings = parameters.actionDefinitionSettings;
            }
        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.BPDefinitionID = actionDefinitionSettings.BPDefinitionId;

            $scope.scopeModel.onBPDefinitionManualDirectiveReady = function (api) {
                bpDefinitionDirectiveApi = api;
                bpDefinitionDirectiveReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.startBPProcess = function () {
                var promises = [];
                var createProcessInputs = buildInstanceObjFromScope();
                promises.push(getCreateNewProcessPromise(createProcessInputs));

                function getCreateNewProcessPromise(createProcessInput) {
                    return BusinessProcess_DynamicBusinessProcessAPIService.StartProcess(UtilsService.replaceAll($scope.bpDefinitionObj.Title, ' ', ''), createProcessInput).then(function (response) {
                        BusinessProcess_BPInstanceService.openProcessTracking(response.ProcessId);
                        $scope.modalContext.closeModal();

                    }).catch(function (error) {
                        VRNotificationService.notifyException(error);
                    });
                }

                return UtilsService.waitMultiplePromises(promises);
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;

            loadAllControls()
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
                .finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
        }

        function loadAllControls() {
            var initialPromises = [];

            initialPromises.push(getAccount());
            initialPromises.push(getBPDefinition());

            function getAccount() {
                return Retail_BE_AccountBEAPIService.GetAccount(accountBEDefinitionId, accountId).then(function (response) {
                    accountEntity = response;
                });
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

            function settitle() {
                $scope.title = (accountEntity != undefined) ? "account name: " + accountEntity.Name : undefined;
            }

            function loadBPDefinition() {
                var loadBPDefinitionPromiseDeferred = UtilsService.createPromiseDeferred();
                bpDefinitionDirectiveReadyPromiseDeferred.promise.then(function () {
                    var bpDefinitionPayload = { bpDefinitionId: actionDefinitionSettings.BPDefinitionId };
                    VRUIUtilsService.callDirectiveLoad(bpDefinitionDirectiveApi, bpDefinitionPayload, loadBPDefinitionPromiseDeferred);
                });
                return loadBPDefinitionPromiseDeferred.promise;
            }

            var rootPromiseNode = {
                promises: initialPromises,
                getChildNode: function () {
                    return {
                        promises: [UtilsService.waitMultipleAsyncOperations([settitle, loadBPDefinition])]
                    };
                }
            };

            return UtilsService.waitPromiseNode(rootPromiseNode);
        }

        function buildInstanceObjFromScope() {
            var createProcessInput;
            if (bpDefinitionDirectiveApi != undefined) {
                 createProcessInput = bpDefinitionDirectiveApi.getData();

                if (createProcessInput == undefined)
                    createProcessInput = {};

                if (createProcessInput.InputArguments == undefined)
                    createProcessInput.InputArguments = {};

                createProcessInput.InputArguments[actionDefinitionSettings.AccountIdInputFieldName] = accountId;

                if (actionDefinitionSettings.AccountBEDefinitionIdInputFieldName != undefined)
                    createProcessInput.InputArguments[actionDefinitionSettings.AccountBEDefinitionIdInputFieldName] = accountBEDefinitionId;
            }
            return createProcessInput;
        }

    }

    appControllers.controller('Retail_BE_StartBPProcessEditorController', StartBPProcessEditorController);

})(appControllers);
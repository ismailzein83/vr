(function (appControllers) {

    'use strict';

    StartBPProcessEditorController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService', 'VR_GenericData_GenericBusinessEntityAPIService', 'BusinessProcess_DynamicBusinessProcessAPIService', 'BusinessProcess_BPDefinitionAPIService', 'BusinessProcess_BPInstanceService'];

    function StartBPProcessEditorController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, VR_GenericData_GenericBusinessEntityAPIService,
        BusinessProcess_DynamicBusinessProcessAPIService, BusinessProcess_BPDefinitionAPIService, BusinessProcess_BPInstanceService) {

        var businessEntityDefinitionId;
        var genericBusinessEntityId;
        var genericBEActionSettings;
        var genericBusinessEntity;

        var bpDefinitionDirectiveApi;
        var bpDefinitionDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                businessEntityDefinitionId = parameters.businessEntityDefinitionId;
                genericBusinessEntityId = parameters.genericBusinessEntityId;
                genericBEActionSettings = parameters.genericBEActionSettings;
            }
        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.BPDefinitionID = genericBEActionSettings.BPDefinitionId;

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

            initialPromises.push(getGenericBusinessEntity());
            initialPromises.push(getBPDefinition());

            function getGenericBusinessEntity() {
                return VR_GenericData_GenericBusinessEntityAPIService.GetGenericBusinessEntityDetail(genericBusinessEntityId, businessEntityDefinitionId).then(function (response) {
                    genericBusinessEntity = response;
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

            function setTitle() {
                $scope.title = 'Start New Process';
            }

            function loadBPDefinition() {
                var loadBPDefinitionPromiseDeferred = UtilsService.createPromiseDeferred();
                bpDefinitionDirectiveReadyPromiseDeferred.promise.then(function () {
                    var bpDefinitionPayload = { bpDefinitionId: genericBEActionSettings.BPDefinitionId };
                    VRUIUtilsService.callDirectiveLoad(bpDefinitionDirectiveApi, bpDefinitionPayload, loadBPDefinitionPromiseDeferred);
                });
                return loadBPDefinitionPromiseDeferred.promise;
            }

            var rootPromiseNode = {
                promises: initialPromises,
                getChildNode: function () {
                    return {
                        promises: [UtilsService.waitMultipleAsyncOperations([setTitle, loadBPDefinition])]
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

                if (genericBEActionSettings.InputArgumentsMapping != undefined) {
                    for (var i = 0; i < genericBEActionSettings.InputArgumentsMapping.length; i++) {
                        var inputArgumentMapped = genericBEActionSettings.InputArgumentsMapping[i];
                        createProcessInput.InputArguments[inputArgumentMapped.InputArgumentName] = genericBusinessEntity.FieldValues[inputArgumentMapped.MappedFieldName].Value;
                    }
                }


            }
            return createProcessInput;
        }

    }

    appControllers.controller('BusinessProcess_GenericBE_StartBPProcessEditorController', StartBPProcessEditorController);

})(appControllers);
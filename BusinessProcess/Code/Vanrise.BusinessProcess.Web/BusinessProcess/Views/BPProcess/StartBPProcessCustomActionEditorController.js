//(function (appControllers) {

//    'use strict';

//    StartBPProcessEditorCustomActionController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService', 'VR_GenericData_GenericBusinessEntityAPIService', 'BusinessProcess_DynamicBusinessProcessAPIService', 'BusinessProcess_BPDefinitionAPIService', 'BusinessProcess_BPInstanceService'];

//    function StartBPProcessEditorCustomActionController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, VR_GenericData_GenericBusinessEntityAPIService,
//        BusinessProcess_DynamicBusinessProcessAPIService, BusinessProcess_BPDefinitionAPIService, BusinessProcess_BPInstanceService) {

//        var dataRecordTypeId;
//        var businessEntityDefinitionId;
//        var customActionSettings;
//        var parentFieldValues;
//        var context;

//        var bpDefinitionDirectiveApi;
//        var bpDefinitionDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();
      
//        loadParameters();
//        defineScope();
//        load();

//        function loadParameters() {
//            var parameters = VRNavigationService.getParameters($scope);

//            if (parameters != undefined) {
//                dataRecordTypeId = parameters.dataRecordTypeId;
//                businessEntityDefinitionId = parameters.businessEntityDefinitionId;
//                customActionSettings = parameters.customActionSettings;
//                parentFieldValues = parameters.parentFieldValues;
//                context = parameters.context;
//            }
//        }

//        function defineScope() {
//            $scope.scopeModel = {};
//            $scope.BPDefinitionID = customActionSettings.BPDefinitionId;

//            $scope.scopeModel.onBPDefinitionManualDirectiveReady = function (api) {
//                console.log(api);
//                bpDefinitionDirectiveApi = api;
//                bpDefinitionDirectiveReadyPromiseDeferred.resolve();
//            };

//            $scope.scopeModel.startBPProcess = function () {
//                var promises = [];
//                var createProcessInputs = buildInstanceObjFromScope();
//                promises.push(getCreateNewProcessPromise(createProcessInputs));

//                function getCreateNewProcessPromise(createProcessInput) {
//                    return BusinessProcess_DynamicBusinessProcessAPIService.StartProcess(UtilsService.replaceAll($scope.bpDefinitionObj.Title, ' ', ''), createProcessInput).then(function (response) {
//                        BusinessProcess_BPInstanceService.openProcessTracking(response.ProcessId, context);
//                        $scope.modalContext.closeModal();

//                    }).catch(function (error) {
//                        VRNotificationService.notifyException(error);
//                    });
//                }

//                return UtilsService.waitMultiplePromises(promises);
//            };

//            $scope.scopeModel.close = function () {
//                $scope.modalContext.closeModal(true);
//            };
//        }

//        function load() {
//            $scope.scopeModel.isLoading = true;

//            loadAllControls()
//                .catch(function (error) {
//                    VRNotificationService.notifyExceptionWithClose(error, $scope);
//                })
//                .finally(function () {
//                    $scope.scopeModel.isLoading = false;
//                });
//        }

//        function loadAllControls() {
//            var initialPromises = [];

//            initialPromises.push(getBPDefinition());

//            function getBPDefinition() {
//                return BusinessProcess_BPDefinitionAPIService.GetBPDefintion($scope.BPDefinitionID)
//                    .then(function (response) {
//                        $scope.bpDefinitionObj = response;
//                        console.log(response);
//                    }).catch(function (error) {
//                        VRNotificationService.notifyExceptionWithClose(error, $scope);
//                    });
//            }

//            function setTitle() {
//                $scope.title = 'Start New Process';
//            }

//            function loadBPDefinition() {
//                var loadBPDefinitionPromiseDeferred = UtilsService.createPromiseDeferred();
//                bpDefinitionDirectiveReadyPromiseDeferred.promise.then(function () {
//                    var bpDefinitionPayload = { bpDefinitionId: customActionSettings.BPDefinitionId };
//                    VRUIUtilsService.callDirectiveLoad(bpDefinitionDirectiveApi, bpDefinitionPayload, loadBPDefinitionPromiseDeferred);
//                });
//                return loadBPDefinitionPromiseDeferred.promise;
//            }

//            var rootPromiseNode = {
//                promises: initialPromises,
//                getChildNode: function () {
//                    return {
//                        promises: [UtilsService.waitMultipleAsyncOperations([setTitle, loadBPDefinition])]
//                    };
//                }
//            };

//            return UtilsService.waitPromiseNode(rootPromiseNode);
//        }

//        function buildInstanceObjFromScope() {
//            var createProcessInput;
//            if (bpDefinitionDirectiveApi != undefined) {
//                createProcessInput = bpDefinitionDirectiveApi.getData();

//                if (createProcessInput == undefined)
//                    createProcessInput = {};

//                if (createProcessInput.InputArguments == undefined)
//                    createProcessInput.InputArguments = {};

//                if (customActionSettings.InputArgumentsMapping != undefined) {
//                    for (var i = 0; i < customActionSettings.InputArgumentsMapping.length; i++) {
//                        var inputArgumentMapped = customActionSettings.InputArgumentsMapping[i];
//                        createProcessInput.InputArguments[inputArgumentMapped.InputArgumentName] = parentFieldValues[inputArgumentMapped.MappedFieldName].value;
//                    }
//                }


//            }
//            return createProcessInput;
//        }

//    }

//    appControllers.controller('BusinessProcess_GenericBE_StartBPProcessEditorCustomActionController', StartBPProcessEditorCustomActionController);

//})(appControllers);
(function (appControllers) {
    "use strict";

    function beReceiveDefinitionEditorController($scope, utilsService, vrNotificationService, vrNavigationService, vruiUtilsService, beRecieveDefinitionApiService) {

        var isEditMode;
        var recevieDefinitionId;
        var receveiveDEfinitionEntity;

        var targetSynchronizerApi;
        var targetSynchronizerReadyDeferred = utilsService.createPromiseDeferred();

        var targetConvertorApi;
        var targetConvertorReadyDeferred = utilsService.createPromiseDeferred();

        var sourceReaderApi;
        var sourceReaderReadyDeferred = utilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = vrNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                recevieDefinitionId = parameters.BEReceiveDefinitionId;
            }
            isEditMode = (recevieDefinitionId != undefined);
        }
        function setTitle() {
            if (isEditMode) {
                var receiveDefinitionName = (receveiveDEfinitionEntity != undefined) ? receveiveDEfinitionEntity.Name : null;
                $scope.title = utilsService.buildTitleForUpdateEditor(receiveDefinitionName, 'ReceiveDefinition');

            }
            else {
                $scope.title = utilsService.buildTitleForAddEditor('ReceiveDefinition');
            }
        }
        function loadAllControls() {
            return utilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadTargetAnalysisDefinitions, loadTargetConvertorDefinitions, loadTSourceReaderDefinitions]).catch(function (error) {
                vrNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function getReceiveDefinition() {
            return beRecieveDefinitionApiService.GetReceiveDefinition(recevieDefinitionId).then(function (response) {
                receveiveDEfinitionEntity = response;
            });
        }
        function loadStaticData() {
            if (receveiveDEfinitionEntity == undefined)
                return;
            $scope.scopeModel.name = receveiveDEfinitionEntity.Name;
        }
        function load() {
            $scope.scopeModel.isLoading = true;
            if (isEditMode) {
                getReceiveDefinition().then(function () {
                    loadAllControls();
                }).catch(function (error) {
                    vrNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }
        }
        function buildStatusChargingSetObjFromScope() {
            var settings =
            {
                SourceBEReader: sourceReaderApi.getData(),
                TargetBEConvertor: targetConvertorApi.getData(),
                TargetBESynchronizer: targetSynchronizerApi.getData()
            }
            return {
                BEReceiveDefinitionId: receveiveDEfinitionEntity != undefined ? receveiveDEfinitionEntity.BEReceiveDefinitionId : undefined,
                Name: $scope.scopeModel.name,
                 Settings: settings
            };
        }
        function update() {
            $scope.scopeModel.isLoading = true;
            return beRecieveDefinitionApiService.UpdateReceiveDefinition(buildStatusChargingSetObjFromScope()).then(function (response) {
                if (vrNotificationService.notifyOnItemUpdated('ReceiveDefinition', response, 'Name')) {
                    if ($scope.onReceiveDefinitionUpdated != undefined) {
                        $scope.onReceiveDefinitionUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                vrNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function insert() {
            $scope.scopeModel.isLoading = true;
            return beRecieveDefinitionApiService.AddReceiveDefinition(buildStatusChargingSetObjFromScope()).then(function (response) {
                if (vrNotificationService.notifyOnItemAdded('ReceiveDefinition', response, 'Name')) {
                    if ($scope.onReceiveDefinitionAdded != undefined)
                        $scope.onReceiveDefinitionAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                vrNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }


        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.save = function () {
                if (isEditMode) {
                    return update();
                } else {
                    return insert();
                }
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
            $scope.scopeModel.onTargetSynchronizerDirectiveReady = function (api) {
                targetSynchronizerApi = api;
                targetSynchronizerReadyDeferred.resolve();
            };
            $scope.scopeModel.onTargetConvertorDirectiveReady = function (api) {
                targetConvertorApi = api;
                targetConvertorReadyDeferred.resolve();
            };
            $scope.scopeModel.onSourceReaderDirectiveReady = function (api) {
                sourceReaderApi = api;
                sourceReaderReadyDeferred.resolve();
            };
        }

        function loadTargetAnalysisDefinitions() {
            var loadTargetAnalysisPromiseDeferred = utilsService.createPromiseDeferred();
            targetSynchronizerReadyDeferred.promise.then(function () {
                var payloadDirective;
                if (receveiveDEfinitionEntity != undefined)
                    payloadDirective = { Settings: receveiveDEfinitionEntity.Settings.TargetBESynchronizer };
                vruiUtilsService.callDirectiveLoad(targetSynchronizerApi, payloadDirective, loadTargetAnalysisPromiseDeferred);
            });
            return loadTargetAnalysisPromiseDeferred.promise;
        }
        function loadTargetConvertorDefinitions() {
            var loadTargetConvertorPromiseDeferred = utilsService.createPromiseDeferred();
            targetConvertorReadyDeferred.promise.then(function () {
                var payloadDirective;
                if (receveiveDEfinitionEntity != undefined)
                    payloadDirective = { Settings: receveiveDEfinitionEntity.Settings.TargetBEConvertor };
                vruiUtilsService.callDirectiveLoad(targetConvertorApi, payloadDirective, loadTargetConvertorPromiseDeferred);
            });
            return loadTargetConvertorPromiseDeferred.promise;
        }
        function loadTSourceReaderDefinitions() {
            var loadSourceReaderPromiseDeferred = utilsService.createPromiseDeferred();
            sourceReaderReadyDeferred.promise.then(function () {
                var payloadDirective;
                if (receveiveDEfinitionEntity != undefined)
                    payloadDirective = { Settings: receveiveDEfinitionEntity.Settings.SourceBEReader };
                vruiUtilsService.callDirectiveLoad(sourceReaderApi, payloadDirective, loadSourceReaderPromiseDeferred);
            });
            return loadSourceReaderPromiseDeferred.promise;
        }
    }

    beReceiveDefinitionEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'VR_BEBridge_BERecieveDefinitionAPIService'];
    appControllers.controller('VR_BEBridge_BEReceiveDefinitionEditorController', beReceiveDefinitionEditorController);

})(appControllers);
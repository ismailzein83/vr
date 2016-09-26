(function (appControllers) {
    "use strict";

    function beReceiveDefinitionEditorController($scope, UtilsService, vrNotificationService, vrNavigationService, VRUIUtilsService, beRecieveDefinitionApiService) {

        var isEditMode;
        var entitySettings = [];
        var recevieDefinitionId;
        var receveiveDEfinitionEntity;

        var sourceReaderApi;
        var sourceReaderReadyDeferred = UtilsService.createPromiseDeferred();

        var beSettingsAPI;
        var beSettingsAPIReadyPromiseDeferred = UtilsService.createPromiseDeferred();

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
                $scope.title = UtilsService.buildTitleForUpdateEditor(receiveDefinitionName, 'ReceiveDefinition');

            }
            else {
                $scope.title = UtilsService.buildTitleForAddEditor('ReceiveDefinition');
            }
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadTSourceReaderDefinitions, loadBESyncSettings]).catch(function (error) {
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
                EntitySyncDefinitions: GetEntitySyncDefinitions()
            }
            return {
                BEReceiveDefinitionId: receveiveDEfinitionEntity != undefined ? receveiveDEfinitionEntity.BEReceiveDefinitionId : undefined,
                Name: $scope.scopeModel.name,
                Settings: settings
            };
        }

        function GetEntitySyncDefinitions() {
            var gridData = beSettingsAPI.getData();
            var settings = [];
            for (var i = 0; i < gridData.Fields.length; i++) {
                var setting = {
                    TargetBEConvertor: gridData.Fields[i].TargetBEConvertor,
                    TargetBESynchronizer: gridData.Fields[i].TargetBESynchronizer
                };
                settings.push(setting);
            }

            return settings;
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
                    if ($scope.onReceiveDefinitionAdded != undefined) {
                        $scope.onReceiveDefinitionAdded(response.InsertedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                vrNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function loadBESyncSettings() {

            var loadBESyncSettingsPromiseDeferred = UtilsService.createPromiseDeferred();

            beSettingsAPIReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload = (receveiveDEfinitionEntity != undefined)
                        ? { Fields: receveiveDEfinitionEntity.Settings.EntitySyncDefinitions }
                        : undefined;

                    VRUIUtilsService.callDirectiveLoad(beSettingsAPI, directivePayload, loadBESyncSettingsPromiseDeferred);
                });

            return loadBESyncSettingsPromiseDeferred.promise;
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

            $scope.scopeModel.onSourceReaderDirectiveReady = function (api) {
                sourceReaderApi = api;
                sourceReaderReadyDeferred.resolve();
            };

            $scope.scopeModel.onBESyncSettingsDirectiveReady = function (api) {
                beSettingsAPI = api;
                beSettingsAPIReadyPromiseDeferred.resolve();
            }
        }

        function loadTSourceReaderDefinitions() {
            var loadSourceReaderPromiseDeferred = UtilsService.createPromiseDeferred();
            sourceReaderReadyDeferred.promise.then(function () {
                var payloadDirective;
                if (receveiveDEfinitionEntity != undefined)
                    payloadDirective = { Settings: receveiveDEfinitionEntity.Settings.SourceBEReader };
                VRUIUtilsService.callDirectiveLoad(sourceReaderApi, payloadDirective, loadSourceReaderPromiseDeferred);
            });
            return loadSourceReaderPromiseDeferred.promise;
        }
    }

    beReceiveDefinitionEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'VR_BEBridge_BERecieveDefinitionAPIService'];
    appControllers.controller('VR_BEBridge_BEReceiveDefinitionEditorController', beReceiveDefinitionEditorController);

})(appControllers);
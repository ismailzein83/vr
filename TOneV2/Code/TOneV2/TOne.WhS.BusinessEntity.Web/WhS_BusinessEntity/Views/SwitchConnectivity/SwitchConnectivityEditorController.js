(function (appControllers) {

    'use strict';

    SwitchConnectivityEditorController.$inject = ['$scope', 'WhS_BE_SwitchConnectivityAPIService', 'WhS_BE_SwitchConnectivityTypeEnum', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function SwitchConnectivityEditorController($scope, WhS_BE_SwitchConnectivityAPIService, WhS_BE_SwitchConnectivityTypeEnum, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {
        var isEditMode;

        var switchConnectivityId;
        var switchConnectivityEntity;

        var carrierAccountSelectorAPI;
        var carrierAccountSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var context;
        var isViewHistoryMode;
        var switchSelectorAPI;
        var switchSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                switchConnectivityId = parameters.switchConnectivityId;
                context = parameters.context;
            }
            isViewHistoryMode = (context != undefined && context.historyId != undefined);
            isEditMode = (switchConnectivityId != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.connectionTypes = UtilsService.getArrayEnum(WhS_BE_SwitchConnectivityTypeEnum);

            $scope.scopeModel.trunks = [];

            $scope.scopeModel.onCarrierAccountSelectorReady = function (api) {
                carrierAccountSelectorAPI = api;
                carrierAccountSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onSwitchSelectorReady = function (api) {
                switchSelectorAPI = api;
                switchSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.disableAddButton = function () {
                if ($scope.scopeModel.trunkName == undefined)
                    return true;
                var index = UtilsService.getItemIndexByVal($scope.scopeModel.trunks, $scope.scopeModel.trunkName, 'Name');
                return (index > -1);
            };

            $scope.scopeModel.validateTrunks = function () {
                return ($scope.scopeModel.trunks.length == 0) ? 'Please add at least one trunk' : null;
            };

            $scope.scopeModel.addTrunk = function () {
                $scope.scopeModel.trunks.push({
                    Name: $scope.scopeModel.trunkName
                });
                $scope.scopeModel.trunkName = undefined;
            };

            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateSwitchConnectivity() : insertSwitchConnectivity();
            };

            $scope.scopeModel.hasSavePermission = function () {
                return (isEditMode) ?
                    WhS_BE_SwitchConnectivityAPIService.HasEditSwitchConnectivityPermission() :
                    WhS_BE_SwitchConnectivityAPIService.HasAddSwitchConnectivityPermission();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                getSwitchConnectivity().then(function () {
                    loadAllControls().finally(function () {
                        switchConnectivityEntity = undefined;
                    });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
            else if (isViewHistoryMode) {
                getSwitchConnectivityHistory().then(function () {
                    loadAllControls().finally(function () {
                        switchConnectivityEntity = undefined;
                    });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.isLoading = false;
                });

            }
            else {
                loadAllControls();
            }
        }
        function getSwitchConnectivityHistory() {
            return WhS_BE_SwitchConnectivityAPIService.GetSwitchConnectivityHistoryDetailbyHistoryId(context.historyId).then(function (response) {
                switchConnectivityEntity = response;

            });
        }
        function getSwitchConnectivity() {
            return WhS_BE_SwitchConnectivityAPIService.GetSwitchConnectivity(switchConnectivityId).then(function (response) {
                switchConnectivityEntity = response;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadCarrierAccountSelector, loadSwitchSelector]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function setTitle() {
            if (isEditMode) {
                if (switchConnectivityEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(switchConnectivityEntity.Name, 'Switch Connectivity');
            }
            else if (isViewHistoryMode && switchConnectivityEntity != undefined)
                $scope.title = "View Switch Connectivity: " + switchConnectivityEntity.Name;
            else
                $scope.title = UtilsService.buildTitleForAddEditor('Switch Connectivity');
        }

        function loadStaticData() {
            if (switchConnectivityEntity == undefined)
                return;
            $scope.scopeModel.name = switchConnectivityEntity.Name;
            $scope.scopeModel.selectedConnectionType = UtilsService.getItemByVal($scope.scopeModel.connectionTypes, switchConnectivityEntity.Settings.ConnectionType, 'value');
            $scope.scopeModel.beginEffectiveDate = switchConnectivityEntity.BED;
            $scope.scopeModel.endEffectiveDate = switchConnectivityEntity.EED;

            // Load trunks
            if (switchConnectivityEntity.Settings != null && switchConnectivityEntity.Settings.Trunks != null) {
                for (var i = 0; i < switchConnectivityEntity.Settings.Trunks.length; i++) {
                    var item = switchConnectivityEntity.Settings.Trunks[i];
                    $scope.scopeModel.trunks.push(item);
                }
            }
        }

        function loadCarrierAccountSelector() {
            var carrierAccountSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            carrierAccountSelectorReadyDeferred.promise.then(function () {
                var payload = (switchConnectivityEntity != undefined) ? { selectedIds: switchConnectivityEntity.CarrierAccountId } : undefined;
                VRUIUtilsService.callDirectiveLoad(carrierAccountSelectorAPI, payload, carrierAccountSelectorLoadDeferred);
            });

            return carrierAccountSelectorLoadDeferred.promise;
        }

        function loadSwitchSelector() {
            var switchSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            switchSelectorReadyDeferred.promise.then(function () {
                var payload = (switchConnectivityEntity != undefined) ? { selectedIds: switchConnectivityEntity.SwitchId } : undefined;
                VRUIUtilsService.callDirectiveLoad(switchSelectorAPI, payload, switchSelectorLoadDeferred);
            });

            return switchSelectorLoadDeferred.promise;
        }

        function insertSwitchConnectivity() {
            $scope.scopeModel.isLoading = true;
            return WhS_BE_SwitchConnectivityAPIService.AddSwitchConnectivity(buildSwitchConnectivityObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('Switch Connectivity', response, 'Name')) {
                    if ($scope.onSwitchConnectivityAdded != undefined)
                        $scope.onSwitchConnectivityAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function updateSwitchConnectivity() {
            $scope.scopeModel.isLoading = true;
            return WhS_BE_SwitchConnectivityAPIService.UpdateSwitchConnectivity(buildSwitchConnectivityObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('Switch Connectivity', response, 'Name')) {
                    if ($scope.onSwitchConnectivityUpdated != undefined)
                        $scope.onSwitchConnectivityUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildSwitchConnectivityObjFromScope() {
            var obj = {
                SwitchConnectivityId: switchConnectivityId,
                Name: $scope.scopeModel.name,
                CarrierAccountId: carrierAccountSelectorAPI.getSelectedIds(),
                SwitchId: switchSelectorAPI.getSelectedIds(),
                Settings: {
                    ConnectionType: $scope.scopeModel.selectedConnectionType.value,
                    Trunks: $scope.scopeModel.trunks
                },
                BED: $scope.scopeModel.beginEffectiveDate,
                EED: $scope.scopeModel.endEffectiveDate
            };
            return obj;
        }
    }

    appControllers.controller('WhS_BE_SwitchConnectivityEditorController', SwitchConnectivityEditorController);

})(appControllers);
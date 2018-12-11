(function (appControllers) {

    "use strict";

    portalAccountEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'WhS_BE_PortalCarrierAccountTypeEnum', 'WhS_BE_PortalAccountAPIService', 'WhS_BE_PortalAccountService','VR_Sec_RemoteGroupAPIService'];

    function portalAccountEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, WhS_BE_PortalCarrierAccountTypeEnum, WhS_BE_PortalAccountAPIService, WhS_BE_PortalAccountService, VR_Sec_RemoteGroupAPIService) {

        var carrierAccountSelectorAPI;
        var carrierAccountSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var portalCarrierAccountTypeSelectedDefered;

        var remoteGroupSelectorAPI;
        var remoteGroupSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var userId;
        var portalAccountEntity;
        var groupIds;
        var isEditMode;
        var carrierProfileId;
        var context;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                carrierProfileId = parameters.carrierProfileId;
                context = parameters.context;
                userId = parameters.userId;
            }
            isEditMode = (userId != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.portalCarrierAccountTypes = UtilsService.getArrayEnum(WhS_BE_PortalCarrierAccountTypeEnum);
            $scope.scopeModel.selectedPortalCarrierAccountType = WhS_BE_PortalCarrierAccountTypeEnum.All;

            $scope.scopeModel.onCarrierAccountSelectorReady = function (api) {
                carrierAccountSelectorAPI = api;
                carrierAccountSelectorReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.onRemoteGroupSelectorReady = function (api) {
                remoteGroupSelectorAPI = api;
                remoteGroupSelectorReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.onPortalCarrierAccountTypeSelectionChanged = function (value) {
                if (value != undefined) {
                    if (portalCarrierAccountTypeSelectedDefered != undefined) {
                        portalCarrierAccountTypeSelectedDefered.resolve();
                    }
                    else {
                        loadCarrierAccountSelector();
                    }
                }
            };
            $scope.scopeModel.save = function () {
                if (isEditMode)
                    updatePortalAccount();
                else
                    insertPortalAccount();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

        }
        function load() {
            if (isEditMode) {
                $scope.scopeModel.isLoading = true;
                portalCarrierAccountTypeSelectedDefered = UtilsService.createPromiseDeferred();
                WhS_BE_PortalAccountAPIService.GetPortalAccountEditorRuntime(carrierProfileId, userId).then(function (response) {
                    portalAccountEntity = response.CarrierProfilePortalAccount;
                    groupIds = response.GroupIds;
                    loadAllControls();
                });
            }
            else {
                loadAllControls();
            }
        }



        function loadAllControls() {

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadRemoteGroupSelectorDirective, loadCarrierAccountSelector])
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
                .finally(function () {
                    portalCarrierAccountTypeSelectedDefered = undefined;
                    $scope.scopeModel.isLoading = false;
                });
        }

        function loadRemoteGroupSelectorDirective() {

            var promises = [];
            var remoteGroupSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            var connectionId = WhS_BE_PortalAccountService.getPortalConnectionId();
            promises.push(remoteGroupSelectorReadyPromiseDeferred.promise);
            UtilsService.waitMultiplePromises(promises).then(function (response) {
                var remoteGroupPayload = {
                    connectionId: connectionId,
                    selectedIds: groupIds
                };
                VRUIUtilsService.callDirectiveLoad(remoteGroupSelectorAPI, remoteGroupPayload, remoteGroupSelectorLoadPromiseDeferred);
            });
            return remoteGroupSelectorLoadPromiseDeferred.promise;
        }
        function loadCarrierAccountSelector() {

            function getCarrierAccountsSelectedIds() {
                var selectedIds;
                if (portalAccountEntity.CarrierAccounts != undefined && portalAccountEntity.CarrierAccounts.length > 0) {
                    selectedIds = [];
                    for (var i = 0; i < portalAccountEntity.CarrierAccounts.length; i++) {
                        selectedIds.push(portalAccountEntity.CarrierAccounts[i].CarrierAccountId);
                    }
                }
                return selectedIds;
            }
            var carrierAccountLoadPromisDeferred = UtilsService.createPromiseDeferred();
            carrierAccountSelectorReadyPromiseDeferred.promise.then(function () {
                var payload = {
                    filter: {
                        CarrierProfileId: carrierProfileId
                    },
                    selectedIds: portalAccountEntity != undefined ? getCarrierAccountsSelectedIds() : undefined
                };
                VRUIUtilsService.callDirectiveLoad(carrierAccountSelectorAPI, payload, carrierAccountLoadPromisDeferred);
                portalAccountEntity = undefined;
            });
        }
        function setTitle() {
            if (!isEditMode)
                $scope.title = UtilsService.buildTitleForAddEditor('Portal Account');
            else
                $scope.title = UtilsService.buildTitleForUpdateEditor(portalAccountEntity.Name, 'Portal Account');
        }
        function loadStaticData() {
            if (portalAccountEntity == undefined)
                return;
            if (portalAccountEntity != undefined) {
                $scope.scopeModel.name = portalAccountEntity.Name;
                $scope.scopeModel.email = portalAccountEntity.Email;
                $scope.scopeModel.selectedPortalCarrierAccountType = UtilsService.getItemByVal($scope.scopeModel.portalCarrierAccountTypes, portalAccountEntity.Type, "value");
            }
        }
        
       
        function insertPortalAccount() {
            $scope.scopeModel.isLoading = true;
            return WhS_BE_PortalAccountAPIService.AddPortalAccount(buildPortalAccountObjFromScope())
                .then(function (response) {
                    if (VRNotificationService.notifyOnItemAdded("Portal Account", response, "Email")) {
                        if ($scope.onPortalAccountAdded != undefined)
                            $scope.onPortalAccountAdded(response.InsertedObject);
                        $scope.modalContext.closeModal();
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
        }
        function updatePortalAccount() {
            $scope.scopeModel.isLoading = true;

            return WhS_BE_PortalAccountAPIService.UpdatePortalAccount(buildPortalAccountObjFromScope())
                .then(function (response) {
                    if (VRNotificationService.notifyOnItemUpdated("Portal Account", response, "Email")) {
                        if ($scope.onPortalAccountUpdated != undefined)
                            $scope.onPortalAccountUpdated(response.UpdatedObject);
                        $scope.modalContext.closeModal();
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
        }

        function buildPortalAccountObjFromScope() {
            var obj = {
                CarrierProfileId: carrierProfileId,
                GroupIds: remoteGroupSelectorAPI.getSelectedIds(),
                Entity: {
                    Name: $scope.scopeModel.name,
                    Email: $scope.scopeModel.email,
                    Type: $scope.scopeModel.selectedPortalCarrierAccountType.value,
                    CarrierAccounts: $scope.scopeModel.selectedPortalCarrierAccountType == WhS_BE_PortalCarrierAccountTypeEnum.Specific ?  getSelectedCarrierAccounts() : undefined
                }
            };
            if (isEditMode)
                obj.Entity.UserId = userId;
            return obj;
        }
        function getSelectedCarrierAccounts() {
            var carrierAccounts;
            if ($scope.scopeModel.selectedCarrierAccounts != undefined && $scope.scopeModel.selectedCarrierAccounts.length > 0) {
                carrierAccounts = [];
                for (var i = 0; i < $scope.scopeModel.selectedCarrierAccounts.length; i++) {
                    carrierAccounts.push({
                        CarrierAccountId: $scope.scopeModel.selectedCarrierAccounts[i].CarrierAccountId
                    });
                }
            }
            return carrierAccounts;
        }
    }

    appControllers.controller('WhS_BE_PortalAccountEditorController', portalAccountEditorController);

})(appControllers);

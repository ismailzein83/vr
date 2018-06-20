(function (appControllers) {

    'use strict';

    TenantEditorController.$inject = ['$scope', 'VR_Sec_TenantAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VR_Sec_SecurityAPIService', 'VRUIUtilsService'];

    function TenantEditorController($scope, VR_Sec_TenantAPIService, VRNotificationService, VRNavigationService, UtilsService, VR_Sec_SecurityAPIService, VRUIUtilsService) {
        var isEditMode;
        var tenantId;
        var tenantEntity;
        var tenantSelectorAPI;
        var tenantReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null)
                tenantId = parameters.tenantId;

            isEditMode = (tenantId != undefined);
        }

        function defineScope() {
            $scope.save = function () {
                if (isEditMode)
                    return updateTenant();
                else
                    return insertTenant();
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };

            $scope.hasSaveTenantPermission = function () {
                if (isEditMode) {
                    return VR_Sec_TenantAPIService.HasUpdateTenantPermission();
                }
                else {
                    return VR_Sec_TenantAPIService.HasAddTenantPermission();
                }
            };

            $scope.onTenantSelectorReady = function (api) {
                tenantSelectorAPI = api;
                tenantReadyPromiseDeferred.resolve();
            };
        }
        function load() {
            $scope.isLoading = true;

            if (isEditMode) {
                getTenant().then(function () {
                    loadAllControls().finally(function () {
                        tenantEntity = undefined;
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

        function getTenant() {
            return VR_Sec_TenantAPIService.GetTenantbyId(tenantId).then(function (response) {
                tenantEntity = response;
                $scope.isInEditMode = true;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadTenantSelector])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }

        function loadTenantSelector() {
            var loadTenantPromiseDeferred = UtilsService.createPromiseDeferred();

            var payload = undefined;
            if (tenantEntity != undefined) {
                payload = { selectedIds: tenantEntity.ParentTenantId, filter: { CanBeParentOfTenantId: tenantEntity.TenantId } };
            }
            tenantReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(tenantSelectorAPI, payload, loadTenantPromiseDeferred);
            });

            return loadTenantPromiseDeferred.promise;
        }

        function setTitle() {
            if (isEditMode && tenantEntity != undefined)
                $scope.title = UtilsService.buildTitleForUpdateEditor(tenantEntity.Name, 'Tenant');
            else
                $scope.title = UtilsService.buildTitleForAddEditor('Tenant');
        }

        function loadStaticData() {

            if (tenantEntity == undefined)
                return;

            $scope.name = tenantEntity.Name;
        }

        function buildTenantObjFromScope() {
            var tenantObject = {
                TenantId: (tenantId != null) ? tenantId : 0,
                Name: $scope.name,
                ParentTenantId: tenantSelectorAPI.getSelectedIds()
            };
            return tenantObject;
        }

        function insertTenant() {
            $scope.isLoading = true;

            var tenantObject = buildTenantObjFromScope();

            return VR_Sec_TenantAPIService.AddTenant(tenantObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('Tenant', response, 'Name')) {
                    if ($scope.onTenantAdded != undefined)
                        $scope.onTenantAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });

        }

        function updateTenant() {
            $scope.isLoading = true;

            var tenantObject = buildTenantObjFromScope();

            return VR_Sec_TenantAPIService.UpdateTenant(tenantObject).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('Tenant', response, 'Name')) {
                    if ($scope.onTenantUpdated != undefined)
                        $scope.onTenantUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }
    }

    appControllers.controller('VR_Sec_TenantEditorController', TenantEditorController);

})(appControllers);

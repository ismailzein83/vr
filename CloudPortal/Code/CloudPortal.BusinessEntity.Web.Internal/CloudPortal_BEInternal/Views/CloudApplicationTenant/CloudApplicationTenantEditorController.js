CloudPortal_BEInternal_CloudApplicationTenantEditorController.$inject = ['$scope', 'CloudPortal_BEInternal_CloudApplicationTenantAPIService', 'VRNotificationService', 'UtilsService', 'VRNavigationService', 'VRUIUtilsService'];

function CloudPortal_BEInternal_CloudApplicationTenantEditorController($scope, CloudPortal_BEInternal_CloudApplicationTenantAPIService, VRNotificationService, UtilsService, VRNavigationService, VRUIUtilsService) {
    var cloudApplicationId;
    var cloudApplicationTenantId;
    var isEditMode;

    var cloudApplicationTenantEntity;

    var tenantSelectorAPI;
    var tenantReadyPromiseDeferred = UtilsService.createPromiseDeferred();
    defineScope();
    loadParameters();

    load();

    function defineScope() {

        $scope.close = function () {
            $scope.modalContext.closeModal()
        };

        $scope.assignCloudApplicationTenant = function () {
            var cloudApplicationTenantObj = buildCloudApplicationTenantObjFromScope();
            if (isEditMode) {
                CloudPortal_BEInternal_CloudApplicationTenantAPIService.UpdateCloudApplicationTenant(cloudApplicationTenantObj).then(function (response) {
                    if (VRNotificationService.notifyOnItemUpdated("Cloud Application Tenant", response)) {
                        $scope.modalContext.closeModal();
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error);
                });
            }
            else {
                CloudPortal_BEInternal_CloudApplicationTenantAPIService.AssignCloudApplicationTenant(cloudApplicationTenantObj).then(function (response) {
                    if (VRNotificationService.notifyOnItemAdded("Cloud Application Tenant", response)) {
                        if ($scope.onTenantAssignedToCloudApplication != undefined && response.InsertedObject != undefined) {
                            $scope.onTenantAssignedToCloudApplication(cloudApplicationTenantObj);
                        }
                        $scope.modalContext.closeModal();
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error);
                });
            }
        };

        $scope.onTenantSelectorReady = function (api) {
            tenantSelectorAPI = api;
            tenantReadyPromiseDeferred.resolve();
        };

        $scope.hasAssignCloudApplicationTenantPermission = function () {
            if (isEditMode) {
                return CloudPortal_BEInternal_CloudApplicationTenantAPIService.HasUpdateCloudApplicationTenantPermission();
            }
            else {
                return CloudPortal_BEInternal_CloudApplicationTenantAPIService.HasAssignCloudApplicationTenantPermission();
            }
        }
    }

    function buildCloudApplicationTenantObjFromScope() {

        var settings = {};
        var cloudTenantSettings = { LicenseExpiresOn: $scope.expiresOn };
        var obj = {
            CloudApplicationTenantId: cloudApplicationTenantId,
            ApplicationId: cloudApplicationId,
            TenantId: tenantSelectorAPI.getSelectedIds(),
            Settings: settings,
            CloudTenantSettings: cloudTenantSettings
        };
        return obj;
    }

    function load() {
        $scope.isLoading = true;

        if (isEditMode) {
            getCloudApplicationTenant().then(function () {
                loadAllControls().finally(function () {
                    cloudApplicationTenantEntity = undefined;
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

    function getCloudApplicationTenant() {
        return CloudPortal_BEInternal_CloudApplicationTenantAPIService.GetCloudApplicationTenant(cloudApplicationTenantId).then(function (response) {
            cloudApplicationTenantEntity = response;
            $scope.isInEditMode = true;
            cloudApplicationId = cloudApplicationTenantEntity.ApplicationId;
        });
    }

    function loadAllControls() {
        $scope.isLoading = true;
        return UtilsService.waitMultipleAsyncOperations([setTitle, setData, loadTenantSelector])
           .catch(function (error) {
               VRNotificationService.notifyExceptionWithClose(error, $scope);
           })
          .finally(function () {
              $scope.isLoading = false;
          });
    }

    function loadTenantSelector() {
        var loadTenantPromiseDeferred = UtilsService.createPromiseDeferred();

        var tenantFilter = {
            $type: "CloudPortal.BusinessEntity.MainExtensions.CloudApplicationTenantFilter,CloudPortal.BusinessEntity.MainExtensions",
            ApplicationId: cloudApplicationId
        };

        var payload = { filter: { Filters: [] } };
        payload.filter.Filters.push(tenantFilter);

        if (cloudApplicationTenantEntity != undefined) {
            payload.selectedIds = cloudApplicationTenantEntity.TenantId;
        }
        tenantReadyPromiseDeferred.promise.then(function () {
            VRUIUtilsService.callDirectiveLoad(tenantSelectorAPI, payload, loadTenantPromiseDeferred);
        });

        return loadTenantPromiseDeferred.promise;
    }

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);

        if (parameters != undefined && parameters != null) {
            cloudApplicationId = parameters.CloudApplicationId;
            cloudApplicationTenantId = parameters.CloudApplicationTenantId;
        }

        if (cloudApplicationTenantId != undefined)
            isEditMode = true;
    }


    function setTitle() {
        if (cloudApplicationTenantEntity != undefined) {
            $scope.title = 'Edit Tenant';
        }
        else {
            $scope.title = 'Assign Tenant';
        }
    }

    function setData() {
        if (cloudApplicationTenantEntity != undefined) {
            $scope.expiresOn = cloudApplicationTenantEntity.CloudTenantSettings.LicenseExpiresOn;
        }
    }
}

appControllers.controller('CloudPortal_BEInternal_CloudApplicationTenantEditorController', CloudPortal_BEInternal_CloudApplicationTenantEditorController);

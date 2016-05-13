CloudPortal_BEInternal_CloudApplicationUserEditorController.$inject = ['$scope', 'CloudPortal_BEInternal_CloudApplicationUserAPIService', 'VRNotificationService', 'UtilsService', 'VRNavigationService'];

function CloudPortal_BEInternal_CloudApplicationUserEditorController($scope, CloudPortal_BEInternal_CloudApplicationUserAPIService, VRNotificationService, UtilsService, VRNavigationService) {
    var cloudApplicationTenantId;
    var tenantId;
    var userSelectorAPI;

    defineScope();
    loadParameters();
    loadAllControls();


    function defineScope() {

        $scope.close = function () {
            $scope.modalContext.closeModal()
        };

        $scope.assignCloudApplicationUser = function () {
            var cloudApplicationUserObj = buildCloudApplicationUserObjFromScope();
            if ($scope.hasFullPermission) {
                CloudPortal_BEInternal_CloudApplicationUserAPIService.AssignCloudApplicationUserWithPermission(cloudApplicationUserObj).then(function (response) {
                    if (VRNotificationService.notifyOnItemAdded("Cloud Application User", response)) {
                        if ($scope.onUserAssignedToCloudApplication != undefined && response.InsertedObject != undefined) {
                            $scope.onUserAssignedToCloudApplication(cloudApplicationUserObj);
                        }
                        $scope.modalContext.closeModal();
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error);
                });
            }
            else {
                CloudPortal_BEInternal_CloudApplicationUserAPIService.AssignCloudApplicationUser(cloudApplicationUserObj).then(function (response) {
                    if (VRNotificationService.notifyOnItemAdded("Cloud Application User", response)) {
                        if ($scope.onUserAssignedToCloudApplication != undefined && response.InsertedObject != undefined) {
                            $scope.onUserAssignedToCloudApplication(cloudApplicationUserObj);
                        }
                        $scope.modalContext.closeModal();
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error);
                });
            }
        };

        $scope.hasAssignCloudApplicationUserPermission = function () {
            return CloudPortal_BEInternal_CloudApplicationUserAPIService.HasAssignCloudApplicationUserPermission();
        }

        $scope.onUserSelectorReady = function (api) {
            userSelectorAPI = api;

            var userFilter = {
                $type: "CloudPortal.BusinessEntity.MainExtensions.CloudApplicationUserFilter,CloudPortal.BusinessEntity.MainExtensions",
                CloudApplicationTenantId: cloudApplicationTenantId
            };

            var payload = { filter: { GetOnlyTenantUsers: true, TenantId: tenantId, Filters: [] } };
            payload.filter.Filters.push(userFilter);
            userSelectorAPI.load(payload);
        };
    }

    function buildCloudApplicationUserObjFromScope() {
        var obj = {
            CloudApplicationTenantID: cloudApplicationTenantId,
            UserIds: userSelectorAPI.getSelectedIds()
        };
        return obj;
    }

    function loadAllControls() {
        $scope.isLoading = true;
        return UtilsService.waitMultipleAsyncOperations([setTitle])
           .catch(function (error) {
               VRNotificationService.notifyExceptionWithClose(error, $scope);
           })
          .finally(function () {
              $scope.isLoading = false;
          });
    }


    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);

        if (parameters != undefined && parameters != null) {
            cloudApplicationTenantId = parameters.CloudApplicationTenantId;
            tenantId = parameters.TenantId;
        }
    }


    function setTitle() {
        $scope.title = 'Assign User';
    }
}

appControllers.controller('CloudPortal_BEInternal_CloudApplicationUserEditorController', CloudPortal_BEInternal_CloudApplicationUserEditorController);

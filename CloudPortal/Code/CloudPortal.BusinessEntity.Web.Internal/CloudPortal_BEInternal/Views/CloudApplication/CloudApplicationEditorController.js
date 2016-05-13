CloudPortal_BEInternal_CloudApplicationEditorController.$inject = ['$scope', 'CloudPortal_BEInternal_CloudApplicationAPIService', 'VRNotificationService', 'UtilsService', 'VRNavigationService', 'CloudPortal_BEInternal_CloudApplicationTypeAPIService'];

function CloudPortal_BEInternal_CloudApplicationEditorController($scope, CloudPortal_BEInternal_CloudApplicationAPIService, VRNotificationService, UtilsService, VRNavigationService, CloudPortal_BEInternal_CloudApplicationTypeAPIService) {
    var isEditMode;
    var cloudApplicationId;
    var cloudApplication;

    defineScope();
    loadParameters();
    load();

    function defineScope() {
        $scope.cloudApplicationTypes = [];
        $scope.close = function () {
            $scope.modalContext.closeModal()
        };

        $scope.saveCloudApplication = function () {
            var cloudApplicationObj = buildCloudApplicationObjFromScope();
            if (isEditMode) {
                CloudPortal_BEInternal_CloudApplicationAPIService.UpdateCloudApplication(cloudApplicationObj).then(function (response) {
                    if (VRNotificationService.notifyOnItemUpdated("Cloud Application", response)) {
                        if ($scope.onCloudApplicationUpdated != undefined) {
                            $scope.onCloudApplicationUpdated(cloudApplicationObj);
                        }
                        $scope.modalContext.closeModal();
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error);
                });
            }
            else {
                CloudPortal_BEInternal_CloudApplicationAPIService.AddCloudApplication(cloudApplicationObj).then(function (response) {
                    if (VRNotificationService.notifyOnItemAdded("Cloud Application", response)) {
                        if ($scope.onCloudApplicationAdded != undefined && response.InsertedObject != undefined) {
                            cloudApplicationObj.CloudApplicationId = response.InsertedObject.CloudApplicationId;
                            $scope.onCloudApplicationAdded(cloudApplicationObj);
                        }
                        $scope.modalContext.closeModal();
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error);
                });
            }
        };

        $scope.hasSaveCloudApplicationPermission = function () {
            if (isEditMode) {
                return CloudPortal_BEInternal_CloudApplicationAPIService.HasUpdateCloudApplicationPermission();
            }
            else {
                return CloudPortal_BEInternal_CloudApplicationAPIService.HasAddCloudApplicationPermission();
            }
        }
    }

    function buildCloudApplicationObjFromScope() {

        var settings = {
            OnlineURL: $scope.onlineURL,
            InternalURL: $scope.internalURL
        };
        var obj = {
            CloudApplicationId: cloudApplicationId,
            Name: $scope.name,
            Settings: settings,
            CloudApplicationTypeId: $scope.selectedCloudApplicationType.CloudApplicationTypeId
        };
        return obj;
    }

    function load() {
        $scope.isLoading = true;

        if (isEditMode) {
            getCloudApplication().then(function () {
                loadAllControls()
                    .finally(function () {
                        cloudApplication = undefined;
                    });
            }).catch(function () {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.isLoading = false;
            });
        }
        else {
            loadAllControls();
        }
    }

    function getCloudApplication() {
        return CloudPortal_BEInternal_CloudApplicationAPIService.GetCloudApplication(cloudApplicationId).then(function (response) {
            cloudApplication = response;
        });

    }

    function loadAllControls() {
        $scope.isLoading = true;
        return UtilsService.waitMultipleAsyncOperations([setTitle, loadData, loadCloudApplicationTypeSelector])
           .catch(function (error) {
               VRNotificationService.notifyExceptionWithClose(error, $scope);
           })
          .finally(function () {
              $scope.isLoading = false;
          });
    }

    function loadCloudApplicationTypeSelector() {
        var serializedFilter = {};
        return CloudPortal_BEInternal_CloudApplicationTypeAPIService.GetCloudApplicationTypesInfo(serializedFilter).then(function (response) {
            $scope.cloudApplicationTypes.length = 0;
            angular.forEach(response, function (itm) {
                $scope.cloudApplicationTypes.push(itm);
            });

            if (isEditMode) {
                $scope.selectedCloudApplicationType = UtilsService.getItemByVal($scope.cloudApplicationTypes, cloudApplication.CloudApplicationTypeId, "CloudApplicationTypeId");
            }
        });
    }

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);

        if (parameters != undefined && parameters != null) {
            cloudApplicationId = parameters.CloudApplicationId;
        }

        $scope.isReadOnly = isEditMode = cloudApplicationId != undefined ? true : false;
    }

    function loadData() {
        if (cloudApplication == undefined)
            return;
        $scope.name = cloudApplication.Name;
        $scope.onlineURL = cloudApplication.Settings.OnlineURL;
        $scope.internalURL = cloudApplication.Settings.InternalURL;
    }

    function setTitle() {
        if (isEditMode)
            $scope.title = 'Edit Cloud Application: ' + cloudApplication.Name;
        else
            $scope.title = 'Add Cloud Application';
    }


}

appControllers.controller('CloudPortal_BEInternal_CloudApplicationEditorController', CloudPortal_BEInternal_CloudApplicationEditorController);

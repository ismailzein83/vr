CloudPortal_BEInternal_CloudApplicationTypeEditorController.$inject = ['$scope', 'CloudPortal_BEInternal_CloudApplicationTypeAPIService', 'VRNotificationService', 'UtilsService', 'VRNavigationService'];

function CloudPortal_BEInternal_CloudApplicationTypeEditorController($scope, CloudPortal_BEInternal_CloudApplicationTypeAPIService, VRNotificationService, UtilsService, VRNavigationService) {
    var isEditMode;
    var cloudApplicationTypeId;
    var cloudApplicationType;

    defineScope();
    loadParameters();
    load();

    function defineScope() {

        $scope.close = function () {
            $scope.modalContext.closeModal()
        };

        $scope.saveCloudApplicationType = function () {
            var cloudApplicationTypeObj = buildCloudApplicationTypeObjFromScope();
            if (isEditMode) {
                CloudPortal_BEInternal_CloudApplicationTypeAPIService.UpdateCloudApplicationType(cloudApplicationTypeObj).then(function (response) {
                    if (VRNotificationService.notifyOnItemUpdated("Cloud Application Type", response)) {
                        if ($scope.onCloudApplicationTypeUpdated != undefined) {
                            $scope.onCloudApplicationTypeUpdated(cloudApplicationTypeObj);
                        }
                        $scope.modalContext.closeModal();
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error);
                });
            }
            else {
                CloudPortal_BEInternal_CloudApplicationTypeAPIService.AddCloudApplicationType(cloudApplicationTypeObj).then(function (response) {
                    if (VRNotificationService.notifyOnItemAdded("Cloud Application Type", response)) {
                        if ($scope.onCloudApplicationTypeAdded != undefined && response.InsertedObject != undefined) {
                            cloudApplicationTypeObj.CloudApplicationTypeId = response.InsertedObject.CloudApplicationTypeId;
                            $scope.onCloudApplicationTypeAdded(cloudApplicationTypeObj);
                        }
                        $scope.modalContext.closeModal();
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error);
                });
            }
        };
    }

    function buildCloudApplicationTypeObjFromScope() {
        var obj = {
            CloudApplicationTypeId: cloudApplicationTypeId,
            Name: $scope.name
        };
        return obj;
    }

    function load() {
        $scope.isLoading = true;

        if (isEditMode) {
            getApplicationType().then(function () {
                loadAllControls()
                    .finally(function () {
                        cloudApplicationType = undefined;
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

    function getApplicationType() {
        return CloudPortal_BEInternal_CloudApplicationTypeAPIService.GetCloudApplicationType(cloudApplicationTypeId).then(function (response) {
            cloudApplicationType = response;
        });

    }

    function loadAllControls() {
        $scope.isLoading = true;
        return UtilsService.waitMultipleAsyncOperations([setTitle, loadData])
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
            cloudApplicationTypeId = parameters.CloudApplicationTypeId;
        }

        isEditMode = cloudApplicationTypeId != undefined ? true : false;
    }

    function loadData() {
        if (cloudApplicationType == undefined)
            return;
        $scope.name = cloudApplicationType.Name;
    }

    function setTitle() {
        if (isEditMode)
            $scope.title = 'Edit Cloud Application Type: ' + cloudApplicationType.Name;
        else
            $scope.title = 'Add Cloud Application Type';
    }


}

appControllers.controller('CloudPortal_BEInternal_CloudApplicationTypeEditorController', CloudPortal_BEInternal_CloudApplicationTypeEditorController);

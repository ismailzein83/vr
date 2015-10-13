TypeEditorController.$inject = ["$scope", "TypeAPIService", "VRNavigationService", "VRNotificationService", "VRModalService"];

function TypeEditorController($scope, TypeAPIService, VRNavigationService, VRNotificationService, VRModalService) {

    var typeId = undefined;
    var editMode = undefined;

    loadParameters();
    defineScope();
    load();

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);

        if (parameters != undefined && parameters != null)
            typeId = parameters.TypeId;

        editMode = (typeId != undefined);
    }

    function defineScope() {

        $scope.name = undefined;

        $scope.saveType = function () {
            if (editMode)
                return updateType();
            else
                return insertType();
        }

        $scope.close = function () {
            $scope.modalContext.closeModal()
        }
    }

    function load() {

        if (editMode) {
            $scope.isGettingData = true;

            TypeAPIService.GetTypeById(typeId)
                .then(function (response) {
                    fillScopeFromTypeObj(response);
                })
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
                .finally(function () {
                    $scope.isGettingData = false;
                });
        }
    }

    function fillScopeFromTypeObj(typeObj) {
        $scope.name = typeObj.Name;
    }

    function updateType() {
        var typeObj = buildTypeObjFromScope();

        return TypeAPIService.UpdateType(typeObj)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Switch Type", response, "Name")) {
                    if ($scope.onTypeUpdated != undefined)
                        $scope.onTypeUpdated(response.UpdatedObject);

                    $scope.modalContext.closeModal();
                }
            })
            .catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
    }

    function insertType() {
        var typeObj = buildTypeObjFromScope();

        return TypeAPIService.AddType(typeObj)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Switch Type", response, "Name")) {
                    if ($scope.onTypeAdded != undefined)
                        $scope.onTypeAdded(response.InsertedObject);

                    $scope.modalContext.closeModal();
                }
            })
            .catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            });
    }

    function buildTypeObjFromScope() {
        return {
            TypeId: typeId,
            Name: $scope.name
        };
    }
}

appControllers.controller("PSTN_BusinessEntity_TypeEditorController", TypeEditorController);

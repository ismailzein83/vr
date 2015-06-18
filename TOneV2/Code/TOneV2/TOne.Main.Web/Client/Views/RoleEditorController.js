RoleEditorController.$inject = ['$scope', 'RolesAPIService', '$routeParams', 'notify', 'VRModalService', 'VRNotificationService', 'VRNavigationService'];
function RoleEditorController($scope, RolesAPIService, $routeParams, notify, VRModalService, VRNotificationService, VRNavigationService) {

    var editMode;
    loadParameters();
    defineScope();
    load();

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);
        $scope.RoleId = undefined;
        if (parameters != undefined && parameters != null)
            $scope.RoleId = parameters.RoleId;

        if ($scope.RoleId != undefined)
            editMode = true;
        else
            editMode = false;
    }

    function defineScope() {
        $scope.SaveRole = function () {
            if (editMode) {
                return updateRole();
            }
            else {
                return insertRole();
            }
        };

        $scope.close = function () {
            $scope.modalContext.closeModal()
        };
    }

    function load() {
        if (editMode) {
            $scope.isGettingData = true;
            getRole().finally(function () {
                $scope.isGettingData = false;
            })
        }
    }

    function getRole() {
        return RolesAPIService.GetRole($scope.RoleId)
           .then(function (response) {
               fillScopeFromRoleObj(response);
           })
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
    }



    function buildRoleObjFromScope() {
        var RoleObject = {
            RoleId: ($scope.RoleId != null) ? $scope.RoleId : 0,
            name: $scope.name,
            description: $scope.description
        };
        return RoleObject;
    }

    function fillScopeFromRoleObj(RoleObject) {
        $scope.name = RoleObject.Name;
        $scope.description = RoleObject.Description;
    }

    function insertRole() {
        $scope.issaving = true;
        var RoleObject = buildRoleObjFromScope();
        return RolesAPIService.AddRole(RoleObject)
        .then(function (response) {
            if (VRNotificationService.notifyOnItemAdded("Role", response)) {
                if ($scope.onRoleAdded != undefined)
                    $scope.onRoleAdded(response.InsertedObject);
                $scope.modalContext.closeModal();
            }
        }).catch(function (error) {
            VRNotificationService.notifyException(error, $scope);
        });

    }

    function updateRole() {
        var RoleObject = buildRoleObjFromScope();
        RolesAPIService.UpdateRole(RoleObject)
        .then(function (response) {
            if (VRNotificationService.notifyOnItemUpdated("Role", response)) {
                if ($scope.onRoleUpdated != undefined)
                    $scope.onRoleUpdated(response.UpdatedObject);
                $scope.modalContext.closeModal();
            }
        }).catch(function (error) {
            VRNotificationService.notifyException(error, $scope);
        });
    }
}
appControllers.controller('RoleEditorController', RoleEditorController);

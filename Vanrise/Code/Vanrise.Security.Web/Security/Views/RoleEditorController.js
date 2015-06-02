RoleEditorController.$inject = ['$scope', 'RoleAPIService', 'VRModalService', 'VRNotificationService', 'VRNavigationService'];
function RoleEditorController($scope, RoleAPIService, VRModalService, VRNotificationService, VRNavigationService) {

    var editMode;
    loadParameters();
    defineScope();
    load();

    function loadParameters() {
        var parameters = VRNavigationService.getParameters($scope);
        $scope.roleId = undefined;
        if (parameters != undefined && parameters != null)
            $scope.roleId = parameters.roleId;

        if ($scope.roleId != undefined)
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
        return RoleAPIService.GetRole($scope.roleId)
           .then(function (response) {
               fillScopeFromRoleObj(response);
           })
            .catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error);
            });
    }



    function buildRoleObjFromScope() {
        var roleObject = {
            roleId: ($scope.roleId != null) ? $scope.roleId : 0,
            name: $scope.name,
            description: $scope.description
        };
        return roleObject;
    }

    function fillScopeFromRoleObj(roleObject) {
        $scope.name = roleObject.Name;
        $scope.description = roleObject.Description;
    }

    function insertRole() {
        $scope.issaving = true;
        var roleObject = buildRoleObjFromScope();
        return RoleAPIService.AddRole(roleObject)
        .then(function (response) {
            if (VRNotificationService.notifyOnItemAdded("Role", response)) {
                if ($scope.onRoleAdded != undefined)
                    $scope.onRoleAdded(response.InsertedObject);
                $scope.modalContext.closeModal();
            }
        }).catch(function (error) {
            VRNotificationService.notifyException(error);
        });

    }

    function updateRole() {
        var roleObject = buildRoleObjFromScope();
        RoleAPIService.UpdateRole(roleObject)
        .then(function (response) {
            if (VRNotificationService.notifyOnItemUpdated("Role", response)) {
                if ($scope.onRoleUpdated != undefined)
                    $scope.onRoleUpdated(response.UpdatedObject);
                $scope.modalContext.closeModal();
            }
        }).catch(function (error) {
            VRNotificationService.notifyException(error);
        });
    }
}
appControllers.controller('Security_RoleEditorController', RoleEditorController);

(function (appControllers) {

    "use strict";

    personilizationEditorController.$inject = ["$scope", "UtilsService", "VRNotificationService", "VRNavigationService", "VRUIUtilsService", "VR_Common_EntityPersonalizationAPIService"];

    function personilizationEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, VR_Common_EntityPersonalizationAPIService) {
        var changedItems;
        var userOptionsLabel;
        var justUser;
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                changedItems = parameters.changedItems;
                userOptionsLabel = parameters.userOptionsLabel;
                justUser = parameters.justUser;
            }
        }
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.userOptionsLabel = userOptionsLabel;
            $scope.scopeModel.hasUpdateGlobal = false;
            $scope.scopeModel.changedItems = [];
            if (justUser == false) {
                $scope.scopeModel.disabledCheck = true;
                $scope.scopeModel.allUsers = true;
            }
            if (justUser == true) {
                $scope.scopeModel.hideCheck = true;
            }

            $scope.scopeModel.savePesonilization = function () {
                var checkedEntityUniqueNames = getCheckedEntityUniqueNames();
                if ($scope.onSavePesonilization != undefined)
                    $scope.onSavePesonilization(checkedEntityUniqueNames, $scope.scopeModel.allUsers);
                $scope.modalContext.closeModal();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

        }
        function load() {
            if (changedItems != undefined && changedItems.length > 0)
                for (var i = 0 ; i < changedItems.length ; i++) {
                    var item = changedItems[i];
                    item.checked = true;
                    $scope.scopeModel.changedItems.push(item);
                }
            $scope.scopeModel.isLoading = true;
            VR_Common_EntityPersonalizationAPIService.DosesUserHaveUpdateGlobalEntityPersonalization().then(function (response) {
                $scope.scopeModel.hasUpdateGlobal = response;
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }


        function getCheckedEntityUniqueNames() {
            var entityUniqueNames = [];
            for (var i = 0 ; i < $scope.scopeModel.changedItems.length ; i++) {
                var item = $scope.scopeModel.changedItems[i];
                if (item.checked == true)
                    entityUniqueNames.push(item.EntityUniqueName);
            }
            return entityUniqueNames;
        }

    }

    appControllers.controller("VRCommon_PersonilizationEditorController", personilizationEditorController);
})(appControllers);

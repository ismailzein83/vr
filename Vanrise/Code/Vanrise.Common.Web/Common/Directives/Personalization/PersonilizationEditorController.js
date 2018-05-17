(function (appControllers) {

    "use strict";

    personilizationEditorController.$inject = ["$scope", "UtilsService", "VRNotificationService", "VRNavigationService", "VRUIUtilsService"];

    function personilizationEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {
        var changedItems;
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                changedItems = parameters.changedItems;
            }
        }
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.changedItems = [];
            $scope.scopeModel.savePesonilization = function () {
                var checkedEntityUniqueNames = getCheckedEntityUniqueNames();
                if ($scope.onSavePesonilization != undefined)
                    $scope.onSavePesonilization(checkedEntityUniqueNames);
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

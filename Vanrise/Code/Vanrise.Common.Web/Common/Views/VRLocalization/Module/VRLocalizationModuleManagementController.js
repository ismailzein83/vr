(function (app) {

    "use strict";

    VRLocalizationModuleManagementController.$inject = ['$scope', 'VRCommon_VRLocalizationModuleAPIService', 'VRCommon_VRLocalizationModuleService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

    function VRLocalizationModuleManagementController($scope, VRCommon_VRLocalizationModuleAPIService, VRCommon_VRLocalizationModuleService, UtilsService, VRUIUtilsService, VRNotificationService) {

        var gridAPI;

        defineScope();

        load();

        function defineScope() {

            $scope.scopeModel = {};

            $scope.scopeModel.search = function () {
                gridAPI.load(buildGridQuery());
            };

            $scope.scopeModel.addLocalizationModule = function () {
                var onVRLocalizationModuleAdded = function (addedItem) {
                    gridAPI.onVRLocalizationModuleAdded(addedItem);
                };
                VRCommon_VRLocalizationModuleService.addVRLocalizationModule(onVRLocalizationModuleAdded);
            };

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.load(buildGridQuery());
            };

        }

        function load() {
        }

        function buildGridQuery() {
            return {
                Name: $scope.scopeModel.name,
            };
        }


    }
    app.controller('VRCommon_VRLocalizationModuleManagementController', VRLocalizationModuleManagementController);

})(app);
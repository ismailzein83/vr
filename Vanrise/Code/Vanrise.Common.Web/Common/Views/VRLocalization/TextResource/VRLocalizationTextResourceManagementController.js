(function (app) {

    "use strict";

    VRLocalizationTextResourceManagementController.$inject = ['$scope', 'VRCommon_VRLocalizationTextResourceAPIService', 'VRCommon_VRLocalizationTextResourceService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

    function VRLocalizationTextResourceManagementController($scope, VRCommon_VRLocalizationTextResourceAPIService, VRCommon_VRLocalizationTextResourceService, UtilsService, VRUIUtilsService, VRNotificationService) {

        var gridAPI;

        defineScope();

        load();

        function defineScope() {

            $scope.scopeModel = {};

            $scope.scopeModel.search = function () {
                gridAPI.load(buildGridQuery());
            };

            $scope.scopeModel.addLocalizationTextResource = function () {

                var onVRLocalizationTextResourceAdded = function (addedItem) {
                    gridAPI.onVRLocalizationTextResourceAdded(addedItem);
                };

                VRCommon_VRLocalizationTextResourceService.addVRLocalizationTextResource(onVRLocalizationTextResourceAdded,undefined);
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
                ResourceKey: $scope.scopeModel.name,
            };
        }


    }
    app.controller('VRCommon_VRLocalizationTextResourceManagementController', VRLocalizationTextResourceManagementController);

})(app);
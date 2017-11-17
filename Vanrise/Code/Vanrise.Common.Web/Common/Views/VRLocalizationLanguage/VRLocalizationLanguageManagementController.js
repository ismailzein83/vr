( function(app) {

    "use strict";

    VRLocalizationLanguageManagementController.$inject = ['$scope', 'VRCommon_VRLocalizationLanguageAPIService', 'VRCommon_VRLocalizationLanguageService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

    function VRLocalizationLanguageManagementController($scope, VRCommon_VRLocalizationLanguageAPIService, VRCommon_VRLocalizationLanguageService, UtilsService, VRUIUtilsService, VRNotificationService) {

        var gridAPI;

        defineScope();

        load();

        function defineScope() {

            $scope.scopeModel = {};

            $scope.scopeModel.search = function () {
                gridAPI.load(buildGridQuery());
            };

            $scope.scopeModel.addLocalizationLanguage = function () {
                var onVRLocalizationLanguageAdded = function (addedItem) {
                    gridAPI.onVRLocalizationLanguageAdded(addedItem);
                };
                VRCommon_VRLocalizationLanguageService.addVRLocalizationLanguage(onVRLocalizationLanguageAdded);
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
    app.controller('VRCommon_VRLocalizationLanguageManagementController', VRLocalizationLanguageManagementController);

})(app);
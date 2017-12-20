(function (appControllers) {
    "use strict";
    EnterprisesDIDsViewController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'Retail_Teles_EnterpriseAPIService'];
    function EnterprisesDIDsViewController($scope, UtilsService, VRNotificationService, Retail_Teles_EnterpriseAPIService) {

        var gridAPI;

        defineScope();
        load();

        function defineScope() {

            $scope.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.load({});
            };
            $scope.save = function () {
                return Retail_Teles_EnterpriseAPIService.SaveAccountEnterprisesDIDs().then(function () {
                    VRNotificationService.showSuccess("Saved Successfully.");
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
            };
            $scope.search = function () {
                var query = buildGridQuery();
                return gridAPI.load(query);
            };
        }
        function load() {
        }

        function buildGridQuery() {
            return {
            };
        }
    }

    appControllers.controller('Retail_Teles_EnterprisesDIDsViewController', EnterprisesDIDsViewController);

})(appControllers);
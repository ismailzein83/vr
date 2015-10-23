(function (appControllers) {

    "use strict";

    RatePlanManagementController.$inject = ["$scope", "WhS_Sales_MainService"];

    function RatePlanManagementController($scope, WhS_Sales_MainService) {
        
        var carrierAccountDirectiveAPI;
        var ratePlanGridAPI;

        defineScope();
        load();

        function defineScope() {

            $scope.onCarrierAccountDirectiveReady = function (api) {
                carrierAccountDirectiveAPI = api;
                load();
            };

            $scope.onRatePlanGridReady = function (api) {
                ratePlanGridAPI = api;
            };

            $scope.search = function () {
                if (ratePlanGridAPI != undefined) {
                    $scope.showRatePlanGrid = true;

                    var query = {
                        CustomerId: carrierAccountDirectiveAPI.getData().CarrierAccountId
                    };

                    return ratePlanGridAPI.loadGrid(query);
                }
            };

            $scope.assignZones = function () {

                var onRatePlanUpdated = function () {
                    console.log("Rate plan has been updated");
                };
                
                WhS_Sales_MainService.editRatePlan(carrierAccountDirectiveAPI.getData().CarrierAccountId, onRatePlanUpdated);
            };
        }

        function load() {
            if (carrierAccountDirectiveAPI == undefined)
                return;

            $scope.loadingFilters = true;

            carrierAccountDirectiveAPI.load()
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose($scope, error);
                })
                .finally(function () {
                    $scope.loadingFilters = false;
                });
        }
    }

    appControllers.controller("WhS_Sales_RatePlanManagementController", RatePlanManagementController);

})(appControllers);

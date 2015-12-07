(function (appControllers) {

    "use strict";

    RatePlanSettingsController.$inject = ["$scope", "VRNavigationService", "VRNotificationService"];

    function RatePlanSettingsController($scope, VRNavigationService, VRNotificationService) {
        var settings;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters) {
                settings = parameters.settings;
            }
        }

        function defineScope() {
            $scope.title = "Edit Rate Plan Settings";
            $scope.tabItems = [{
                title: "Cost Columns",
                directive: "vr-whs-sales-costcolumns",
                loadDirective: function (api) {
                    return api.load(settings);
                }
            }];

            $scope.saveSettings = function () {
                var settings = {};
                settings.CostCalculationMethods = $scope.tabItems[0].directiveAPI ? $scope.tabItems[0].directiveAPI.getData() : null;

                if ($scope.onSettingsUpdate && typeof ($scope.onSettingsUpdate) == "function")
                    $scope.onSettingsUpdate(settings);

                $scope.modalContext.closeModal();
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {

        }
    }

    appControllers.controller("WhS_Sales_RatePlanSettingsController", RatePlanSettingsController);

})(appControllers);

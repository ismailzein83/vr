(function (appControllers) {

    "use strict";

    BulkActionWizardController.$inject = ["$scope"];

    function BulkActionWizardController($scope) {
        var settings;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            //var parameters = VRNavigationService.getParameters($scope);

            //if (parameters) {
            //    settings = parameters.settings;
            //}
        }

        function defineScope() {
            //$scope.title = "Edit Rate Plan Settings";
            //$scope.tabItems = [{
            //    title: "Cost Columns",
            //    directive: "vr-whs-sales-costcolumns",
            //    loadDirective: function (api) {
            //        return api.load(settings);
            //    }
            //}];

            //$scope.saveSettings = function () {

            //    var settings;
            //    var costCalculationMethods;

            //    var directiveAPI = $scope.tabItems[0].directiveAPI;
            //    if (directiveAPI != undefined)
            //        costCalculationMethods = directiveAPI.getData();

            //    if (costCalculationMethods != undefined) {
            //        settings = {
            //            costCalculationMethods: costCalculationMethods
            //        };
            //    }

            //    if ($scope.onSettingsUpdated != null && typeof $scope.onSettingsUpdated == "function")
            //        $scope.onSettingsUpdated(settings);

            //    $scope.modalContext.closeModal();
            //};

            //$scope.close = function () {
            //    $scope.modalContext.closeModal();
            //};
        }

        function load() {

        }
    }

    appControllers.controller("WhS_Sales_BulkActionWizardController", BulkActionWizardController);

})(appControllers);

(function (appControllers) {

    "use strict";

    PricingTemplateManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'WhS_Sales_PricingTemplateService', 'WhS_Sales_PricingTemplateAPIService'];

    function PricingTemplateManagementController($scope, UtilsService, VRUIUtilsService, WhS_Sales_PricingTemplateService, WhS_Sales_PricingTemplateAPIService) {

        var gridAPI;

        defineScope();
        load();


        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.load({});
            };

            $scope.scopeModel.search = function () {
                var query = buildGridQuery();
                return gridAPI.load(query);
            };

            $scope.scopeModel.add = function () {
                var onPricingTemplateAdded = function (addedPricingTemplate) {
                    gridAPI.onPricingTemplateAdded(addedPricingTemplate);
                };

                WhS_Sales_PricingTemplateService.addPricingTemplate(onPricingTemplateAdded);
            };

            $scope.scopeModel.hasAddPricingTemplatePermission = function () {
                return WhS_Sales_PricingTemplateAPIService.HasAddPricingTemplatePermission()
            };
        }

        function load() {

        }

        function buildGridQuery() {
            return {
                Name: $scope.scopeModel.name
            };
        }
    }

    appControllers.controller('WhS_Sales_PricingTemplateManagementController', PricingTemplateManagementController);

})(appControllers);
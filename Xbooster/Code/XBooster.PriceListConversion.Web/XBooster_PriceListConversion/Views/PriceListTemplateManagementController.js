(function (appControllers) {
    'use strict';

    PriceListTemplateManagementController.$inject = ['$scope', 'XBooster_PriceListConversion_PriceListTemplateService', 'XBooster_PriceListConversion_PriceListTemplateAPIService'];

    function PriceListTemplateManagementController($scope, XBooster_PriceListConversion_PriceListTemplateService, XBooster_PriceListConversion_PriceListTemplateAPIService) {

        var priceListTemplateGridAPI;
        

        defineScope();
        load();

        function defineScope() {
            $scope.onGridReady = function (api) {
                priceListTemplateGridAPI = api;
                priceListTemplateGridAPI.loadGrid({ });
            };

            $scope.search = function () {
                return priceListTemplateGridAPI.loadGrid(getFilterObject());
            };
            $scope.hasaddPriceListTemplatePermission = function () {
                return XBooster_PriceListConversion_PriceListTemplateAPIService.HasaddOutputPriceListTemplatePermission();
            };
            $scope.addPriceListTemplate = function () {
                var onPriceListTemplateAdded = function (priceListTemplateObj) {
                    priceListTemplateGridAPI.onPriceListTemplateAdded(priceListTemplateObj);
                };
                XBooster_PriceListConversion_PriceListTemplateService.addOutputPriceListTemplate(onPriceListTemplateAdded);
            };
        }

        function load() {

        }

        function getFilterObject() {
          var  filter = {
              Name: $scope.name,
          };
          return filter;
        }
    }

    appControllers.controller('XBooster_PriceListConversion_PriceListTemplateManagementController', PriceListTemplateManagementController);

})(appControllers);

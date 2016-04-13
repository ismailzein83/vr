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
                priceListTemplateGridAPI.loadGrid({});
            };

            $scope.search = function () {
                return priceListTemplateGridAPI.loadGrid(getFilterObject);
            };
            //$scope.hasAddDataStore = function () {
            //    return VR_GenericData_DataStoreAPIService.HasAddDataStore();
            //}
            $scope.addPriceListTemplate = function () {
                var onPriceListTemplateAdded = function (dataStoreObj) {
                    //DatStoreGridAPI.onDataStoreAdded(dataStoreObj);
                };

                XBooster_PriceListConversion_PriceListTemplateService.addPriceListTemplate(onPriceListTemplateAdded);
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
